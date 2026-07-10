// Ibatech.Domain/Entities/Usuario.cs
using Ibatech.Domain.Entities.Base;
using Ibatech.Domain.Enums;

namespace Ibatech.Domain.Entities;

public class Usuario : EntityBase
{
    public string NomeCompleto { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string SenhaHash { get; private set; } = string.Empty;
    public string? Telefone { get; private set; }
    public string? Cpf { get; private set; }
    public RoleUsuario Role { get; private set; }

    // Relacionamentos
    public IReadOnlyCollection<ProjetoRequisitos> Projetos =>
        _projetos.AsReadOnly();
    private readonly List<ProjetoRequisitos> _projetos = [];

    public IReadOnlyCollection<TransacaoFinanceira> Transacoes =>
        _transacoes.AsReadOnly();
    private readonly List<TransacaoFinanceira> _transacoes = [];

    // EF Core: construtor sem parâmetros exigido
    protected Usuario() { }

    public Usuario(
        string nomeCompleto,
        string email,
        string senhaHash,
        RoleUsuario role,
        string? telefone = null,
        string? cpf = null)
    {
        NomeCompleto = Guard(nomeCompleto, nameof(nomeCompleto));
        Email = Guard(email, nameof(email));
        SenhaHash = Guard(senhaHash, nameof(senhaHash));
        Role = role;
        Telefone = telefone;
        Cpf = cpf;
    }

    public void AtualizarPerfil(string nomeCompleto, string? telefone)
    {
        NomeCompleto = Guard(nomeCompleto, nameof(nomeCompleto));
        Telefone = telefone;
        MarcarAtualizado();
    }

    public void AlterarSenha(string novaSenhaHash)
    {
        SenhaHash = Guard(novaSenhaHash, nameof(novaSenhaHash));
        MarcarAtualizado();
    }

    public void AlterarRole(RoleUsuario novaRole)
    {
        Role = novaRole;
        MarcarAtualizado();
    }

    private static string Guard(string value, string campo)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"Campo '{campo}' não pode ser vazio.", campo);
        return value.Trim();
    }
}