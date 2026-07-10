namespace Ibatech.Services.DTOs.Auth;

public sealed record LoginResponseDto(
    string Token,
    string Nome,
    string Email,
    string Role,
    DateTime ExpiraEm
);