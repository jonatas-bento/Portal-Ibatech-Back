using Ibatech.Domain.DTOs;
using Ibatech.Domain.Entities;
using Ibatech.Domain.Enums;
using Ibatech.Domain.Exceptions;
using Ibatech.Domain.Interfaces.Repositories;
using Ibatech.Domain.Interfaces.Services;
using Ibatech.Repository.UnitOfWork;
using Ibatech.Services.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Ibatech.Services.Implementations;

public sealed class VendaService(
    IVendaRepository vendaRepository,
    IClienteRepository clienteRepository,
    IProdutoRepository produtoRepository,
    IUsuarioRepository usuarioRepository,
    IEstoqueRepository estoqueRepository,
    IFinanceiroRepository financeiroRepository,
    IUnitOfWork uow) : IVendaService
{
    private const string CategoriaVenda = "Venda";
    private const string CategoriaEstornoVenda = "Estorno de Venda";

    public async Task<IReadOnlyCollection<VendaResumoDto>> ListarAsync(

        VendaFiltroDto filtros,
        CancellationToken cancellationToken = default)
    {
        var vendas = await vendaRepository.ListarAsync(filtros, cancellationToken);
        return vendas.Select(VendaMapper.ToResumoDto).ToList();
    }

    public async Task<VendaDetalheDto> ObterPorIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty) throw new ArgumentException("ID inválido.");

        var venda = await vendaRepository.ObterDetalheAsync(id, cancellationToken);
        if (venda is null) throw new KeyNotFoundException("Venda não encontrada.");

        return VendaMapper.ToDetalheDto(venda);
    }

    public async Task<VendaDetalheDto> CriarAsync(
        CriarVendaDto dto,
        Guid vendedorId,
        CancellationToken cancellationToken = default)
    {
        if (vendedorId == Guid.Empty) throw new ArgumentException("Vendedor inválido.");

        var vendedor = await usuarioRepository.ObterPorIdAsync(vendedorId);
        if (vendedor is null || !vendedor.Ativo)
            throw new InvalidOperationException("Vendedor não encontrado ou inativo.");

        if (vendedor.Role != RoleUsuario.Admin && vendedor.Role != RoleUsuario.Vendedor)
            throw new InvalidOperationException("Usuário sem permissão para criar vendas.");

        Guid? clienteId = null;
        string? clienteNome = null;
        string? clienteCpfCnpj = null;

        if (dto.ClienteId.HasValue)
        {
            var cliente = await clienteRepository.ObterPorIdAsync(dto.ClienteId.Value);
            if (cliente is null || !cliente.Ativo)
                throw new InvalidOperationException("Cliente não encontrado ou inativo.");

            clienteId = cliente.Id;
            clienteNome = cliente.Nome;
            clienteCpfCnpj = cliente.CpfCnpj;
        }

        var numero = await GerarNumeroUnicoAsync(cancellationToken);

        var venda = new Venda(
            numero,
            clienteId,
            clienteNome,
            clienteCpfCnpj,
            vendedor.Id,
            vendedor.NomeCompleto,
            dto.Observacao);

        await vendaRepository.AdicionarAsync(venda);
        await uow.CommitAsync(cancellationToken);

        return VendaMapper.ToDetalheDto(venda);
    }

    public async Task<VendaDetalheDto> AtualizarAsync(
        Guid id,
        AtualizarVendaDto dto,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty) throw new ArgumentException("ID da venda inválido.");

        var venda = await vendaRepository.ObterComItensAsync(id, cancellationToken);
        if (venda is null) throw new KeyNotFoundException("Venda não encontrada.");

        Guid? clienteId = null;
        string? clienteNome = null;
        string? clienteCpfCnpj = null;

        if (dto.ClienteId.HasValue)
        {
            var cliente = await clienteRepository.ObterPorIdAsync(dto.ClienteId.Value);
            if (cliente is null || !cliente.Ativo)
                throw new InvalidOperationException("Cliente não encontrado ou inativo.");

            clienteId = cliente.Id;
            clienteNome = cliente.Nome;
            clienteCpfCnpj = cliente.CpfCnpj;
        }

        venda.AtualizarCabecalho(clienteId, clienteNome, clienteCpfCnpj, dto.Observacao);

        await uow.CommitAsync(cancellationToken);
        return VendaMapper.ToDetalheDto(venda);
    }

    public async Task<VendaDetalheDto> AdicionarItemAsync(
        Guid vendaId,
        AdicionarVendaItemDto dto,
        CancellationToken cancellationToken = default)
    {
        if (vendaId == Guid.Empty) throw new ArgumentException("ID da venda inválido.");
        if (dto.ProdutoId == Guid.Empty) throw new ArgumentException("ID do produto inválido.");

        var venda = await vendaRepository.ObterComItensAsync(vendaId, cancellationToken);
        if (venda is null) throw new KeyNotFoundException("Venda não encontrada.");

        var produto = await produtoRepository.ObterPorIdAsync(dto.ProdutoId);
        if (produto is null || !produto.Ativo)
            throw new InvalidOperationException("Produto não encontrado ou inativo.");

        var item = venda.AdicionarItem(
            produto.Id,
            produto.CodigoSku,
            produto.Nome,
            produto.Descricao,
            dto.Quantidade,
            produto.PrecoVenda,
            dto.Desconto);

        vendaRepository.AdicionarItem(item);

        await uow.CommitAsync(cancellationToken);
        return VendaMapper.ToDetalheDto(venda);
    }

    public async Task<VendaDetalheDto> AtualizarItemAsync(
        Guid vendaId,
        Guid itemId,
        AtualizarVendaItemDto dto,
        CancellationToken cancellationToken = default)
    {
        if (vendaId == Guid.Empty) throw new ArgumentException("ID da venda inválido.");
        if (itemId == Guid.Empty) throw new ArgumentException("ID da venda inválido.");

        var venda = await vendaRepository.ObterComItensAsync(vendaId, cancellationToken);
        if (venda is null) throw new KeyNotFoundException("Venda não encontrada.");

        venda.AtualizarItem(itemId, dto.Quantidade, dto.Desconto);

        await uow.CommitAsync(cancellationToken);
        return VendaMapper.ToDetalheDto(venda);
    }

    public async Task<VendaDetalheDto> RemoverItemAsync(
        Guid vendaId,
        Guid itemId,
        CancellationToken cancellationToken = default)
    {
        if (vendaId == Guid.Empty) throw new ArgumentException("ID da venda inválido.");
        if (itemId == Guid.Empty) throw new ArgumentException("ID da venda inválido.");

        var venda = await vendaRepository.ObterComItensAsync(vendaId, cancellationToken);
        if (venda is null) throw new KeyNotFoundException("Venda não encontrada.");

        venda.RemoverItem(itemId);

        await uow.CommitAsync(cancellationToken);
        return VendaMapper.ToDetalheDto(venda);
    }

    public async Task<VendaDetalheDto> FinalizarAsync(
        Guid vendaId,
        FinalizarVendaDto dto,
        Guid usuarioId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (vendaId == Guid.Empty) throw new ArgumentException("ID da venda inválido.");
        if (usuarioId == Guid.Empty) throw new ArgumentException("Usuário inválido.");

        var usuario = await usuarioRepository.ObterPorIdAsync(usuarioId);

        if (usuario is null || !usuario.Ativo)
        {
            throw new InvalidOperationException(
                "Usuário responsável não encontrado ou inativo.");
        }

        if (usuario.Role != RoleUsuario.Admin &&
            usuario.Role != RoleUsuario.Vendedor)
        {
            throw new InvalidOperationException(
                "Usuário sem permissão para finalizar vendas.");
        }

        // 1. Carregar a venda rastreada, com itens, em uma única instância.
        var venda = await vendaRepository.ObterComItensAsync(vendaId, cancellationToken);
        if (venda is null) throw new KeyNotFoundException("Venda não encontrada.");

        // Data única da operação, usada em todos os registros criados.
        var dataOperacaoUtc = DateTime.UtcNow;

        // 2. Validar a conclusão da venda (status, itens, total, pagamento)
        //    ANTES de tocar em qualquer estoque.
        venda.ValidarConclusao(dto.FormaPagamento, dto.ValorRecebido, dataOperacaoUtc);

        // 3. Carregar todos os estoques necessários em uma única consulta.
        var produtoIds = venda.Itens
            .Select(i => i.ProdutoId)
            .Distinct()
            .ToArray();

        var estoques = await estoqueRepository.ObterPorProdutosAsync(produtoIds, cancellationToken);
        var estoquesPorProduto = estoques.ToDictionary(e => e.ProdutoId);

        // 4. Validar todos os estoques antes de alterar qualquer um deles.
        foreach (var item in venda.Itens)
        {
            if (!estoquesPorProduto.TryGetValue(item.ProdutoId, out var estoque))
            {
                throw new InvalidOperationException(
                    $"Estoque não encontrado para o produto '{item.NomeProduto}'.");
            }

            if (item.Quantidade > estoque.QuantidadeAtual)
            {
                throw new InvalidOperationException(
                    $"Estoque insuficiente para o produto '{item.NomeProduto}'. " +
                    $"Disponível: {estoque.QuantidadeAtual}. Solicitado: {item.Quantidade}.");
            }
        }

        // 5. Executar a baixa de estoque e criar as movimentações.
        var movimentacoes = new List<MovimentacaoEstoque>(venda.Itens.Count);

        foreach (var item in venda.Itens)
        {
            var estoque = estoquesPorProduto[item.ProdutoId];
            estoque.Saida(item.Quantidade);

            var movimentacao = new MovimentacaoEstoque(
                item.ProdutoId,
                TipoMovimentacao.Saida,
                item.Quantidade,
                usuario.Id,
                $"Venda {venda.Numero} concluída",
                venda.Id);

            movimentacoes.Add(movimentacao);
        }

        await estoqueRepository.AdicionarMovimentacoesAsync(movimentacoes, cancellationToken);

        // 6. Criar a transação financeira de entrada com o valor total da
        //    venda (nunca o valor recebido) e liquidá-la imediatamente.
        var transacao = new TransacaoFinanceira(
            $"Venda {venda.Numero}",
            venda.ValorTotal,
            TipoTransacao.Receita,
            dataOperacaoUtc,
            CategoriaVenda,
            usuario.Id,
            venda.Id);

        transacao.Liquidar(dataOperacaoUtc);

        await financeiroRepository.AdicionarAsync(transacao, cancellationToken);

        // 7. Concluir a venda (revalida e define os campos de finalização).
        venda.Concluir(dto.FormaPagamento, dto.ValorRecebido, dataOperacaoUtc);

        // 8. Único CommitAsync, com tratamento de concorrência.
        try
        {
            await uow.CommitAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new InvalidOperationException(
                "A venda ou o estoque foi alterado por outra operação. Atualize os dados e tente novamente.",
                ex);
        }

        return VendaMapper.ToDetalheDto(venda);
    }

    public async Task<VendaDetalheDto> CancelarAsync(
        Guid vendaId,
        CancelarVendaDto dto,
        Guid usuarioId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (vendaId == Guid.Empty) throw new ArgumentException("ID da venda inválido.");
        if (usuarioId == Guid.Empty) throw new ArgumentException("Usuário inválido.");

        var usuario = await usuarioRepository.ObterPorIdAsync(usuarioId);

        if (usuario is null || !usuario.Ativo)
        {
            throw new InvalidOperationException(
                "Usuário responsável não encontrado ou inativo.");
        }

        if (usuario.Role != RoleUsuario.Admin && usuario.Role != RoleUsuario.Vendedor)
        {
            throw new InvalidOperationException(
                "Usuário sem permissão para cancelar vendas.");
        }

        var venda = await vendaRepository.ObterComItensAsync(vendaId, cancellationToken);
        if (venda is null) throw new KeyNotFoundException("Venda não encontrada.");

        // Vendedor só pode cancelar as próprias vendas; Admin pode cancelar
        // qualquer rascunho.
        if (usuario.Role == RoleUsuario.Vendedor && venda.VendedorId != usuario.Id)
        {
            throw new ForbiddenAccessException("Você não tem permissão para cancelar esta venda.");
        }

        var dataOperacaoUtc = DateTime.UtcNow;

        venda.Cancelar(dto.Motivo, usuario.Id, dataOperacaoUtc);

        try
        {
            await uow.CommitAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new InvalidOperationException(
                "A venda ou o estoque foi alterado por outra operação. Atualize os dados e tente novamente.",
                ex);
        }

        return VendaMapper.ToDetalheDto(venda);
    }

    public async Task<VendaDetalheDto> EstornarAsync(
        Guid vendaId,
        EstornarVendaDto dto,
        Guid usuarioId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (vendaId == Guid.Empty) throw new ArgumentException("ID da venda inválido.");
        if (usuarioId == Guid.Empty) throw new ArgumentException("Usuário inválido.");

        var usuario = await usuarioRepository.ObterPorIdAsync(usuarioId);

        if (usuario is null || !usuario.Ativo)
        {
            throw new InvalidOperationException(
                "Usuário responsável não encontrado ou inativo.");
        }

        if (usuario.Role != RoleUsuario.Admin)
        {
            throw new InvalidOperationException(
                "Somente administradores podem estornar vendas.");
        }

        // 1. Carregar a venda rastreada, com itens, em uma única instância.
        var venda = await vendaRepository.ObterComItensAsync(vendaId, cancellationToken);
        if (venda is null) throw new KeyNotFoundException("Venda não encontrada.");

        // 2. Data única da operação, usada em todos os registros criados.
        var dataOperacaoUtc = DateTime.UtcNow;

        // 3. Validar o estorno (status, usuário, data, motivo) sem nenhum
        //    efeito colateral, antes de tocar em estoque ou financeiro.
        venda.ValidarEstorno(dto.Motivo, usuario.Id, dataOperacaoUtc);

        // 4. Validar a situação financeira: precisa existir a Receita
        //    original e não pode existir compensação de estorno anterior.
        var existeReceitaOriginal = await financeiroRepository.ExisteTransacaoDaVendaAsync(
            venda.Id, TipoTransacao.Receita, CategoriaVenda, cancellationToken);

        if (!existeReceitaOriginal)
        {
            throw new InvalidOperationException(
                "A venda não possui a receita financeira original.");
        }

        var existeCompensacaoAnterior = await financeiroRepository.ExisteTransacaoDaVendaAsync(
            venda.Id, TipoTransacao.Despesa, CategoriaEstornoVenda, cancellationToken);

        if (existeCompensacaoAnterior)
        {
            throw new InvalidOperationException(
                "A venda já possui um estorno financeiro.");
        }

        // 5. Carregar todos os estoques necessários em uma única consulta.
        var produtoIds = venda.Itens
            .Select(i => i.ProdutoId)
            .Distinct()
            .ToArray();

        var estoques = await estoqueRepository.ObterPorProdutosAsync(produtoIds, cancellationToken);
        var estoquesPorProduto = estoques.ToDictionary(e => e.ProdutoId);

        // 6. Validar que todos os estoques necessários foram encontrados
        //    antes de alterar qualquer um deles.
        foreach (var item in venda.Itens)
        {
            if (!estoquesPorProduto.ContainsKey(item.ProdutoId))
            {
                throw new InvalidOperationException(
                    $"Estoque não encontrado para o produto '{item.NomeProduto}'.");
            }
        }

        // 7. Executar a devolução de estoque e criar as movimentações de
        //    Entrada, uma por item. As movimentações de Saída originais são
        //    preservadas (nunca alteradas ou removidas).
        var movimentacoes = new List<MovimentacaoEstoque>(venda.Itens.Count);

        foreach (var item in venda.Itens)
        {
            var estoque = estoquesPorProduto[item.ProdutoId];
            estoque.Entrada(item.Quantidade);

            var movimentacao = new MovimentacaoEstoque(
                item.ProdutoId,
                TipoMovimentacao.Entrada,
                item.Quantidade,
                usuario.Id,
                $"Estorno da venda {venda.Numero}: {dto.Motivo}",
                venda.Id);

            movimentacoes.Add(movimentacao);
        }

        await estoqueRepository.AdicionarMovimentacoesAsync(movimentacoes, cancellationToken);

        // 8. Criar a transação financeira compensatória de Despesa, com o
        //    ValorTotal da venda (nunca ValorRecebido/Troco), e liquidá-la
        //    imediatamente. A Receita original permanece intacta.
        var compensacao = new TransacaoFinanceira(
            $"Estorno da venda {venda.Numero}",
            venda.ValorTotal,
            TipoTransacao.Despesa,
            dataOperacaoUtc,
            CategoriaEstornoVenda,
            usuario.Id,
            venda.Id);

        compensacao.Liquidar(dataOperacaoUtc);

        await financeiroRepository.AdicionarAsync(compensacao, cancellationToken);

        // 9. Estornar a venda (revalida status e registra auditoria).
        venda.Estornar(dto.Motivo, usuario.Id, dataOperacaoUtc);

        // 10. Único CommitAsync, com tratamento de concorrência.

        try
        {
            await uow.CommitAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new InvalidOperationException(
                "A venda ou o estoque foi alterado por outra operação. Atualize os dados e tente novamente.",
                ex);
        }

        return VendaMapper.ToDetalheDto(venda);
    }

    private async Task<string> GerarNumeroUnicoAsync(CancellationToken ct)

    {

        for (int i = 0; i < 5; i++)
        {
            var sufixo = Guid.NewGuid().ToString("N")[..10].ToUpperInvariant();
            var numero = $"VD-{DateTime.UtcNow:yyMMdd}-{sufixo}";

            if (!await vendaRepository.ExisteNumeroAsync(numero, ct))
                return numero;
        }

        throw new InvalidOperationException("Não foi possível gerar um número de venda único após várias tentativas.");
    }
}
