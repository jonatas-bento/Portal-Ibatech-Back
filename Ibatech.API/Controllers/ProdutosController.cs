using Microsoft.AspNetCore.Mvc;
using Ibatech.Domain.Interfaces.Services;
using Ibatech.Domain.DTOs;
using Ibatech.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Ibatech.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public sealed class ProdutosController(IProdutoService produtoService) : ControllerBase
    {
        [HttpGet]
        [Authorize(Policy = "Autenticado")]
        public async Task<ActionResult<IEnumerable<ProdutoResponseDto>>> ListarTodos(CancellationToken ct)
        {
            var produtos = await produtoService.ListarAsync(ct);
            return Ok(produtos);
        }

        [HttpGet("alertas-reposicao")]
        [Authorize(Policy = "AdminOuFuncionario")]
        public async Task<ActionResult<IEnumerable<ProdutoResponseDto>>> ListarAlertas(CancellationToken ct)
        {
            var produtosAlertas = await produtoService.ListarAlertasReposicaoAsync(ct);
            return Ok(produtosAlertas);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOuFuncionario")]
        public async Task<ActionResult<ProdutoResponseDto>> Criar(
            [FromBody] ProdutoCreateDto dto,
            CancellationToken ct)
        {
            try
            {
                var novoProduto = await produtoService.CriarAsync(dto, ct);

                // Retorna o status 201 Created apontando para a listagem geral ou detalhe futuro
                return CreatedAtAction(nameof(ListarTodos), null, novoProduto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { detail = ex.Message });
            }
        }

        [HttpPost("{id:guid}/movimentar")]
        [Authorize(Policy = "AdminOuFuncionario")]
        public async Task<IActionResult> RegistrarMovimentacao(
            Guid id,
            [FromBody] RegistrarMovimentacaoRequestDto dto,
            CancellationToken ct)
        {
            // Extrai o ID do usuário/funcionário logado que disparou a movimentação do token JWT
            var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(usuarioIdClaim, out var usuarioId))
                return Unauthorized(new { detail = "Identificação do usuário inválida no token." });

            if (!Enum.TryParse<TipoMovimentacao>(dto.Tipo, true, out var tipoEnum))
                return BadRequest(new { detail = $"Tipo de movimentação '{dto.Tipo}' é inválido. Use 'Entrada' ou 'Saida'." });

            try
            {
                await produtoService.RegistrarMovimentacaoAsync(
                    id,
                    tipoEnum,
                    dto.Quantidade,
                    usuarioId,
                    dto.Motivo,
                    ct
                );

                return Ok(new { message = "Movimentação de estoque consolidada com sucesso." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { detail = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { detail = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                // Captura falhas de regra de negócio (ex: Saída maior que a quantidade atual)
                return BadRequest(new { detail = ex.Message });
            }
        }
    }
}