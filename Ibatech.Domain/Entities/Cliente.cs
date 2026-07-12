using Ibatech.Domain.Entities.Base;

namespace Ibatech.Domain.Entities;

public sealed class Cliente : EntityBase
{
    public string Nome { get; private set; }
    public string? CpfCnpj { get; private set; }
    public string? Telefone { get; private set; }
    public string? Email { get; private set; }
    public string? Endereco { get; private set; }
    public string? Observacao { get; private set; }

    private Cliente() { }

    public Cliente(string nome, string? cpfCnpj, string? telefone, string? email, string? endereco, string? observacao)
    {
        Nome = nome;
        CpfCnpj = cpfCnpj;
        Telefone = telefone;
        Email = email?.ToLowerInvariant();
        Endereco = endereco;
        Observacao = observacao;
    }

    public void Atualizar(string nome, string? cpfCnpj, string? telefone, string? email, string? endereco, string? observacao)
    {
        Nome = nome;
        CpfCnpj = cpfCnpj;
        Telefone = telefone;
        Email = email?.ToLowerInvariant();
        Endereco = endereco;
        Observacao = observacao;
    }
}
