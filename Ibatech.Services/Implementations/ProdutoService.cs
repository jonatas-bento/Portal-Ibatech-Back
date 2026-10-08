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
        ArgumentNullException.ThrowIfNull(dto);

        var tipo =
            ParseTipo(dto.Tipo);

        await ValidarSkuDisponivelAsync(
            produtoIdAtual: null,
            dto.CodigoSku,
            ct);

        var produto =
            new Produto(
                dto.Nome,
                tipo,
                dto.PrecoCompra,
                dto.PrecoVenda,
                dto.Descricao,
                dto.CodigoSku,
                dto.Marca,
                dto.Modelo,
                dto.CodigoFornecedor,
                dto.CodigoBarras,
                dto.Ncm,
                dto.UnidadeComercial);

        await produtoRepo.AdicionarAsync(
            produto,
            ct);

        await uow.CommitAsync(ct);

        var estoque =
            new Estoque(
                produto.Id,
                dto.QuantidadeInicial,
                dto.QuantidadeMinima);

        await estoqueRepo.AdicionarAsync(
            estoque,
            ct);

        /*
         * Na criação manual mantemos o comportamento atual:
         * cria Produto + Estoque.
         *
         * A geração automática de MovimentacaoEstoque será tratada
         * separadamente para não alterar silenciosamente o contrato
         * existente deste endpoint neste momento.
         */
        await uow.CommitAsync(ct);

        var produtoCompleto =
            await produtoRepo.ObterComEstoqueAsync(
                produto.Id,
                ct);

        return produtoCompleto!.ToDomainDto();
    }

    public async Task<ProdutoResponseDto> AtualizarAsync(
        Guid id,
        ProdutoUpdateDto dto,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        if (id == Guid.Empty)
            throw new ArgumentException(
                "ID do produto inválido.");

        var produto =
            await produtoRepo.ObterComEstoqueAsync(
                id,
                ct)
            ?? throw new KeyNotFoundException(
                "Produto não encontrado.");

        if (!produto.Ativo)
            throw new InvalidOperationException(
                "Produto inativo não pode ser alterado.");

        var tipo =
            ParseTipo(dto.Tipo);

        await ValidarSkuDisponivelAsync(
            produto.Id,
            dto.CodigoSku,
            ct);

        produto.AtualizarCadastro(
            dto.Nome,
            tipo,
            dto.PrecoVenda,
            dto.Descricao,
            dto.CodigoSku,
            dto.CodigoFornecedor,
            dto.CodigoBarras,
            dto.Ncm,
            dto.UnidadeComercial,
            dto.Marca,
            dto.Modelo);

        var estoque =
            produto.Estoque
            ?? throw new InvalidOperationException(
                "Estoque não encontrado para o produto.");

        estoque.AjustarMinimo(
            dto.QuantidadeMinima);

        await uow.CommitAsync(ct);

        /*
         * Produto e Estoque continuam rastreados e já possuem
         * os valores persistidos.
         */
        return produto.ToDomainDto();
    }

    public async Task<IEnumerable<ProdutoResponseDto>> ListarAsync(
        CancellationToken ct = default)
    {
        var produtos =
            await produtoRepo.ListarComEstoqueAsync(ct);

        return produtos.ToDomainDtoList();
    }

    public async Task<IEnumerable<ProdutoResponseDto>>
        ListarAlertasReposicaoAsync(
            CancellationToken ct = default)
    {
        var produtos =
            await produtoRepo.ListarComEstoqueAsync(ct);

        return produtos
            .Where(p =>
                p.Estoque?.EstaBaixoDoMinimo == true)
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
        var estoque =
            await estoqueRepo.ObterPorProdutoAsync(
                produtoId,
                ct)
            ?? throw new KeyNotFoundException(
                "Estoque não encontrado para o produto.");

        if (tipo == TipoMovimentacao.Entrada)
        {
            estoque.Entrada(quantidade);
        }
        else if (tipo == TipoMovimentacao.Saida)
        {
            estoque.Saida(quantidade);
        }
        else
        {
            throw new ArgumentException(
                "Tipo de movimentação inválido para esta operação.");
        }

        var movimentacao =
            new MovimentacaoEstoque(
                produtoId,
                tipo,
                quantidade,
                usuarioId,
                motivo);

        estoqueRepo.Atualizar(estoque);

        await produtoRepo.AdicionarMovimentacaoAsync(
            movimentacao,
            ct);

        await uow.CommitAsync(ct);
    }

    private static TipoProduto ParseTipo(
        string tipo)
    {
        if (!Enum.TryParse<TipoProduto>(
                tipo,
                true,
                out var resultado))
        {
            throw new ArgumentException(
                $"Tipo de produto '{tipo}' é inválido.");
        }

        return resultado;
    }

    private async Task ValidarSkuDisponivelAsync(
        Guid? produtoIdAtual,
        string? codigoSku,
        CancellationToken ct)
    {
        var sku =
            codigoSku?.Trim();

        if (string.IsNullOrWhiteSpace(sku))
            return;

        var duplicado =
            await produtoRepo.ExisteAsync(
                p =>
                    p.CodigoSku == sku &&
                    (!produtoIdAtual.HasValue ||
                     p.Id != produtoIdAtual.Value),
                ct);

        if (duplicado)
            throw new InvalidOperationException(
                $"Já existe um produto cadastrado com o SKU '{sku}'.");
    }
}
