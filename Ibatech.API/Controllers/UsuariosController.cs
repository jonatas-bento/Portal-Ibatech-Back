// Ibatech.API/Controllers/UsuariosController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Ibatech.Domain.Interfaces.Services;
using Ibatech.Domain.DTOs;
using Ibatech.Domain.Enums;
using System.Security.Claims;

namespace Ibatech.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class UsuariosController(IUsuarioService usuarioService) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = "ApenasAdmin")]
    public async Task<ActionResult<IEnumerable<UsuarioResumoDto>>> Listar(
        [FromQuery] string? nome,
        [FromQuery] string? email,
        [FromQuery] RoleUsuario? role,
        [FromQuery] bool? ativo,
        CancellationToken ct)
    {
        var usuarios = await usuarioService.ListarAsync(nome, email, role, ativo, ct);
        return Ok(usuarios);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "ApenasAdmin")]
    public async Task<ActionResult<UsuarioResponseDto>> ObterPorId(
        Guid id, CancellationToken ct)
    {
        var usuario = await usuarioService.ObterPorIdAsync(id, ct);
        return usuario is null ? NotFound() : Ok(usuario);
    }

    [HttpPost]
    [Authorize(Policy = "ApenasAdmin")]
    public async Task<ActionResult<UsuarioResponseDto>> Criar(
        [FromBody] UsuarioCreateDto dto,
        CancellationToken ct)
    {
        try
        {
            var usuario = await usuarioService.CriarAsync(dto, ct);
            return CreatedAtAction(nameof(ObterPorId), new { id = usuario.Id }, usuario);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { title = "Conflito", detail = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "ApenasAdmin")]
    public async Task<IActionResult> Atualizar(
        Guid id,
        [FromBody] AtualizarUsuarioDto dto,
        CancellationToken ct)
    {
        try
        {
            await usuarioService.AtualizarAsync(id, dto, ct);
            return NoContent();
        }
        catch (KeyNotFoundException) { return NotFound(); }
        catch (InvalidOperationException ex)
        {
            if (ex.Message == "Senha atual incorreta.")
                return BadRequest(new { detail = ex.Message });
            return Conflict(new { detail = ex.Message });
        }
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Policy = "ApenasAdmin")]
    public async Task<IActionResult> AlterarStatus(
        Guid id,
        [FromBody] AlterarStatusUsuarioDto dto,
        CancellationToken ct)
    {
        var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (adminId == id.ToString())
            return BadRequest(new { detail = "Não é possível desativar a própria conta." });

        try
        {
            await usuarioService.AlterarStatusAsync(id, dto.Ativo, ct);
            return NoContent();
        }
        catch (KeyNotFoundException) { return NotFound(); }
        catch (InvalidOperationException ex) { return Conflict(new { detail = ex.Message }); }
    }

    [HttpPut("{id:guid}/senha")]
    [Authorize(Policy = "ApenasAdmin")]
    public async Task<IActionResult> RedefinirSenha(
        Guid id,
        [FromBody] RedefinirSenhaUsuarioDto dto,
        CancellationToken ct)
    {
        try
        {
            await usuarioService.RedefinirSenhaAsync(id, dto.NovaSenha, ct);
            return NoContent();
        }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [HttpPut("minha-senha")]
    [Authorize]
    public async Task<IActionResult> AlterarMinhaSenha(
        [FromBody] AlterarMinhaSenhaDto dto,
        CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        try
        {
            await usuarioService.AlterarMinhaSenhaAsync(Guid.Parse(userId), dto.SenhaAtual, dto.NovaSenha, ct);
            return NoContent();
        }
        catch (KeyNotFoundException) { return NotFound(); }
        catch (InvalidOperationException ex)
        {
            if (ex.Message == "Senha atual incorreta.")
                return BadRequest(new { detail = ex.Message });
            return Conflict(new { detail = ex.Message });
        }
    }
}
