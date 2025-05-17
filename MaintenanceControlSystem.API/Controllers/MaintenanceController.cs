using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MaintenanceControlSystem.Application.DTOs;
using MaintenanceControlSystem.Domain.Entities;
using MaintenanceControlSystem.Infrastructure.Repositories.Interfaces;
using MaintenanceControlSystem.Domain.Enums;
using System.Security.Claims;

namespace MaintenanceControlSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Tecnico")]
public class MaintenanceController : ControllerBase
{
    private readonly IMaintenanceRepository _maintenanceRepository;
    private readonly IMachineRepository _machineRepository;

    public MaintenanceController(IMaintenanceRepository maintenanceRepository, IMachineRepository machineRepository)
    {
        _maintenanceRepository = maintenanceRepository;
        _machineRepository = machineRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _maintenanceRepository.GetAllAsync();

        var response = list.Select(m => new CreateMaintenanceResponse
        {
            Id = m.Id,
            MachineName = m.Machine?.Name ?? "N/A",
            Type = m.Type.ToString(),
            PerformedAt = m.PerformedAt,
            DurationHours = m.DurationHours,
            Description = m.Description,
            RootCause = m.RootCause,
            CorrectiveAction = m.CorrectiveAction
        });

        return Ok(new
        {
            message = "Lista de manutenções carregada com sucesso.",
            data = response
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _maintenanceRepository.GetByIdAsync(id);
        if (item == null)
            return NotFound(new { message = $"Manutenção com ID {id} não foi encontrada." });

        var response = new CreateMaintenanceResponse
        {
            Id = item.Id,
            MachineName = item.Machine?.Name ?? "N/A",
            Type = item.Type.ToString(),
            PerformedAt = item.PerformedAt,
            DurationHours = item.DurationHours,
            Description = item.Description,
            RootCause = item.RootCause,
            CorrectiveAction = item.CorrectiveAction
        };

        return Ok(new
        {
            message = "Manutenção encontrada com sucesso.",
            data = response
        });
    }

    [HttpGet("machine/{machineId}")]
    public async Task<IActionResult> GetByMachine(int machineId)
    {
        var list = await _maintenanceRepository.GetByMachineIdAsync(machineId);

        var response = list.Select(m => new CreateMaintenanceResponse
        {
            Id = m.Id,
            MachineName = m.Machine?.Name ?? "N/A",
            Type = m.Type.ToString(),
            PerformedAt = m.PerformedAt,
            DurationHours = m.DurationHours,
            Description = m.Description,
            RootCause = m.RootCause,
            CorrectiveAction = m.CorrectiveAction
        });

        return Ok(new
        {
            message = $"Manutenções da máquina {machineId} carregadas com sucesso.",
            data = response
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMaintenanceRequest dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Token inválido ou ID do usuário não encontrado." });
        }

        var machine = await _machineRepository.GetByIdAsync(dto.MachineId);
        if (machine == null)
            return BadRequest(new { message = "Máquina não encontrada." });

        var maintenance = new Maintenance
        {
            MachineId = dto.MachineId,
            Type = (MaintenanceType)dto.Type,
            PerformedAt = dto.PerformedAt,
            DurationHours = (float)dto.DurationHours,
            Description = dto.Description,
            RootCause = dto.RootCause,
            CorrectiveAction = dto.CorrectiveAction,
            CreatedByUserId = userId // ESSENCIAL
        };

        await _maintenanceRepository.AddAsync(maintenance);
        await _maintenanceRepository.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = maintenance.Id }, new
        {
            message = "Manutenção registrada com sucesso.",
            data = new { maintenance.Id }
        });
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateMaintenanceRequest updated)
    {
        var existing = await _maintenanceRepository.GetByIdAsync(id);
        if (existing == null)
            return NotFound(new { message = $"Manutenção com ID {id} não foi encontrada." });

        existing.Type = (MaintenanceType)updated.Type;
        existing.PerformedAt = updated.PerformedAt;
        existing.DurationHours = (float)updated.DurationHours;
        existing.RootCause = updated.RootCause;
        existing.CorrectiveAction = updated.CorrectiveAction;

        _maintenanceRepository.Update(existing);
        await _maintenanceRepository.SaveChangesAsync();

        return Ok(new { message = "Manutenção atualizada com sucesso." });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _maintenanceRepository.GetByIdAsync(id);
        if (item == null)
            return NotFound(new { message = $"Manutenção com ID {id} não foi encontrada." });

        _maintenanceRepository.Delete(item);
        await _maintenanceRepository.SaveChangesAsync();

        return Ok(new { message = "Manutenção deletada com sucesso." });
    }
    [HttpGet("mine")]
    public async Task<IActionResult> GetMyMaintenances()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Token inválido ou ID do usuário não encontrado." });
        }
        var maintenances = await _maintenanceRepository.GetMaintenancesByCreatorAsync(userId);

        var response = maintenances.Select(m => new CreateMaintenanceResponse
        {
            Id = m.Id,
            MachineName = m.Machine?.Name ?? "N/A",
            Type = m.Type.ToString(),
            PerformedAt = m.PerformedAt,
            DurationHours = m.DurationHours,
            Description = m.Description,
            RootCause = m.RootCause,
            CorrectiveAction = m.CorrectiveAction
        });

        return Ok(new
        {
            message = "Manutenções registradas por você.",
            data = response
        });
    }

}
