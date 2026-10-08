namespace Ibatech.Domain.DTOs;

public sealed record CriarFornecedorDto(
    string Nome,
    string? NomeFantasia,
    string? Documento,
    string? Email,
    string? Telefone,
    string? Observacao);

public sealed record AtualizarFornecedorDto(
    string Nome,
    string? NomeFantasia,
    string? Documento,
    string? Email,
    string? Telefone,
    string? Observacao);

public sealed record FornecedorDto(
    Guid Id,
    string Nome,
    string? NomeFantasia,
    string? Documento,
    string? Email,
    string? Telefone,
    string? Observacao,
    bool Ativo);
