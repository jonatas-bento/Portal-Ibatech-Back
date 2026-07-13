using Ibatech.Domain.Entities.Base;
using Ibatech.Domain.Enums;

namespace Ibatech.Domain.Entities;

public sealed class Venda : EntityBase
{
    private readonly List<VendaItem> _itens = [];

    public string Numero { get; private set; } = null!;
    public Guid? ClienteId { get; private set; }
    public string? ClienteNomeSnapshot { get; private set; }
    public string? ClienteCpfCnpjSnapshot { get; private set; }
    public Guid VendedorId { get; private set; }
    public string VendedorNomeSnapshot { get; private set; } = null!;
    public StatusVenda Status { get; private set; }
    public DateTime DataVenda { get; private set; }
    public decimal ValorBruto { get; private set; }
    public decimal Desconto { get; private set; }
    public decimal ValorTotal { get; private set; }
    public string? Observacao { get; private set; }

    public IReadOnlyCollection<VendaItem> Itens => _itens.AsReadOnly();

    private Venda() { }

    public Venda(
        string numero,
        Guid? clienteId,
        string? clienteNomeSnapshot,
        string? clienteCpfCnpjSnapshot,
        Guid vendedorId,
        string vendedorNomeSnapshot,
        string? observacao)
    {
        if (string.IsNullOrWhiteSpace(numero)) throw new ArgumentException("Número é obrigatório.");
        if (vendedorId == Guid.Empty) throw new ArgumentException("VendedorId inválido.");
        if (string.IsNullOrWhiteSpace(vendedorNomeSnapshot)) throw new ArgumentException("VendedorNomeSnapshot é obrigatório.");

        Numero = numero;
        ClienteId = clienteId;
        ClienteNomeSnapshot = clienteNomeSnapshot;
        ClienteCpfCnpjSnapshot = clienteCpfCnpjSnapshot;
        VendedorId = vendedorId;
        VendedorNomeSnapshot = vendedorNomeSnapshot;
        Observacao = observacao?.Trim();
        Status = StatusVenda.Rascunho;
        DataVenda = DateTime.UtcNow;
        ValorBruto = 0;
        Desconto = 0;
        ValorTotal = 0;
    }

    public void AdicionarItem(
        Guid produtoId,
        string? codigoSku,
        string nomeProduto,
        string? descricaoProduto,
        int quantidade,
        decimal precoUnitario,
        decimal desconto)
    {
        if (Status != StatusVenda.Rascunho) throw new InvalidOperationException("Apenas vendas em rascunho podem ser alteradas.");
        if (_itens.Any(i => i.ProdutoId == produtoId)) throw new InvalidOperationException("Produto já adicionado.");

        var item = new VendaItem(Id, produtoId, codigoSku, nomeProduto, descricaoProduto, quantidade, precoUnitario, desconto);
        _itens.Add(item);
        RecalcularTotais();
        MarcarAtualizado();
    }

    public void AtualizarItem(Guid itemId, int quantidade, decimal desconto)
    {
        if (Status != StatusVenda.Rascunho) throw new InvalidOperationException("Apenas vendas em rascunho podem ser alteradas.");
        
        var item = _itens.FirstOrDefault(i => i.Id == itemId);
        if (item == null) throw new InvalidOperationException("Item não encontrado.");

        item.Atualizar(quantidade, desconto);
        RecalcularTotais();
        MarcarAtualizado();
    }

    public void RemoverItem(Guid itemId)
    {
        if (Status != StatusVenda.Rascunho) throw new InvalidOperationException("Apenas vendas em rascunho podem ser alteradas.");

        var item = _itens.FirstOrDefault(i => i.Id == itemId);
        if (item == null) throw new InvalidOperationException("Item não encontrado.");

        _itens.Remove(item);
        RecalcularTotais();
        MarcarAtualizado();
    }

    public void AtualizarCabecalho(
        Guid? clienteId,
        string? clienteNomeSnapshot,
        string? clienteCpfCnpjSnapshot,
        string? observacao)
    {
        if (Status != StatusVenda.Rascunho) throw new InvalidOperationException("Apenas vendas em rascunho podem ser alteradas.");

        ClienteId = clienteId;
        ClienteNomeSnapshot = clienteNomeSnapshot;
        ClienteCpfCnpjSnapshot = clienteCpfCnpjSnapshot;
        Observacao = observacao?.Trim();
        MarcarAtualizado();
    }

    private void RecalcularTotais()
    {
        ValorBruto = _itens.Sum(i => i.Quantidade * i.PrecoUnitario);
        Desconto = _itens.Sum(i => i.Desconto);
        ValorTotal = _itens.Sum(i => i.ValorTotal);
    }
}
