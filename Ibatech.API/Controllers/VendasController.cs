using System.Security.Claims;
using Ibatech.Domain.DTOs;
using Ibatech.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ibatech.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "AcessoVendas")]
public sealed class VendasController(IVendaService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VendaResumoDto>>> Listar([FromQuery] VendaFiltroDto filtros, CancellationToken ct) =>
        Ok(await service.ListarAsync(filtros, ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<VendaDetalheDto>> Obter(Guid id, CancellationToken ct) =>
        Ok(await service.ObterPorIdAsync(id, ct));

    [HttpPost]
    public async Task<ActionResult<VendaDetalheDto>> Criar(CriarVendaDto dto, CancellationToken ct)
    {
        var venda = await service.CriarAsync(dto, ObterUsuarioId(), ct);
        return CreatedAtAction(nameof(Obter), new { id = venda.Id }, venda);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<VendaDetalheDto>> Atualizar(Guid id, AtualizarVendaDto dto, CancellationToken ct) =>
        Ok(await service.AtualizarAsync(id, dto, ct));

    [HttpPost("{id:guid}/itens")]
    public async Task<ActionResult<VendaDetalheDto>> AdicionarItem(Guid id, AdicionarVendaItemDto dto, CancellationToken ct) =>
        Ok(await service.AdicionarItemAsync(id, dto, ct));

    [HttpPut("{id:guid}/itens/{itemId:guid}")]
    public async Task<ActionResult<VendaDetalheDto>> AtualizarItem(Guid id, Guid itemId, AtualizarVendaItemDto dto, CancellationToken ct) =>
        Ok(await service.AtualizarItemAsync(id, itemId, dto, ct));

    [HttpDelete("{id:guid}/itens/{itemId:guid}")]
    public async Task<ActionResult<VendaDetalheDto>> RemoverItem(Guid id, Guid itemId, CancellationToken ct) =>
        Ok(await service.RemoverItemAsync(id, itemId, ct));

    [HttpPost("{id:guid}/finalizar")]
    public async Task<ActionResult<VendaDetalheDto>> Finalizar(Guid id, FinalizarVendaDto dto, CancellationToken ct) =>
        Ok(await service.FinalizarAsync(id, dto, ObterUsuarioId(), ct));

    private Guid ObterUsuarioId()
    {
        var valor = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(valor, out var id))
            throw new UnauthorizedAccessException("Usuário autenticado inválido.");

        return id;
    }
}
