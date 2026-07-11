namespace Ibatech.Domain.DTOs;

public record AtualizarUsuarioDto(
    string NomeCompleto,
    string Email,
    string? Telefone,
    string? Cpf,
    string Role);
