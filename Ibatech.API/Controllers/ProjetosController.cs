using Microsoft.AspNetCore.Mvc;
using Ibatech.Domain.Interfaces.Services;
using Ibatech.Domain.DTOs; // CORREÇÃO: Puxa todos os DTOs (ProjetoCreateDto, AtualizarStatusDto, etc.) do Domínio
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Ibatech.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public sealed class ProjetosController(IProjetoService projetoService) : ControllerBase
    {
        [HttpPost]
        [Authorize(Policy = "Autenticado")]
        public async Task<ActionResult<ProjetoResponseDto>> Criar(
            [FromBody] ProjetoCreateDto dto,
            CancellationToken ct)
        {
            var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(usuarioIdClaim, out var usuarioId))
                return Unauthorized(new { detail = "Identificação do usuário inválida no token." });

            var projeto = await projetoService.CriarAsync(usuarioId, dto, ct);
            return CreatedAtAction(nameof(ListarPorUsuario), null, projeto);
        }

        [HttpGet("meus")]
        [Authorize(Policy = "Autenticado")]
        public async Task<ActionResult<IEnumerable<ProjetoResponseDto>>> ListarPorUsuario(CancellationToken ct)
        {
            var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(usuarioIdClaim, out var usuarioId))
                return Unauthorized(new { detail = "Identificação do usuário inválida no token." });

            var projetos = await projetoService.ListarPorUsuarioAsync(usuarioId, ct);
            return Ok(projetos);
        }

        [HttpGet]
        [Authorize(Policy = "AdminOuFuncionario")]
        public async Task<ActionResult<IEnumerable<ProjetoResponseDto>>> ListarTodos(CancellationToken ct)
        {
            var projetos = await projetoService.ListarAsync(ct);
            return Ok(projetos);
        }

        [HttpPatch("{id}/status")]
        [Authorize(Policy = "AdminOuFuncionario")]
        public async Task<ActionResult<ProjetoResponseDto>> AtualizarStatus(
            Guid id,
            [FromBody] AtualizarStatusDto dto, // Agora mapeado corretamente para o Domínio
            CancellationToken ct)
        {
            try
            {
                var projetoAtualizado = await projetoService.AtualizarStatusAsync(id, dto, ct);
                return Ok(projetoAtualizado);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { detail = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { detail = ex.Message });
            }
        }

        [HttpGet("{id:guid}")]
        [Authorize(Policy = "Autenticado")]
        public async Task<ActionResult<ProjetoResponseDto>> ObterPorId(
            Guid id,
            CancellationToken ct)
        {
            // Opcional: Se o seu serviço já faz a validação de quem pode ou não ver,
            // basta chamar o método diretamente.
            var projeto = await projetoService.ObterPorIdAsync(id, ct);

            if (projeto == null)
                return NotFound(new { detail = $"Projeto com ID {id} não foi encontrado." });

            return Ok(projeto);
        }
    }
}