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
[Authorize(Roles = "Admin,Gestor")]
public class MaintenancePlanController : ControllerBase
{
    private readonly IMaintenancePlanRepository _planRepository;

    public MaintenancePlanController(IMaintenancePlanRepository planRepository)
    {
        _planRepository = planRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var plans = await _planRepository.GetAllAsync();

        var response = plans.Select(plan => new CreateMaintenancePlanResponse
        {
            Id = plan.Id,
            Title = plan.Title,
            Description = plan.Description,
            Frequency = $"{plan.FrequencyType} ({plan.FrequencyValue} dias)",
            IsActive = plan.IsActive
        });

        return Ok(new
        {
            message = "Planos de manutenção carregados com sucesso.",
            data = response
        });
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActive()
    {
        var plans = await _planRepository.GetActivePlansAsync();

        var response = plans.Select(plan => new CreateMaintenancePlanResponse
        {
            Id = plan.Id,
            Title = plan.Title,
            Description = plan.Description,
            Frequency = $"{plan.FrequencyType} ({plan.FrequencyValue} dias)",
            IsActive = plan.IsActive
        });

        return Ok(new
        {
            message = "Planos de manutenção ativos carregados com sucesso.",
            data = response
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var plan = await _planRepository.GetByIdAsync(id);
        if (plan == null)
            return NotFound(new { message = $"Plano de manutenção com ID {id} não foi encontrado." });

        var response = new CreateMaintenancePlanResponse
        {
            Id = plan.Id,
            Title = plan.Title,
            Description = plan.Description,
            Frequency = $"{plan.FrequencyType} ({plan.FrequencyValue} dias)",
            IsActive = plan.IsActive
        };

        return Ok(new
        {
            message = "Plano de manutenção encontrado com sucesso.",
            data = response
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMaintenancePlanRequest dto)
    {
        var userId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

        var plan = new MaintenancePlan
        {
            Title = dto.Title!,
            Description = dto.Description!,
            FrequencyType = (FrequencyType)dto.FrequencyType,
            FrequencyValue = dto.FrequencyValue,
            IsActive = dto.IsActive,
            CreatedByUserId = userId
        };

        await _planRepository.AddAsync(plan);
        await _planRepository.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = plan.Id }, new
        {
            message = "Plano de manutenção criado com sucesso.",
            data = new { plan.Id }
        });
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateMaintenancePlanRequest dto)
    {
        var existing = await _planRepository.GetByIdAsync(id);
        if (existing == null)
            return NotFound(new { message = $"Plano com ID {id} não foi encontrado." });

        existing.Title = dto.Title;
        existing.Description = dto.Description;
        existing.FrequencyType = (FrequencyType)dto.FrequencyType;
        existing.FrequencyValue = dto.FrequencyValue;
        existing.IsActive = dto.IsActive;

        _planRepository.Update(existing);
        await _planRepository.SaveChangesAsync();

        return Ok(new { message = "Plano de manutenção atualizado com sucesso." });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var plan = await _planRepository.GetByIdAsync(id);
        if (plan == null)
            return NotFound(new { message = $"Plano com ID {id} não foi encontrado." });

        _planRepository.Delete(plan);
        await _planRepository.SaveChangesAsync();

        return Ok(new { message = "Plano de manutenção deletado com sucesso." });
    }

    [HttpGet("mine")]
    public async Task<IActionResult> GetMyPlans()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Token inválido ou ID do usuário não encontrado." });
        }
        var plans = await _planRepository.GetPlansByCreatorAsync(userId);

        var response = plans.Select(plan => new CreateMaintenancePlanResponse
        {
            Id = plan.Id,
            Title = plan.Title,
            Description = plan.Description,
            Frequency = $"{plan.FrequencyType} ({plan.FrequencyValue} dias)",
            IsActive = plan.IsActive
        });

        return Ok(new
        {
            message = "Planos de manutenção criados por você.",
            data = response
        });
    }

}
