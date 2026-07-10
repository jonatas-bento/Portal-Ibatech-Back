// src/Ibatech.API/Controllers/FinanceiroController.cs
using Microsoft.AspNetCore.Mvc;
using Ibatech.Domain.Interfaces.Services;
using Ibatech.Domain.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace Ibatech.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "AdminOuFuncionario")] // Trava gerencial de segurança
    public sealed class FinanceiroController(IFinanceiroService financeiroService) : ControllerBase
    {
        [HttpGet("resumo")]
        public async Task<ActionResult<ResumoFinanceiroDto>> ObterResumo(CancellationToken ct)
        {
            var resumo = await financeiroService.ObterResumoAsync(ct);
            return Ok(resumo);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransacaoResponseDto>>> ListarTodas(CancellationToken ct)
        {
            var transacoes = await financeiroService.ListarAsync(ct);
            return Ok(transacoes);
        }

        [HttpPost]
        public async Task<ActionResult<TransacaoResponseDto>> Criar(
            [FromBody] TransacaoCreateDto dto,
            CancellationToken ct)
        {
            try
            {
                var novaTransacao = await financeiroService.CriarAsync(dto, ct);

                // Aponta para o método ListarTodas como referência de criação
                return CreatedAtAction(nameof(ListarTodas), null, novaTransacao);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { detail = ex.Message });
            }
        }

        //[HttpPost("{id:guid}/liquidar")]
        //public async Task<IActionResult> Liquidar(Guid id, CancellationToken ct)
        //{
        //    try
        //    {
        //        await financeiroService.LiquidarAsync(id, ct);
        //        return Ok(new { message = "Transação liquidada e baixada com sucesso no fluxo de caixa." });
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        return NotFound(new { detail = ex.Message });
        //    }
        //}
    }
}