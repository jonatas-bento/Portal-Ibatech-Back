// Ibatech.Services/Implementations/ProdutoService.cs
using Ibatech.Domain.DTOs;
using Ibatech.Domain.Entities;
using Ibatech.Domain.Enums;
using Ibatech.Domain.Interfaces.Repositories;
using Ibatech.Domain.Interfaces.Services;
using Ibatech.Repository.UnitOfWork;
using Ibatech.Services.Mappers;

namespace Ibatech.Services.Implementations;

public sealed class ProdutoService(
    IProdutoRepository produtoRepo,
    IEstoqueRepository estoqueRepo,
    IUnitOfWork uow) : IProdutoService
{
    public async Task<ProdutoResponseDto> CriarAsync(
        ProdutoCreateDto dto,
        CancellationToken ct = default)
    {
        var tipo = Enum.Parse<TipoProduto>(dto.Tipo);
        var produto = new Produto(
            dto.Nome,
            tipo,
            dto.PrecoCompra,
            dto.PrecoVenda,
            dto.Descricao,
            dto.CodigoSku,
            dto.Marca,
            dto.Modelo);

        await produtoRepo.AdicionarAsync(produto, ct);
        await uow.CommitAsync(ct); // persiste para obter o Id

        var estoque = new Estoque(
            produto.Id,
            dto.QuantidadeInicial,
            dto.QuantidadeMinima);

        await estoqueRepo.AdicionarAsync(estoque, ct);
        await uow.CommitAsync(ct);

        // Recarrega com navegação
        var produtoCompleto = await produtoRepo.ObterComEstoqueAsync(produto.Id, ct);
        return produtoCompleto!.ToDomainDto();
    }

    public async Task<IEnumerable<ProdutoResponseDto>> ListarAsync(
        CancellationToken ct = default)
    {
        var produtos = await produtoRepo.ListarComEstoqueAsync(ct);
        return produtos.ToDomainDtoList();
    }

    public async Task<IEnumerable<ProdutoResponseDto>> ListarAlertasReposicaoAsync(
        CancellationToken ct = default)
    {
        var produtos = await produtoRepo.ListarComEstoqueAsync(ct);
        return produtos
            .Where(p => p.Estoque?.EstaBaixoDoMinimo == true)
            .ToDomainDtoList();
    }

    public async Task RegistrarMovimentacaoAsync(
        Guid produtoId,
        TipoMovimentacao tipo,
        int quantidade,
        Guid? usuarioId,
        string? motivo,
        CancellationToken ct = default)
    {
        var estoque = await estoqueRepo.ObterPorProdutoAsync(produtoId, ct)
            ?? throw new KeyNotFoundException("Estoque não encontrado para o produto.");

        if (tipo == TipoMovimentacao.Entrada)
            estoque.Entrada(quantidade);
        else if (tipo == TipoMovimentacao.Saida)
            estoque.Saida(quantidade);

        var movimentacao = new MovimentacaoEstoque(
            produtoId, tipo, quantidade, usuarioId, motivo);

        estoqueRepo.Atualizar(estoque);
        await produtoRepo.AdicionarMovimentacaoAsync(movimentacao, ct);
        await uow.CommitAsync(ct);
    }
}
