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

    private int ObterIdUsuarioAtual()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
    }

    [HttpGet]
    public async Task<IActionResult> ObterTodas()
    {
        var manutenções = await _maintenanceRepository.GetAllAsync();
        var resposta = manutenções.Select(MapearManutencaoParaResposta);

        return Ok(new
        {
            mensagem = "Lista de manutenções carregada com sucesso.",
            dados = resposta
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var userId = ObterIdUsuarioAtual();
        var manutencao = await _maintenanceRepository.GetByIdAsync(id);

        if (manutencao == null || manutencao.CreatedByUserId != userId)
            return NotFound(new { mensagem = "Manutenção não encontrada ou acesso negado." });

        return Ok(new { mensagem = "Manutenção carregada com sucesso.", dados = MapearManutencaoParaResposta(manutencao) });
    }

    [HttpGet("machine/{machineId}")]
    public async Task<IActionResult> ObterPorMaquina(int machineId)
    {
        var userId = ObterIdUsuarioAtual();
        var manutenções = await _maintenanceRepository.GetByMachineIdAsync(machineId);

        var resposta = manutenções
            .Where(m => m.CreatedByUserId == userId)
            .Select(MapearManutencaoParaResposta);

        return Ok(new
        {
            mensagem = $"Manutenções da máquina {machineId} carregadas com sucesso.",
            dados = resposta
        });
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CreateMaintenanceRequest dto)
    {
        var userId = ObterIdUsuarioAtual();
        var maquina = await _machineRepository.GetByIdAsync(dto.MachineId);

        if (maquina == null)
            return BadRequest(new { mensagem = "Máquina não encontrada." });

        var manutencao = new Maintenance
        {
            MachineId = dto.MachineId,
            Type = (MaintenanceType)dto.Type,
            PerformedAt = dto.PerformedAt,
            DurationHours = (float)dto.DurationHours,
            Description = dto.Description,
            RootCause = dto.RootCause,
            CorrectiveAction = dto.CorrectiveAction,
            CreatedByUserId = userId
        };

        await _maintenanceRepository.AddAsync(manutencao);
        await _maintenanceRepository.SaveChangesAsync();

        return CreatedAtAction(nameof(ObterPorId), new { id = manutencao.Id },
        new { mensagem = "Manutenção registrada com sucesso.", dados = new { manutencao.Id } });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(int id, [FromBody] CreateMaintenanceRequest dto)
    {
        var userId = ObterIdUsuarioAtual();
        var manutencao = await _maintenanceRepository.GetByIdAsync(id);

        if (manutencao == null || manutencao.CreatedByUserId != userId)
            return NotFound(new { mensagem = "Manutenção não encontrada ou acesso negado." });

        manutencao.Type = (MaintenanceType)dto.Type;
        manutencao.PerformedAt = dto.PerformedAt;
        manutencao.DurationHours = (float)dto.DurationHours;
        manutencao.Description = dto.Description;
        manutencao.RootCause = dto.RootCause;
        manutencao.CorrectiveAction = dto.CorrectiveAction;

        _maintenanceRepository.Update(manutencao);
        await _maintenanceRepository.SaveChangesAsync();

        return Ok(new { mensagem = "Manutenção atualizada com sucesso." });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Excluir(int id)
    {
        var userId = ObterIdUsuarioAtual();
        var manutencao = await _maintenanceRepository.GetByIdAsync(id);

        if (manutencao == null || manutencao.CreatedByUserId != userId)
            return NotFound(new { mensagem = "Manutenção não encontrada ou acesso negado." });

        _maintenanceRepository.Delete(manutencao);
        await _maintenanceRepository.SaveChangesAsync();

        return Ok(new { mensagem = "Manutenção excluída com sucesso." });
    }

    [HttpGet("mine")]
    public async Task<IActionResult> ObterMinhasManutencoes()
    {
        var userId = ObterIdUsuarioAtual();
        var manutenções = await _maintenanceRepository.GetMaintenancesByCreatorAsync(userId);

        var resposta = manutenções.Select(MapearManutencaoParaResposta);

        return Ok(new
        {
            mensagem = "Suas manutenções carregadas com sucesso.",
            dados = resposta
        });
    }

    private CreateMaintenanceResponse MapearManutencaoParaResposta(Maintenance m)
    {
        return new CreateMaintenanceResponse
        {
            Id = m.Id,
            MachineName = m.Machine?.Name ?? "N/A",
            Type = m.Type.ToString(),
            PerformedAt = m.PerformedAt,
            DurationHours = m.DurationHours,
            Description = m.Description,
            RootCause = m.RootCause,
            CorrectiveAction = m.CorrectiveAction
        };
    }
}
