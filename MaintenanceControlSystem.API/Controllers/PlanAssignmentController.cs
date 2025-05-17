using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MaintenanceControlSystem.Application.DTOs;
using MaintenanceControlSystem.Domain.Entities;
using MaintenanceControlSystem.Infrastructure.Repositories.Interfaces;
using System.Security.Claims;

namespace MaintenanceControlSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Gestor")]
public class PlanAssignmentController : ControllerBase
{
    private readonly IMaintenancePlanAssignmentRepository _assignmentRepository;
    private readonly IMachineRepository _machineRepository;
    private readonly IMaintenancePlanRepository _planRepository;

    public PlanAssignmentController(
        IMaintenancePlanAssignmentRepository assignmentRepository,
        IMachineRepository machineRepository,
        IMaintenancePlanRepository planRepository)
    {
        _assignmentRepository = assignmentRepository;
        _machineRepository = machineRepository;
        _planRepository = planRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var assignments = await _assignmentRepository.GetAllAsync();

        var response = assignments.Select(a => new CreatePlanAssignmentResponse
        {
            Id = a.Id,
            MachineName = a.Machine?.Name ?? "N/A",
            PlanTitle = a.MaintenancePlan?.Title ?? "N/A",
            NextDueDate = a.NextDueDate,
            LastPerformed = a.LastPerformed
        });

        return Ok(new
        {
            message = "Lista de atribuições carregada com sucesso.",
            data = response
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var assignment = await _assignmentRepository.GetByIdAsync(id);
        if (assignment == null)
            return NotFound(new { message = $"Atribuição com ID {id} não foi encontrada." });

        var response = new CreatePlanAssignmentResponse
        {
            Id = assignment.Id,
            MachineName = assignment.Machine?.Name ?? "N/A",
            PlanTitle = assignment.MaintenancePlan?.Title ?? "N/A",
            NextDueDate = assignment.NextDueDate,
            LastPerformed = assignment.LastPerformed
        };

        return Ok(new
        {
            message = "Atribuição encontrada com sucesso.",
            data = response
        });
    }

    [HttpGet("machine/{machineId}")]
    public async Task<IActionResult> GetByMachine(int machineId)
    {
        var assignments = await _assignmentRepository.GetByMachineIdAsync(machineId);

        var response = assignments.Select(a => new CreatePlanAssignmentResponse
        {
            Id = a.Id,
            MachineName = a.Machine?.Name ?? "N/A",
            PlanTitle = a.MaintenancePlan?.Title ?? "N/A",
            NextDueDate = a.NextDueDate,
            LastPerformed = a.LastPerformed
        });

        return Ok(new
        {
            message = $"Atribuições da máquina {machineId} carregadas com sucesso.",
            data = response
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePlanAssignmentRequest dto)
    {

        var machine = await _machineRepository.GetByIdAsync(dto.MachineId);
        var plan = await _planRepository.GetByIdAsync(dto.MaintenancePlanId);

        if (machine == null || plan == null)
            return BadRequest("Machine or Maintenance Plan not found.");

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Token inválido ou ID do usuário não encontrado." });
        }

        var assignment = new MaintenancePlanAssignment
        {
            MachineId = dto.MachineId,
            MaintenancePlanId = dto.MaintenancePlanId,
            NextDueDate = dto.NextDueDate,
            CreatedByUserId = userId
        };

        await _assignmentRepository.AddAsync(assignment);
        await _assignmentRepository.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = assignment.Id }, new
        {
            message = "Plano atribuído com sucesso.",
            data = new { assignment.Id }
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreatePlanAssignmentRequest dto)
    {
        var existing = await _assignmentRepository.GetByIdAsync(id);
        if (existing == null)
            return NotFound(new { message = $"Atribuição com ID {id} não foi encontrada." });

        var machine = await _machineRepository.GetByIdAsync(dto.MachineId);
        var plan = await _planRepository.GetByIdAsync(dto.MaintenancePlanId);

        if (machine == null || plan == null)
            return BadRequest(new { message = "Máquina ou Plano de manutenção não encontrado." });

        existing.MachineId = dto.MachineId;
        existing.MaintenancePlanId = dto.MaintenancePlanId;
        existing.NextDueDate = dto.NextDueDate;

        _assignmentRepository.Update(existing);
        await _assignmentRepository.SaveChangesAsync();

        return Ok(new { message = "Atribuição atualizada com sucesso." });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var assignment = await _assignmentRepository.GetByIdAsync(id);
        if (assignment == null)
            return NotFound(new { message = $"Atribuição com ID {id} não foi encontrada." });

        _assignmentRepository.Delete(assignment);
        await _assignmentRepository.SaveChangesAsync();

        return Ok(new { message = "Atribuição deletada com sucesso." });
    }

    [HttpGet("mine")]
    public async Task<IActionResult> GetMyAssignments()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Token inválido ou ID do usuário não encontrado." });
        }

        var assignments = await _assignmentRepository.GetAssignmentsByCreatorAsync(userId);

        var response = assignments.Select(a => new CreatePlanAssignmentResponse
        {
            Id = a.Id,
            MachineName = a.Machine?.Name ?? "N/A",
            PlanTitle = a.MaintenancePlan?.Title ?? "N/A",
            NextDueDate = a.NextDueDate,
            LastPerformed = a.LastPerformed
        });

        return Ok(new
        {
            message = "Atribuições de planos criadas por você.",
            data = response
        });
    }

}
