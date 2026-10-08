using System.Globalization;
using System.Security.Claims;
using Ibatech.Domain.DTOs;
using Ibatech.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ibatech.API.Controllers;

[ApiController]
[Route("api/entradas-compras")]
[Authorize(Roles = "Admin,Estoque")]
public sealed class EntradasComprasController(
    IEntradaCompraService service,
    IEntradaCompraImportacaoService importacaoService)
    : ControllerBase
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

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<EntradaCompraDetalheDto>> Atualizar(
        Guid id,
        AtualizarEntradaCompraDto dto,
        CancellationToken ct) =>
        Ok(await service.AtualizarAsync(
            id,
            dto,
            ObterUsuarioId(),
            ct));

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

    [HttpPut("{id:guid}/itens/{itemId:guid}")]
    public async Task<ActionResult<EntradaCompraDetalheDto>> AtualizarItem(
        Guid id,
        Guid itemId,
        AtualizarEntradaCompraItemDto dto,
        CancellationToken ct) =>
        Ok(await service.AtualizarItemAsync(
            id,
            itemId,
            dto,
            ObterUsuarioId(),
            ct));

    [HttpDelete("{id:guid}/itens/{itemId:guid}")]
    public async Task<ActionResult<EntradaCompraDetalheDto>> RemoverItem(
        Guid id,
        Guid itemId,
        CancellationToken ct) =>
        Ok(await service.RemoverItemAsync(
            id,
            itemId,
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

    [HttpPost("importar")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<EntradaCompraImportacaoResultadoDto>> Importar(
        [FromForm] IFormFile arquivo,
        [FromForm] Guid fornecedorId,
        [FromForm] string numeroDocumento,
        [FromForm] DateTime dataEntrada,
        [FromForm] string valorFrete,
        [FromForm] string valorDesconto,
        [FromForm] string outrasDespesas,
        [FromForm] string? observacao,
        CancellationToken ct)
    {
        if (arquivo is null)
            throw new ArgumentException(
                "Arquivo é obrigatório.");

        var frete =
            ParseValorMonetario(
                valorFrete,
                nameof(valorFrete));

        var desconto =
            ParseValorMonetario(
                valorDesconto,
                nameof(valorDesconto));

        var despesas =
            ParseValorMonetario(
                outrasDespesas,
                nameof(outrasDespesas));

        await using var stream =
            arquivo.OpenReadStream();

        var resultado =
            await importacaoService.ImportarAsync(
                stream,
                arquivo.FileName,
                arquivo.Length,
                fornecedorId,
                numeroDocumento,
                dataEntrada,
                frete,
                desconto,
                despesas,
                observacao,
                ObterUsuarioId(),
                ct);

        if (!resultado.Sucesso)
            return BadRequest(resultado);

        return CreatedAtAction(
            nameof(Obter),
            new
            {
                id = resultado.EntradaCompraId
            },
            resultado);
    }

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

    private static decimal ParseValorMonetario(
        string? valor,
        string campo)
    {
        if (string.IsNullOrWhiteSpace(valor))
            return 0m;

        var texto = valor
            .Trim()
            .Replace("R$", "", StringComparison.OrdinalIgnoreCase)
            .Replace(" ", "");

        var posVirgula = texto.LastIndexOf(',');
        var posPonto = texto.LastIndexOf('.');

        string normalizado;

        if (posVirgula >= 0 && posPonto >= 0)
        {
            // 1.234,56 -> 1234.56
            // 1,234.56 -> 1234.56
            normalizado =
                posVirgula > posPonto
                    ? texto
                        .Replace(".", "")
                        .Replace(",", ".")
                    : texto.Replace(",", "");
        }
        else if (posVirgula >= 0)
        {
            // 11,00 -> 11.00
            normalizado =
                texto.Replace(",", ".");
        }
        else
        {
            // 11.00 ou 11
            normalizado = texto;
        }

        if (!decimal.TryParse(
                normalizado,
                NumberStyles.AllowDecimalPoint |
                NumberStyles.AllowLeadingSign,
                CultureInfo.InvariantCulture,
                out var resultado) ||
            resultado < 0)
        {
            throw new ArgumentException(
                $"Valor inválido para {campo}: '{valor}'.");
        }

        if (decimal.Round(resultado, 2) != resultado)
        {
            throw new ArgumentException(
                $"{campo} não pode ter mais de duas casas decimais.");
        }

        return resultado;
    }
}
