using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MaintenanceControlSystem.Application.DTOs;
using MaintenanceControlSystem.Domain.Entities;
using MaintenanceControlSystem.Infrastructure.Repositories.Interfaces;
using MaintenanceControlSystem.Domain.Enums;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace MaintenanceControlSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Gestor")]
public class MachineController : ControllerBase
{
    private readonly IMachineRepository _machineRepository;

    public MachineController(IMachineRepository machineRepository)
    {
        _machineRepository = machineRepository;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMachineRequest dto)
    {
        var existing = await _machineRepository.GetByCodeAsync(dto.Code);
        if (existing != null)
        {
            return BadRequest(new { message = $"Já existe uma máquina com o código '{dto.Code}'." });
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Token inválido ou ID do usuário não encontrado." });
        }

        var machine = new Machine
        {
            Name = dto.Name,
            Code = dto.Code,
            Location = dto.Location,
            InstallationDate = dto.InstallationDate,
            Status = (MachineStatus)dto.Status,
            Manufacturer = dto.Manufacturer,
            CreatedByUserId = userId 
        };

        await _machineRepository.AddAsync(machine);
        await _machineRepository.SaveChangesAsync();

        var response = new CreateMachineResponse
        {
            Id = machine.Id,
            Name = machine.Name,
            Code = machine.Code,
            Location = machine.Location,
            InstallationDate = machine.InstallationDate,
            Status = machine.Status.ToString(),
            Manufacturer = machine.Manufacturer
        };

        return CreatedAtAction(nameof(GetById), new { id = machine.Id }, new
        {
            message = "Máquina criada com sucesso.",
            data = response
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var machines = await _machineRepository.GetAllAsync();

        var response = machines.Select(machine => new CreateMachineResponse
        {
            Id = machine.Id,
            Name = machine.Name,
            Code = machine.Code,
            Location = machine.Location,
            InstallationDate = machine.InstallationDate,
            Status = machine.Status.ToString(),
            Manufacturer = machine.Manufacturer
        });

        return Ok(new
        {
            message = "Lista de máquinas carregada com sucesso.",
            data = response
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var machine = await _machineRepository.GetByIdAsync(id);
        if (machine == null)
        {
            return NotFound(new { message = $"Máquina com ID {id} não foi encontrada." });
        }

        var response = new CreateMachineResponse
        {
            Id = machine.Id,
            Name = machine.Name,
            Code = machine.Code,
            Location = machine.Location,
            InstallationDate = machine.InstallationDate,
            Status = machine.Status.ToString(),
            Manufacturer = machine.Manufacturer
        };

        return Ok(new
        {
            message = "Máquina encontrada com sucesso.",
            data = response
        });
    }

    [HttpGet("mine")]
    public async Task<IActionResult> GetMyMachines()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Token inválido ou ID do usuário não encontrado." });
        }
        var machines = await _machineRepository.GetMachinesByCreatorAsync(userId);

        var response = machines.Select(machine => new CreateMachineResponse
        {
            Id = machine.Id,
            Name = machine.Name,
            Code = machine.Code,
            Location = machine.Location,
            InstallationDate = machine.InstallationDate,
            Status = machine.Status.ToString(),
            Manufacturer = machine.Manufacturer
        });

        return Ok(new
        {
            message = "Máquinas criadas por você.",
            data = response
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateMachineRequest dto)
    {
        var existing = await _machineRepository.GetByIdAsync(id);
        if (existing == null)
            return NotFound(new { message = $"Máquina com ID {id} não foi encontrada." });

        existing.Name = dto.Name;
        existing.Description = dto.Description;
        existing.Status = dto.Status;

        _machineRepository.Update(existing);
        await _machineRepository.SaveChangesAsync();

        return Ok(new { message = "Máquina atualizada com sucesso." });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var machine = await _machineRepository.GetByIdAsync(id);
        if (machine == null)
            return NotFound(new { message = $"Máquina com ID {id} não foi encontrada." });

        _machineRepository.Delete(machine);
        await _machineRepository.SaveChangesAsync();

        return Ok(new { message = "Máquina deletada com sucesso." });
    }



}
