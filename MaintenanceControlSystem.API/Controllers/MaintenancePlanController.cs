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

    private int ObterIdUsuarioAtual()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
    }

    [HttpGet]
    public async Task<IActionResult> ObterTodos()
    {
        var planos = await _planRepository.GetAllAsync();
        var resposta = planos.Select(MapearPlanoParaResposta);

        return Ok(new
        {
            mensagem = "Planos de manutenção carregados com sucesso.",
            dados = resposta
        });
    }

    [HttpGet("active")]
    public async Task<IActionResult> ObterAtivos()
    {
        var planos = await _planRepository.GetActivePlansAsync();
        var resposta = planos.Select(MapearPlanoParaResposta);

        return Ok(new
        {
            mensagem = "Planos de manutenção ativos carregados com sucesso.",
            dados = resposta
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var userId = ObterIdUsuarioAtual();
        var plano = await _planRepository.GetByIdAsync(id);

        if (plano == null || plano.CreatedByUserId != userId)
            return NotFound(new { mensagem = "Plano de manutenção não encontrado ou acesso negado." });

        return Ok(new { mensagem = "Plano de manutenção encontrado com sucesso.", dados = MapearPlanoParaResposta(plano) });
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CreateMaintenancePlanRequest dto)
    {
        var userId = ObterIdUsuarioAtual();

        var plano = new MaintenancePlan
        {
            Title = dto.Title!,
            Description = dto.Description!,
            FrequencyType = (FrequencyType)dto.FrequencyType,
            FrequencyValue = dto.FrequencyValue,
            IsActive = dto.IsActive,
            CreatedByUserId = userId
        };

        await _planRepository.AddAsync(plano);
        await _planRepository.SaveChangesAsync();

        return CreatedAtAction(nameof(ObterPorId), new { id = plano.Id },
        new
        {
            mensagem = "Plano de manutenção criado com sucesso.",
            dados = new { plano.Id }
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(int id, [FromBody] CreateMaintenancePlanRequest dto)
    {
        var userId = ObterIdUsuarioAtual();
        var plano = await _planRepository.GetByIdAsync(id);

        if (plano == null || plano.CreatedByUserId != userId)
            return NotFound(new { mensagem = "Plano de manutenção não encontrado ou acesso negado." });

        plano.Title = dto.Title;
        plano.Description = dto.Description;
        plano.FrequencyType = (FrequencyType)dto.FrequencyType;
        plano.FrequencyValue = dto.FrequencyValue;
        plano.IsActive = dto.IsActive;

        _planRepository.Update(plano);
        await _planRepository.SaveChangesAsync();

        return Ok(new { mensagem = "Plano de manutenção atualizado com sucesso." });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Excluir(int id)
    {
        var userId = ObterIdUsuarioAtual();
        var plano = await _planRepository.GetByIdAsync(id);

        if (plano == null || plano.CreatedByUserId != userId)
            return NotFound(new { mensagem = "Plano de manutenção não encontrado ou acesso negado." });

        _planRepository.Delete(plano);
        await _planRepository.SaveChangesAsync();

        return Ok(new { mensagem = "Plano de manutenção excluído com sucesso." });
    }

    [HttpGet("mine")]
    public async Task<IActionResult> ObterMeusPlanos()
    {
        var userId = ObterIdUsuarioAtual();
        var planos = await _planRepository.GetPlansByCreatorAsync(userId);

        var resposta = planos.Select(MapearPlanoParaResposta);
        return Ok(new
        {
            mensagem = "Seus planos de manutenção carregados com sucesso.",
            dados = resposta
        });
    }

    private CreateMaintenancePlanResponse MapearPlanoParaResposta(MaintenancePlan plano)
    {
        return new CreateMaintenancePlanResponse
        {
            Id = plano.Id,
            Title = plano.Title,
            Description = plano.Description,
            Frequency = $"{plano.FrequencyType} ({plano.FrequencyValue} dias)",
            IsActive = plano.IsActive
        };
    }
}
