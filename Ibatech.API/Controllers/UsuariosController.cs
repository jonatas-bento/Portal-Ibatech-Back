// Ibatech.API/Controllers/UsuariosController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Ibatech.Domain.Interfaces.Services;
using Ibatech.Domain.DTOs; // ← mesmo namespace

namespace Ibatech.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class UsuariosController(IUsuarioService usuarioService) : ControllerBase
{
    [HttpPost("cadastrar")]
    [AllowAnonymous]
    public async Task<ActionResult<UsuarioResponseDto>> Cadastrar(
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

    [HttpGet]
    [Authorize(Policy = "AdminOuFuncionario")]
    public async Task<ActionResult<IEnumerable<UsuarioResponseDto>>> Listar(
        CancellationToken ct)
    {
        var usuarios = await usuarioService.ListarAsync(ct);
        return Ok(usuarios);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Autenticado")]
    public async Task<ActionResult<UsuarioResponseDto>> ObterPorId(
        Guid id, CancellationToken ct)
    {
        var usuario = await usuarioService.ObterPorIdAsync(id, ct);
        return usuario is null ? NotFound() : Ok(usuario);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "ApenasAdmin")]
    public async Task<IActionResult> Desativar(Guid id, CancellationToken ct)
    {
        try
        {
            await usuarioService.DesativarAsync(id, ct);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}