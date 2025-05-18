using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MaintenanceControlSystem.Application.DTOs;
using MaintenanceControlSystem.Infrastructure.Repositories.Interfaces;
using MaintenanceControlSystem.Domain.Entities;
using System.Security.Claims;

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

    private int ObterIdUsuarioAtual()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    }

    [HttpPost]
    public async Task<IActionResult> CriarAlerta([FromBody] CreateAlertRequest dto)
    {
        var userId = ObterIdUsuarioAtual();
        var planos = await _assignmentRepository.GetByMachineIdAsync(dto.MachineId);
        var plano = planos.FirstOrDefault();

        if (plano == null)
        {
            return BadRequest(new { mensagem = $"Nenhum plano de manutenção encontrado para a máquina {dto.MachineId}." });
        }

        var alerta = new Alert
        {
            Message = dto.Message,
            IsRead = false,
            MachineId = dto.MachineId,
            AssignmentId = plano.Id,
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = userId
        };

        await _alertRepository.AddAsync(alerta);
        await _alertRepository.SaveChangesAsync();

        var resposta = MapearAlertaParaResposta(alerta);
        return CreatedAtAction(nameof(ObterPorId), new { id = alerta.Id }, new { mensagem = "Alerta criado com sucesso.", dados = resposta });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var userId = ObterIdUsuarioAtual();
        var alerta = await _alertRepository.GetByIdAsync(id);

        if (alerta == null || alerta.CreatedByUserId != userId)
            return NotFound(new { mensagem = "Alerta não encontrado ou acesso negado." });

        return Ok(new { mensagem = "Alerta carregado com sucesso.", dados = MapearAlertaParaResposta(alerta) });
    }

    [HttpGet("machine/{machineId}")]
    public async Task<IActionResult> ObterPorMaquina(int machineId)
    {
        var userId = ObterIdUsuarioAtual();
        var alertas = await _alertRepository.GetByMachineAsync(machineId);

        var resposta = alertas
            .Where(a => a.CreatedByUserId == userId)
            .Select(MapearAlertaParaResposta);

        return Ok(new { mensagem = $"Alertas da máquina {machineId} carregados com sucesso.", dados = resposta });
    }

    [HttpGet("unread")]
    public async Task<IActionResult> ObterNaoLidos()
    {
        var userId = ObterIdUsuarioAtual();
        var alertas = await _alertRepository.GetUnreadAsync();
        var resposta = alertas
            .Where(a => a.CreatedByUserId == userId)
            .Select(MapearAlertaParaResposta);

        return Ok(new { mensagem = "Alertas não lidos carregados com sucesso.", dados = resposta });
    }

    [HttpPut("{id}/read")]
    public async Task<IActionResult> MarcarComoLido(int id)
    {
        var userId = ObterIdUsuarioAtual();
        var alerta = await _alertRepository.GetByIdAsync(id);

        if (alerta == null || alerta.CreatedByUserId != userId)
            return NotFound(new { mensagem = "Alerta não encontrado ou acesso negado." });

        alerta.IsRead = true;
        _alertRepository.Update(alerta);
        await _alertRepository.SaveChangesAsync();

        return Ok(new { mensagem = "Alerta marcado como lido com sucesso." });
    }

    [HttpGet("mine")]
    public async Task<IActionResult> ObterMeusAlertas()
    {
        var userId = ObterIdUsuarioAtual();
        var alertas = await _alertRepository.GetAlertsByCreatorAsync(userId);
        var resposta = alertas.Select(MapearAlertaParaResposta);

        return Ok(new { mensagem = "Seus alertas carregados com sucesso.", dados = resposta });
    }

    [HttpGet]
    public async Task<IActionResult> ObterTodos()
    {
        var alertas = await _alertRepository.GetAllAsync();
        var resposta = alertas.Select(MapearAlertaParaResposta);

        return Ok(new { mensagem = "Todos os alertas carregados com sucesso.", dados = resposta });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(int id, [FromBody] CreateAlertRequest dto)
    {
        var userId = ObterIdUsuarioAtual();
        var alerta = await _alertRepository.GetByIdAsync(id);

        if (alerta == null || alerta.CreatedByUserId != userId)
            return NotFound(new { mensagem = "Alerta não encontrado ou acesso negado." });

        alerta.Message = dto.Message;
        alerta.MachineId = dto.MachineId;
        alerta.IsRead = dto.IsRead;

        _alertRepository.Update(alerta);
        await _alertRepository.SaveChangesAsync();

        return Ok(new { mensagem = "Alerta atualizado com sucesso." });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Excluir(int id)
    {
        var userId = ObterIdUsuarioAtual();
        var alerta = await _alertRepository.GetByIdAsync(id);

        if (alerta == null || alerta.CreatedByUserId != userId)
            return NotFound(new { mensagem = "Alerta não encontrado ou acesso negado." });

        _alertRepository.Delete(alerta);
        await _alertRepository.SaveChangesAsync();

        return Ok(new { mensagem = "Alerta excluído com sucesso." });
    }

    private CreateAlertResponse MapearAlertaParaResposta(Alert alerta)
    {
        return new CreateAlertResponse
        {
            Id = alerta.Id,
            Message = alerta.Message,
            IsRead = alerta.IsRead,
            MachineId = alerta.MachineId,
            CreatedAt = alerta.CreatedAt
        };
    }
}
