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
public class MachineController : ControllerBase
{
    private readonly IMachineRepository _machineRepository;

    public MachineController(IMachineRepository machineRepository)
    {
        _machineRepository = machineRepository;
    }

    private int ObterIdUsuarioAtual()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CreateMachineRequest dto)
    {
        var maquinaExistente = await _machineRepository.GetByCodeAsync(dto.Code);
        if (maquinaExistente != null)
        {
            return BadRequest(new { mensagem = $"Já existe uma máquina com o código '{dto.Code}'." });
        }

        var userId = ObterIdUsuarioAtual();

        var maquina = new Machine
        {
            Name = dto.Name,
            Code = dto.Code,
            Location = dto.Location,
            InstallationDate = dto.InstallationDate,
            Status = (MachineStatus)dto.Status,
            Manufacturer = dto.Manufacturer,
            CreatedByUserId = userId
        };

        await _machineRepository.AddAsync(maquina);
        await _machineRepository.SaveChangesAsync();

        var resposta = MapearMaquinaParaResposta(maquina);

        return CreatedAtAction(nameof(ObterPorId), new { id = maquina.Id }, new
        {
            mensagem = "Máquina criada com sucesso.",
            dados = resposta
        });
    }

    [HttpGet]
    public async Task<IActionResult> ObterTodas()
    {
        var maquinas = await _machineRepository.GetAllAsync();
        var resposta = maquinas.Select(MapearMaquinaParaResposta);

        return Ok(new
        {
            mensagem = "Lista de máquinas carregada com sucesso.",
            dados = resposta
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var userId = ObterIdUsuarioAtual();
        var maquina = await _machineRepository.GetByIdAsync(id);

        if (maquina == null || maquina.CreatedByUserId != userId)
        {
            return NotFound(new { mensagem = "Máquina não encontrada ou acesso negado." });
        }

        var resposta = MapearMaquinaParaResposta(maquina);
        return Ok(new { mensagem = "Máquina carregada com sucesso.", dados = resposta });
    }

    [HttpGet("mine")]
    public async Task<IActionResult> ObterMinhasMaquinas()
    {
        var userId = ObterIdUsuarioAtual();
        var maquinas = await _machineRepository.GetMachinesByCreatorAsync(userId);

        var resposta = maquinas.Select(MapearMaquinaParaResposta);
        return Ok(new
        {
            mensagem = "Máquinas criadas por você carregadas com sucesso.",
            dados = resposta
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(int id, [FromBody] CreateMachineRequest dto)
    {
        var userId = ObterIdUsuarioAtual();
        var maquina = await _machineRepository.GetByIdAsync(id);

        if (maquina == null || maquina.CreatedByUserId != userId)
        {
            return NotFound(new { mensagem = "Máquina não encontrada ou acesso negado." });
        }

        maquina.Name = dto.Name;
        maquina.Location = dto.Location;
        maquina.Status = (MachineStatus)dto.Status;
        maquina.Manufacturer = dto.Manufacturer;
        maquina.InstallationDate = dto.InstallationDate;

        _machineRepository.Update(maquina);
        await _machineRepository.SaveChangesAsync();

        return Ok(new { mensagem = "Máquina atualizada com sucesso." });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Excluir(int id)
    {
        var userId = ObterIdUsuarioAtual();
        var maquina = await _machineRepository.GetByIdAsync(id);

        if (maquina == null || maquina.CreatedByUserId != userId)
        {
            return NotFound(new { mensagem = "Máquina não encontrada ou acesso negado." });
        }

        _machineRepository.Delete(maquina);
        await _machineRepository.SaveChangesAsync();

        return Ok(new { mensagem = "Máquina excluída com sucesso." });
    }

    private CreateMachineResponse MapearMaquinaParaResposta(Machine maquina)
    {
        return new CreateMachineResponse
        {
            Id = maquina.Id,
            Name = maquina.Name,
            Code = maquina.Code,
            Location = maquina.Location,
            InstallationDate = maquina.InstallationDate,
            Status = maquina.Status.ToString(),
            Manufacturer = maquina.Manufacturer
        };
    }
}
