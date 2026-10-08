using Ibatech.Domain.Entities.Base;
using Ibatech.Domain.Enums;

namespace Ibatech.Domain.Entities;

public class MovimentacaoEstoque : EntityBase
{
    public Guid ProdutoId { get; private set; }
    public TipoMovimentacao Tipo { get; private set; }
    public int Quantidade { get; private set; }
    public string? Motivo { get; private set; }
    public Guid? UsuarioId { get; private set; }

    public Guid? VendaId { get; private set; }
    public Guid? EntradaCompraItemId { get; private set; }

    public Produto? Produto { get; private set; }
    public Usuario? Usuario { get; private set; }
    public Venda? Venda { get; private set; }
    public EntradaCompraItem? EntradaCompraItem { get; private set; }

    protected MovimentacaoEstoque() { }

    public MovimentacaoEstoque(
        Guid produtoId,
        TipoMovimentacao tipo,
        int quantidade,
        Guid? usuarioId = null,
        string? motivo = null,
        Guid? vendaId = null,
        Guid? entradaCompraItemId = null)
    {
        if (produtoId == Guid.Empty)
            throw new ArgumentException(
                "ProdutoId inválido.",
                nameof(produtoId));

        if (quantidade <= 0)
            throw new ArgumentException(
                "Quantidade deve ser maior que zero.",
                nameof(quantidade));

        if (vendaId.HasValue && vendaId.Value == Guid.Empty)
            throw new ArgumentException(
                "VendaId inválido.",
                nameof(vendaId));

        if (entradaCompraItemId.HasValue &&
            entradaCompraItemId.Value == Guid.Empty)
            throw new ArgumentException(
                "EntradaCompraItemId inválido.",
                nameof(entradaCompraItemId));

        if (vendaId.HasValue && entradaCompraItemId.HasValue)
            throw new InvalidOperationException(
                "Uma movimentação não pode estar vinculada simultaneamente a venda e entrada de compra.");

        ProdutoId = produtoId;
        Tipo = tipo;
        Quantidade = quantidade;
        UsuarioId = usuarioId;
        Motivo = string.IsNullOrWhiteSpace(motivo)
            ? null
            : motivo.Trim();

        VendaId = vendaId;
        EntradaCompraItemId = entradaCompraItemId;
    }
}
