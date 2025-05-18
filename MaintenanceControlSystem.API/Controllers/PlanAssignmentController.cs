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

    private int ObterIdUsuarioAtual()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
    }

    [HttpGet]
    public async Task<IActionResult> ObterTodas()
    {
        var atribuicoes = await _assignmentRepository.GetAllAsync();
        var resposta = atribuicoes.Select(MapearAtribuicaoParaResposta);

        return Ok(new { mensagem = "Lista de atribuições carregada com sucesso.", dados = resposta });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var userId = ObterIdUsuarioAtual();
        var atribuicao = await _assignmentRepository.GetByIdAsync(id);

        if (atribuicao == null || atribuicao.CreatedByUserId != userId)
            return NotFound(new { mensagem = "Atribuição não encontrada ou acesso negado." });

        return Ok(new { mensagem = "Atribuição carregada com sucesso.", dados = MapearAtribuicaoParaResposta(atribuicao) });
    }

    [HttpGet("machine/{machineId}")]
    public async Task<IActionResult> ObterPorMaquina(int machineId)
    {
        var userId = ObterIdUsuarioAtual();
        var atribuicoes = await _assignmentRepository.GetByMachineIdAsync(machineId);

        var resposta = atribuicoes
            .Where(a => a.CreatedByUserId == userId)
            .Select(MapearAtribuicaoParaResposta);

        return Ok(new { mensagem = $"Atribuições da máquina {machineId} carregadas com sucesso.", dados = resposta });
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CreatePlanAssignmentRequest dto)
    {
        var userId = ObterIdUsuarioAtual();
        var maquina = await _machineRepository.GetByIdAsync(dto.MachineId);
        var plano = await _planRepository.GetByIdAsync(dto.MaintenancePlanId);

        if (maquina == null || plano == null)
            return BadRequest(new { mensagem = "Máquina ou Plano de manutenção não encontrado." });

        var atribuicao = new MaintenancePlanAssignment
        {
            MachineId = dto.MachineId,
            MaintenancePlanId = dto.MaintenancePlanId,
            NextDueDate = dto.NextDueDate,
            CreatedByUserId = userId
        };

        await _assignmentRepository.AddAsync(atribuicao);
        await _assignmentRepository.SaveChangesAsync();

        return CreatedAtAction(nameof(ObterPorId), new { id = atribuicao.Id },
        new { mensagem = "Plano atribuído com sucesso.", dados = new { atribuicao.Id } });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(int id, [FromBody] CreatePlanAssignmentRequest dto)
    {
        var userId = ObterIdUsuarioAtual();
        var atribuicao = await _assignmentRepository.GetByIdAsync(id);

        if (atribuicao == null || atribuicao.CreatedByUserId != userId)
            return NotFound(new { mensagem = "Atribuição não encontrada ou acesso negado." });

        var maquina = await _machineRepository.GetByIdAsync(dto.MachineId);
        var plano = await _planRepository.GetByIdAsync(dto.MaintenancePlanId);

        if (maquina == null || plano == null)
            return BadRequest(new { mensagem = "Máquina ou Plano de manutenção não encontrado." });

        atribuicao.MachineId = dto.MachineId;
        atribuicao.MaintenancePlanId = dto.MaintenancePlanId;
        atribuicao.NextDueDate = dto.NextDueDate;

        _assignmentRepository.Update(atribuicao);
        await _assignmentRepository.SaveChangesAsync();

        return Ok(new { mensagem = "Atribuição atualizada com sucesso." });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Excluir(int id)
    {
        var userId = ObterIdUsuarioAtual();
        var atribuicao = await _assignmentRepository.GetByIdAsync(id);

        if (atribuicao == null || atribuicao.CreatedByUserId != userId)
            return NotFound(new { mensagem = "Atribuição não encontrada ou acesso negado." });

        _assignmentRepository.Delete(atribuicao);
        await _assignmentRepository.SaveChangesAsync();

        return Ok(new { mensagem = "Atribuição excluída com sucesso." });
    }

    [HttpGet("mine")]
    public async Task<IActionResult> ObterMinhasAtribuicoes()
    {
        var userId = ObterIdUsuarioAtual();
        var atribuicoes = await _assignmentRepository.GetAssignmentsByCreatorAsync(userId);
        var resposta = atribuicoes.Select(MapearAtribuicaoParaResposta);

        return Ok(new { mensagem = "Suas atribuições carregadas com sucesso.", dados = resposta });
    }

    private CreatePlanAssignmentResponse MapearAtribuicaoParaResposta(MaintenancePlanAssignment a)
    {
        return new CreatePlanAssignmentResponse
        {
            Id = a.Id,
            MachineName = a.Machine?.Name ?? "N/A",
            PlanTitle = a.MaintenancePlan?.Title ?? "N/A",
            NextDueDate = a.NextDueDate,
            LastPerformed = a.LastPerformed
        };
    }
}
