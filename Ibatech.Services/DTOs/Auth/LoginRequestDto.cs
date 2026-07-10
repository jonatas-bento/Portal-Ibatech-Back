namespace Ibatech.Services.DTOs.Auth;

public sealed record LoginRequestDto(
    string Email,
    string Senha
);
