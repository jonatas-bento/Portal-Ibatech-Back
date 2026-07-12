namespace Ibatech.Domain.DTOs;

public record CriarClienteDto(
    string Nome,
    string? CpfCnpj,
    string? Telefone,
    string? Email,
    string? Endereco,
    string? Observacao);

public record AtualizarClienteDto(
    string Nome,
    string? CpfCnpj,
    string? Telefone,
    string? Email,
    string? Endereco,
    string? Observacao);

public record AlterarStatusClienteDto(bool Ativo);

public record ClienteResumoDto(
    Guid Id,
    string Nome,
    string? CpfCnpj,
    string? Email,
    bool Ativo);
