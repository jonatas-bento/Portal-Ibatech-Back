// Ibatech.Services/Implementations/AuthService.cs
using Ibatech.Domain.DTOs;
using Ibatech.Domain.Interfaces.Repositories;
using Ibatech.Domain.Interfaces.Services;
using Ibatech.Services.Security;

namespace Ibatech.Services.Implementations;

public sealed class AuthService(
    IUsuarioRepository usuarioRepo,
    JwtTokenGenerator jwtGenerator) : IAuthService
{
    public async Task<LoginResponseDto> LoginAsync(
        LoginRequestDto dto,
        CancellationToken ct = default)
    {
        var usuario = await usuarioRepo.ObterPorEmailAsync(dto.Email, ct)
            ?? throw new UnauthorizedAccessException("Credenciais inválidas.");

        if (!usuario.Ativo)
            throw new UnauthorizedAccessException("Usuário inativo.");

        if (!PasswordHasher.Verify(dto.Senha, usuario.SenhaHash))
            throw new UnauthorizedAccessException("Credenciais inválidas.");

        var (token, expiraEm) = jwtGenerator.Gerar(usuario);

        return new LoginResponseDto
        {
            Token = token,
            Nome = usuario.NomeCompleto,
            Email = usuario.Email,
            Role = usuario.Role.ToString(),
            ExpiraEm = expiraEm
        };
    }
}
