namespace Ibatech.Domain.DTOs;

public record UsuarioResumoDto(
    Guid Id,
    string NomeCompleto,
    string Email,
    string? Telefone,
    string? Cpf,
    string Role,
    bool Ativo,
    DateTime CriadoEm);
