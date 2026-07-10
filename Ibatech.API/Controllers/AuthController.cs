// Ibatech.API/Controllers/AuthController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Ibatech.Domain.Interfaces.Services;
using Ibatech.Services.DTOs;
using Ibatech.Domain.DTOs;// ← correto: DTOs estão em Services

namespace Ibatech.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponseDto>> Login(
        [FromBody] LoginRequestDto dto,
        CancellationToken ct)
    {
        try
        {
            var response = await authService.LoginAsync(dto, ct);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { title = "Acesso Negado", detail = ex.Message });
        }
    }
}