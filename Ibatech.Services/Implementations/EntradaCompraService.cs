using Ibatech.Domain.DTOs;
using Ibatech.Domain.Entities;
using Ibatech.Domain.Enums;
using Ibatech.Domain.Interfaces.Repositories;
using Ibatech.Domain.Interfaces.Services;
using Ibatech.Repository.UnitOfWork;
using Ibatech.Services.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Ibatech.Services.Implementations;

public sealed class EntradaCompraService(
    IEntradaCompraRepository entradaRepository,
    IFornecedorRepository fornecedorRepository,
    IProdutoRepository produtoRepository,
    IUsuarioRepository usuarioRepository,
    IEstoqueRepository estoqueRepository,
    IUnitOfWork uow) : IEntradaCompraService
{
    public async Task<IReadOnlyCollection<EntradaCompraResumoDto>> ListarAsync(
        CancellationToken ct = default)
    {
        var entradas =
            await entradaRepository.ListarAsync(ct);

        return entradas
            .Select(EntradaCompraMapper.ToResumoDto)
            .ToList();
    }

    public async Task<EntradaCompraDetalheDto> ObterPorIdAsync(
        Guid id,
        CancellationToken ct = default)
    {
        if (id == Guid.Empty)
            throw new ArgumentException(
                "ID da entrada inválido.");

        var entrada =
            await entradaRepository.ObterDetalheAsync(id, ct);

        if (entrada is null)
            throw new KeyNotFoundException(
                "Entrada de compra não encontrada.");

        return EntradaCompraMapper.ToDetalheDto(entrada);
    }

    public async Task<EntradaCompraDetalheDto> CriarAsync(
        CriarEntradaCompraDto dto,
        Guid usuarioId,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var usuario =
            await ValidarUsuarioOperacaoAsync(usuarioId, ct);

        var fornecedor =
            await fornecedorRepository.ObterPorIdAsync(
                dto.FornecedorId,
                ct);

        if (fornecedor is null || !fornecedor.Ativo)
            throw new InvalidOperationException(
                "Fornecedor não encontrado ou inativo.");

        if (await entradaRepository.ExisteDocumentoAsync(
                fornecedor.Id,
                dto.NumeroDocumento,
                ct))
        {
            throw new InvalidOperationException(
                "Já existe uma entrada com este documento para o fornecedor informado.");
        }

        var entrada = new EntradaCompra(
            fornecedor.Id,
            dto.NumeroDocumento,
            dto.DataEntrada,
            dto.ValorFrete,
            dto.ValorDesconto,
            dto.OutrasDespesas,
            usuario.Id,
            dto.Observacao);

        await entradaRepository.AdicionarAsync(
            entrada,
            ct);

        await uow.CommitAsync(ct);

        return await ObterPorIdAsync(
            entrada.Id,
            ct);
    }

    public async Task<EntradaCompraDetalheDto> AdicionarItemAsync(
        Guid entradaId,
        AdicionarEntradaCompraItemDto dto,
        Guid usuarioId,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        await ValidarUsuarioOperacaoAsync(
            usuarioId,
            ct);

        if (entradaId == Guid.Empty)
            throw new ArgumentException(
                "ID da entrada inválido.");

        if (dto.ProdutoId == Guid.Empty)
            throw new ArgumentException(
                "ID do produto inválido.");

        var entrada =
            await entradaRepository.ObterComItensAsync(
                entradaId,
                ct);

        if (entrada is null)
            throw new KeyNotFoundException(
                "Entrada de compra não encontrada.");

        var produto =
            await produtoRepository.ObterPorIdAsync(
                dto.ProdutoId,
                ct);

        if (produto is null || !produto.Ativo)
            throw new InvalidOperationException(
                "Produto não encontrado ou inativo.");

        var item = entrada.AdicionarItem(
            produto.Id,
            produto.CodigoSku,
            produto.CodigoFornecedor,
            produto.Nome,
            dto.Quantidade,
            dto.PrecoUnitarioCompra);

        entradaRepository.AdicionarItem(item);

        await uow.CommitAsync(ct);

        return await ObterPorIdAsync(
            entrada.Id,
            ct);
    }

    public async Task<EntradaCompraDetalheDto> ConfirmarAsync(
        Guid entradaId,
        Guid usuarioId,
        CancellationToken ct = default)
    {
        if (entradaId == Guid.Empty)
            throw new ArgumentException(
                "ID da entrada inválido.");

        var usuario =
            await ValidarUsuarioOperacaoAsync(
                usuarioId,
                ct);

        var entrada =
            await entradaRepository.ObterComItensAsync(
                entradaId,
                ct);

        if (entrada is null)
            throw new KeyNotFoundException(
                "Entrada de compra não encontrada.");

        var dataOperacaoUtc = DateTime.UtcNow;

        // Valida todo o agregado antes de qualquer alteração de estoque.
        entrada.ValidarConfirmacao(dataOperacaoUtc);

        var produtoIds = entrada.Itens
            .Select(x => x.ProdutoId)
            .Distinct()
            .ToArray();

        // Produtos carregados explicitamente de forma rastreada,
        // pois o custo atual será atualizado nesta operação.
        var produtos =
            await produtoRepository.ObterPorIdsAsync(
                produtoIds,
                ct);

        var produtosPorId =
            produtos.ToDictionary(x => x.Id);

        if (produtosPorId.Count != produtoIds.Length)
            throw new InvalidOperationException(
                "Um ou mais produtos da entrada não foram encontrados.");

        foreach (var item in entrada.Itens)
        {
            if (!produtosPorId.TryGetValue(
                    item.ProdutoId,
                    out var produto) ||
                !produto.Ativo)
            {
                throw new InvalidOperationException(
                    $"Produto '{item.NomeProduto}' não encontrado ou inativo.");
            }
        }

        var estoques =
            await estoqueRepository.ObterPorProdutosAsync(
                produtoIds,
                ct);

        var estoquesPorProduto =
            estoques.ToDictionary(x => x.ProdutoId);

        // Nenhuma alteração é feita até termos certeza de que
        // todos os produtos possuem estoque cadastrado.
        foreach (var item in entrada.Itens)
        {
            if (!estoquesPorProduto.ContainsKey(
                    item.ProdutoId))
            {
                throw new InvalidOperationException(
                    $"Estoque não encontrado para o produto '{item.NomeProduto}'.");
            }
        }

        var movimentacoes =
            new List<MovimentacaoEstoque>(
                entrada.Itens.Count);

        foreach (var item in entrada.Itens)
        {
            var estoque =
                estoquesPorProduto[item.ProdutoId];

            var produto =
                produtosPorId[item.ProdutoId];

            estoque.Entrada(item.Quantidade);

            // O histórico permanece com 4 casas em EntradaCompraItem.
            // Produto.PrecoCompra representa o custo efetivo corrente.
            var custoAtual = Math.Round(
                item.CustoEfetivoUnitario,
                2,
                MidpointRounding.AwayFromZero);

            produto.AtualizarPrecos(
                custoAtual,
                produto.PrecoVenda);

            var movimentacao =
                new MovimentacaoEstoque(
                    item.ProdutoId,
                    TipoMovimentacao.Entrada,
                    item.Quantidade,
                    usuario.Id,
                    $"Entrada de compra {entrada.NumeroDocumento}",
                    vendaId: null,
                    entradaCompraItemId: item.Id);

            movimentacoes.Add(movimentacao);
        }

        await estoqueRepository
            .AdicionarMovimentacoesAsync(
                movimentacoes,
                ct);

        entrada.Confirmar(dataOperacaoUtc);

        try
        {
            // Estoques + produtos + movimentações + entrada:
            // todos persistidos no mesmo SaveChanges.
            await uow.CommitAsync(ct);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new InvalidOperationException(
                "A entrada, o produto ou o estoque foi alterado por outra operação. Atualize os dados e tente novamente.",
                ex);
        }

        return await ObterPorIdAsync(
            entrada.Id,
            ct);
    }

    private async Task<Usuario> ValidarUsuarioOperacaoAsync(
        Guid usuarioId,
        CancellationToken ct)
    {
        if (usuarioId == Guid.Empty)
            throw new ArgumentException(
                "Usuário inválido.");

        var usuario =
            await usuarioRepository.ObterPorIdAsync(
                usuarioId,
                ct);

        if (usuario is null || !usuario.Ativo)
            throw new InvalidOperationException(
                "Usuário responsável não encontrado ou inativo.");

        if (usuario.Role != RoleUsuario.Admin &&
            usuario.Role != RoleUsuario.Estoque)
        {
            throw new InvalidOperationException(
                "Usuário sem permissão para registrar entradas de compra.");
        }

        return usuario;
    }
}
