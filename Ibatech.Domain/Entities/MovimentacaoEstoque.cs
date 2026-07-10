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
    public Produto? Produto { get; private set; }
    public Usuario? Usuario { get; private set; }

    protected MovimentacaoEstoque() { }

    public MovimentacaoEstoque(
        Guid produtoId,
        TipoMovimentacao tipo,
        int quantidade,
        Guid? usuarioId = null,
        string? motivo = null)
    {
        ProdutoId = produtoId;
        Tipo = tipo;
        Quantidade = quantidade;
        UsuarioId = usuarioId;
        Motivo = motivo;
    }
}