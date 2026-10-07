using System.Security.Claims;
using Ibatech.Domain.DTOs;
using Ibatech.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ibatech.API.Controllers;

[ApiController]
[Route("api/entradas-compras")]
[Authorize]
public sealed class EntradasComprasController(
    IEntradaCompraService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EntradaCompraResumoDto>>> Listar(
        CancellationToken ct) =>
        Ok(await service.ListarAsync(ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EntradaCompraDetalheDto>> Obter(
        Guid id,
        CancellationToken ct) =>
        Ok(await service.ObterPorIdAsync(id, ct));

    [HttpPost]
    public async Task<ActionResult<EntradaCompraDetalheDto>> Criar(
        CriarEntradaCompraDto dto,
        CancellationToken ct)
    {
        var entrada =
            await service.CriarAsync(
                dto,
                ObterUsuarioId(),
                ct);

        return CreatedAtAction(
            nameof(Obter),
            new { id = entrada.Id },
            entrada);
    }

    [HttpPost("{id:guid}/itens")]
    public async Task<ActionResult<EntradaCompraDetalheDto>> AdicionarItem(
        Guid id,
        AdicionarEntradaCompraItemDto dto,
        CancellationToken ct) =>
        Ok(await service.AdicionarItemAsync(
            id,
            dto,
            ObterUsuarioId(),
            ct));

    [HttpPost("{id:guid}/confirmar")]
    public async Task<ActionResult<EntradaCompraDetalheDto>> Confirmar(
        Guid id,
        CancellationToken ct) =>
        Ok(await service.ConfirmarAsync(
            id,
            ObterUsuarioId(),
            ct));

    private Guid ObterUsuarioId()
    {
        var valor =
            User.FindFirstValue(
                ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(valor, out var id))
            throw new UnauthorizedAccessException(
                "Usuário autenticado inválido.");

        return id;
    }
}
