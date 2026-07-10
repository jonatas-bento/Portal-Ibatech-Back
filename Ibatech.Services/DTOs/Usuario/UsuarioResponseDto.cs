using Ibatech.Domain.Enums;
namespace Ibatech.Services.DTOs.Usuario;

public sealed record UsuarioResponseDto(
    Guid Id,
    string NomeCompleto,
    string Email,
    string? Telefone,
    string? Cpf,
    RoleUsuario Role,
    bool Ativo,
    DateTime CriadoEm
);