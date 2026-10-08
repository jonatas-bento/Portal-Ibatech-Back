using Ibatech.Domain.Entities.Base;

namespace Ibatech.Domain.Entities;

public sealed class Fornecedor : EntityBase
{
    public string Nome { get; private set; } = null!;
    public string? NomeFantasia { get; private set; }
    public string? Documento { get; private set; }
    public string? Email { get; private set; }
    public string? Telefone { get; private set; }
    public string? Observacao { get; private set; }

    private Fornecedor() { }

    public Fornecedor(
        string nome,
        string? nomeFantasia = null,
        string? documento = null,
        string? email = null,
        string? telefone = null,
        string? observacao = null)
    {
        AplicarDados(
            nome,
            nomeFantasia,
            documento,
            email,
            telefone,
            observacao);
    }

    public void Atualizar(
        string nome,
        string? nomeFantasia,
        string? documento,
        string? email,
        string? telefone,
        string? observacao)
    {
        AplicarDados(
            nome,
            nomeFantasia,
            documento,
            email,
            telefone,
            observacao);

        MarcarAtualizado();
    }

    private void AplicarDados(
        string nome,
        string? nomeFantasia,
        string? documento,
        string? email,
        string? telefone,
        string? observacao)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome do fornecedor é obrigatório.");

        Nome = nome.Trim();
        NomeFantasia = Normalizar(nomeFantasia);
        Documento = Normalizar(documento);
        Email = Normalizar(email);
        Telefone = Normalizar(telefone);
        Observacao = Normalizar(observacao);
    }

    private static string? Normalizar(string? valor) =>
        string.IsNullOrWhiteSpace(valor)
            ? null
            : valor.Trim();
}
