using Ibatech.Domain.DTOs;
using Ibatech.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ibatech.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "AcessoVendas")]
public sealed class ClientesController(IClienteService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClienteResumoDto>>> Listar([FromQuery] string? texto, [FromQuery] bool? ativo) =>
        Ok(await service.ListarAsync(texto, ativo));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ClienteDetalheDto>> Obter(Guid id)
    {
        var cliente = await service.ObterPorIdAsync(id);
        return cliente is null ? NotFound() : Ok(cliente);
    }

    [HttpPost]
    public async Task<ActionResult<ClienteResumoDto>> Criar(CriarClienteDto dto)
    {
        var cliente = await service.CriarAsync(dto);
        return CreatedAtAction(nameof(Obter), new { id = cliente.Id }, cliente);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(Guid id, AtualizarClienteDto dto) =>
        await service.AtualizarAsync(id, dto) ? NoContent() : NotFound();

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> AlterarStatus(Guid id, AlterarStatusClienteDto dto) =>
        await service.AlterarStatusAsync(id, dto.Ativo) ? NoContent() : NotFound();
}
