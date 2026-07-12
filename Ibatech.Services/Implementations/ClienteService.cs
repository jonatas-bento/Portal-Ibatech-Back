using Ibatech.Domain.Entities;
using Ibatech.Domain.DTOs;
using Ibatech.Domain.Interfaces.Repositories;
using Ibatech.Domain.Interfaces.Services;
using Ibatech.Services.Mappers;
using Ibatech.Repository.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Ibatech.Services.Implementations;

public sealed class ClienteService(
    IClienteRepository repository,
    IUnitOfWork uow) : IClienteService
{
    public async Task<IEnumerable<ClienteResumoDto>> ListarAsync(string? texto, bool? ativo)
    {
        var query = repository.ObterQueryable()
            .IgnoreQueryFilters();

        if (ativo.HasValue)
            query = query.Where(c => c.Ativo == ativo.Value);

        if (!string.IsNullOrWhiteSpace(texto))
        {
            texto = texto.Trim().ToLower();
            query = query.Where(c =>
                c.Nome.ToLower().Contains(texto) ||
                (c.CpfCnpj != null && c.CpfCnpj.Contains(texto)) ||
                (c.Email != null && c.Email.ToLower().Contains(texto)) ||
                (c.Telefone != null && c.Telefone.Contains(texto)));
        }

        var clientes = await query.ToListAsync();
        return clientes.ToResumoDtoList();
    }

    public async Task<ClienteDetalheDto?> ObterPorIdAsync(Guid id)
    {
        var cliente = await repository.ObterPorIdAsync(id);
        return cliente?.ToDetalheDto();
    }

    public async Task<ClienteResumoDto> CriarAsync(CriarClienteDto dto)
    {
        var cpfCnpj = string.IsNullOrWhiteSpace(dto.CpfCnpj) 
            ? null 
            : new string(dto.CpfCnpj.Where(char.IsDigit).ToArray());

        if (!string.IsNullOrEmpty(cpfCnpj) && cpfCnpj.Length != 11 && cpfCnpj.Length != 14)
            throw new ArgumentException("CPF/CNPJ inválido.");

        var cliente = new Cliente(dto.Nome.Trim(), cpfCnpj, dto.Telefone, dto.Email?.Trim().ToLowerInvariant(), dto.Endereco, dto.Observacao);
        await repository.AdicionarAsync(cliente);
        await uow.CommitAsync();
        return cliente.ToResumoDto();
    }

    public async Task<bool> AtualizarAsync(Guid id, AtualizarClienteDto dto)
    {
        var cliente = await repository.ObterPorIdAsync(id);
        if (cliente is null) return false;

        cliente.Atualizar(dto.Nome, dto.CpfCnpj, dto.Telefone, dto.Email, dto.Endereco, dto.Observacao);
        repository.Atualizar(cliente);
        await uow.CommitAsync();
        return true;
    }

    public async Task<bool> AlterarStatusAsync(Guid id, bool ativo)
    {
        var cliente = await repository.ObterQueryable()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.Id == id);

        if (cliente is null) return false;

        if (ativo) cliente.Ativar();
        else cliente.Desativar();

        repository.Atualizar(cliente);
        await uow.CommitAsync();
        return true;
    }
}
