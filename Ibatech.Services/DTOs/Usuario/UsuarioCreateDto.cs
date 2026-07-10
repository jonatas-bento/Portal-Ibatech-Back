using Ibatech.Domain.Enums;
namespace Ibatech.Services.DTOs.Usuario;

public sealed record UsuarioCreateDto(
    string NomeCompleto,
    string Email,
    string Senha,
    RoleUsuario Role,
    string? Telefone = null,
    string? Cpf = null
);