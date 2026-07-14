using Ibatech.Domain.DTOs;

namespace Ibatech.Domain.Interfaces.Services;

public interface IVendaService
{
    Task<IReadOnlyCollection<VendaResumoDto>> ListarAsync(
        VendaFiltroDto filtros,
        CancellationToken cancellationToken = default);

    Task<VendaDetalheDto> ObterPorIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<VendaDetalheDto> CriarAsync(
        CriarVendaDto dto,
        Guid vendedorId,
        CancellationToken cancellationToken = default);

    Task<VendaDetalheDto> AtualizarAsync(
        Guid id,
        AtualizarVendaDto dto,
        CancellationToken cancellationToken = default);

    Task<VendaDetalheDto> AdicionarItemAsync(
        Guid vendaId,
        AdicionarVendaItemDto dto,
        CancellationToken cancellationToken = default);

    Task<VendaDetalheDto> AtualizarItemAsync(
        Guid vendaId,
        Guid itemId,
        AtualizarVendaItemDto dto,
        CancellationToken cancellationToken = default);

    Task<VendaDetalheDto> RemoverItemAsync(
        Guid vendaId,
        Guid itemId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Conclui uma venda em rascunho de maneira atômica: valida a venda e os
    /// estoques, baixa os estoques, registra as movimentações, cria e liquida
    /// a transação financeira de entrada e conclui a venda, persistindo tudo
    /// em um único CommitAsync.
    /// </summary>
    Task<VendaDetalheDto> FinalizarAsync(
        Guid vendaId,
        FinalizarVendaDto dto,
        Guid usuarioId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Cancela uma venda em rascunho. Não movimenta estoque nem cria
    /// transação financeira. Admin pode cancelar qualquer rascunho;
    /// Vendedor só pode cancelar rascunhos dos quais é o próprio VendedorId.
    /// </summary>
    Task<VendaDetalheDto> CancelarAsync(
        Guid vendaId,
        CancelarVendaDto dto,
        Guid usuarioId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Estorna uma venda concluída de maneira atômica: somente Admin,
    /// devolve estoque (nova movimentação de Entrada por item, preservando
    /// as Saídas originais), cria uma transação financeira compensatória de
    /// Despesa com o ValorTotal da venda (preservando a Receita original) e
    /// marca a venda como Estornada, persistindo tudo em um único
    /// CommitAsync.
    /// </summary>
    Task<VendaDetalheDto> EstornarAsync(
        Guid vendaId,
        EstornarVendaDto dto,
        Guid usuarioId,
        CancellationToken cancellationToken = default);
}
