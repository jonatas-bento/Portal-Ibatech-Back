using Ibatech.Domain.DTOs;
using Ibatech.Domain.Entities;
using Ibatech.Domain.Interfaces.Repositories;
using Ibatech.Domain.Interfaces.Services;
using Ibatech.Repository.UnitOfWork;
using Ibatech.Services.Mappers;

namespace Ibatech.Services.Implementations;

public sealed class FornecedorService(
    IFornecedorRepository fornecedorRepository,
    IUnitOfWork uow) : IFornecedorService
{
    public async Task<IReadOnlyCollection<FornecedorDto>> ListarAsync(
        CancellationToken ct = default)
    {
        var fornecedores =
            await fornecedorRepository.ListarAsync(ct);

        return fornecedores
            .Select(FornecedorMapper.ToDto)
            .ToList();
    }

    public async Task<FornecedorDto> ObterPorIdAsync(
        Guid id,
        CancellationToken ct = default)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("ID do fornecedor inválido.");

        var fornecedor =
            await fornecedorRepository.ObterPorIdAsync(id, ct);

        if (fornecedor is null)
            throw new KeyNotFoundException(
                "Fornecedor não encontrado.");

        return FornecedorMapper.ToDto(fornecedor);
    }

    public async Task<FornecedorDto> CriarAsync(
        CriarFornecedorDto dto,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var fornecedor = new Fornecedor(
            dto.Nome,
            dto.NomeFantasia,
            dto.Documento,
            dto.Email,
            dto.Telefone,
            dto.Observacao);

        await fornecedorRepository.AdicionarAsync(
            fornecedor,
            ct);

        await uow.CommitAsync(ct);

        return FornecedorMapper.ToDto(fornecedor);
    }

    public async Task<FornecedorDto> AtualizarAsync(
        Guid id,
        AtualizarFornecedorDto dto,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        if (id == Guid.Empty)
            throw new ArgumentException(
                "ID do fornecedor inválido.");

        var fornecedor =
            await fornecedorRepository.ObterPorIdAsync(id, ct);

        if (fornecedor is null)
            throw new KeyNotFoundException(
                "Fornecedor não encontrado.");

        fornecedor.Atualizar(
            dto.Nome,
            dto.NomeFantasia,
            dto.Documento,
            dto.Email,
            dto.Telefone,
            dto.Observacao);

        await uow.CommitAsync(ct);

        return FornecedorMapper.ToDto(fornecedor);
    }
}
