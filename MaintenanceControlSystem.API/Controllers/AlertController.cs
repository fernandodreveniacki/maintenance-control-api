using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MaintenanceControlSystem.Application.DTOs;
using MaintenanceControlSystem.Infrastructure.Repositories.Interfaces;
using MaintenanceControlSystem.Domain.Entities;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace MaintenanceControlSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AlertController : ControllerBase
{
    private readonly IAlertRepository _alertRepository;
    private readonly IMaintenancePlanAssignmentRepository _assignmentRepository;
    public AlertController(IAlertRepository alertRepository, IMaintenancePlanAssignmentRepository assignmentRepository)
    {
        _alertRepository = alertRepository;
        _assignmentRepository = assignmentRepository;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateAlert([FromBody] CreateAlertRequest dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var assignments = await _assignmentRepository.GetByMachineIdAsync(dto.MachineId);
        var assignment = assignments.FirstOrDefault();

        if (assignment == null)
        {
            return BadRequest(new { message = $"Nenhum plano de manutenção encontrado para a máquina {dto.MachineId}." });
        }

        var alert = new Alert
        {
            Message = dto.Message,
            IsRead = false,
            MachineId = dto.MachineId,
            AssignmentId = assignment.Id,
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = userId
        };

        await _alertRepository.AddAsync(alert);
        await _alertRepository.SaveChangesAsync();

        var response = new CreateAlertResponse
        {
            Id = alert.Id,
            Message = alert.Message,
            MachineId = alert.MachineId,
            IsRead = alert.IsRead,
            CreatedAt = alert.CreatedAt
        };

        return CreatedAtAction(nameof(GetById), new { id = alert.Id }, new
        {
            message = "Alerta criado com sucesso.",
            data = response
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var alert = await _alertRepository.GetByIdAsync(id);
        if (alert == null)
            return NotFound(new { message = $"Alerta com ID {id} não foi encontrado." });

        var response = new CreateAlertResponse
        {
            Id = alert.Id,
            Message = alert.Message,
            MachineId = alert.MachineId,
            IsRead = alert.IsRead,
            CreatedAt = alert.CreatedAt
        };

        return Ok(new
        {
            message = "Alerta carregado com sucesso.",
            data = response
        });
    }

    [HttpGet("machine/{machineId}")]
    public async Task<IActionResult> GetByMachine(int machineId)
    {
        var alerts = await _alertRepository.GetByMachineAsync(machineId);

        var response = alerts.Select(a => new CreateAlertResponse
        {
            Id = a.Id,
            Message = a.Message,
            MachineId = a.MachineId,
            IsRead = a.IsRead,
            CreatedAt = a.CreatedAt
        });

        return Ok(new
        {
            message = $"Alertas da máquina {machineId} carregados com sucesso.",
            data = response
        });
    }


    [HttpGet("unread")]
    public async Task<IActionResult> GetUnread()
    {
        var alerts = await _alertRepository.GetUnreadAsync();

        var response = alerts.Select(a => new CreateAlertResponse
        {
            Id = a.Id,
            Message = a.Message,
            IsRead = a.IsRead,
            MachineId = a.MachineId,
            CreatedAt = a.CreatedAt
        });

        return Ok(new
        {
            message = "Alertas não lidos carregados com sucesso.",
            data = response
        });
    }


    [HttpPut("{id}/read")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var alert = await _alertRepository.GetByIdAsync(id);
        if (alert == null)
            return NotFound(new { message = $"Alerta com ID {id} não foi encontrado." });

        alert.IsRead = true;
        _alertRepository.Update(alert);
        await _alertRepository.SaveChangesAsync();

        return Ok(new { message = "Alerta marcado como lido com sucesso." });
    }
    [HttpGet("mine")]
    public async Task<IActionResult> GetMyAlerts()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Token inválido ou ID do usuário não encontrado." });
        }
        var alerts = await _alertRepository.GetAlertsByCreatorAsync(userId);

        var response = alerts.Select(a => new CreateAlertResponse
        {
            Id = a.Id,
            Message = a.Message,
            IsRead = a.IsRead,
            MachineId = a.Assignment?.MachineId ?? 0,
            CreatedAt = a.CreatedAt
        });

        return Ok(new
        {
            message = "Alertas gerados para planos criados por você.",
            data = response
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var alerts = await _alertRepository.GetAllAsync();

        var response = alerts.Select(a => new CreateAlertResponse
        {
            Id = a.Id,
            Message = a.Message,
            IsRead = a.IsRead,
            MachineId = a.MachineId,
            CreatedAt = a.CreatedAt
        });

        return Ok(new
        {
            message = "Alertas carregados com sucesso.",
            data = response
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateAlertRequest dto)
    {
        var existing = await _alertRepository.GetByIdAsync(id);
        if (existing == null)
            return NotFound(new { message = $"Alerta com ID {id} não foi encontrado." });

        existing.Title = dto.Title;
        existing.Message = dto.Message;
        existing.MachineId = dto.MachineId;
        existing.IsRead = dto.IsRead;

        _alertRepository.Update(existing);
        await _alertRepository.SaveChangesAsync();

        return Ok(new { message = "Alerta atualizado com sucesso." });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var alert = await _alertRepository.GetByIdAsync(id);
        if (alert == null)
            return NotFound(new { message = $"Alerta com ID {id} não foi encontrado." });

        _alertRepository.Delete(alert);
        await _alertRepository.SaveChangesAsync();

        return Ok(new { message = "Alerta deletado com sucesso." });
    }



}
