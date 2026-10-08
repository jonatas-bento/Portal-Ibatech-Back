using Ibatech.Domain.DTOs;
using Ibatech.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ibatech.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Estoque")]
public sealed class FornecedoresController(
    IFornecedorService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<FornecedorDto>>> Listar(
        CancellationToken ct) =>
        Ok(await service.ListarAsync(ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<FornecedorDto>> Obter(
        Guid id,
        CancellationToken ct) =>
        Ok(await service.ObterPorIdAsync(id, ct));

    [HttpPost]
    public async Task<ActionResult<FornecedorDto>> Criar(
        CriarFornecedorDto dto,
        CancellationToken ct)
    {
        var fornecedor =
            await service.CriarAsync(dto, ct);

        return CreatedAtAction(
            nameof(Obter),
            new { id = fornecedor.Id },
            fornecedor);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<FornecedorDto>> Atualizar(
        Guid id,
        AtualizarFornecedorDto dto,
        CancellationToken ct) =>
        Ok(await service.AtualizarAsync(
            id,
            dto,
            ct));
}
