// Ibatech.Domain/Entities/ProjetoRequisitos.cs
using Ibatech.Domain.Entities.Base;
using Ibatech.Domain.Enums;

namespace Ibatech.Domain.Entities;

public class ProjetoRequisitos : EntityBase
{
    // Dados do solicitante / contexto B2B
    public Guid UsuarioId { get; private set; }
    public string NomeEmpresa { get; private set; } = string.Empty;
    public string NomeContato { get; private set; } = string.Empty;
    public string EmailContato { get; private set; } = string.Empty;

    // Escopo e dores
    public string DescricaoDores { get; private set; } = string.Empty;
    public string InfraAtual { get; private set; } = string.Empty;
    public bool UtilizaNuvem { get; private set; }
    public string? ProvedorNuvem { get; private set; }
    public int VolumetriaUsuarios { get; private set; }
    public string? ObservacoesExtra { get; private set; }

    // Workflow de tramitação
    public StatusProjeto Status { get; private set; } = StatusProjeto.Recebido;
    public string? NotaAnalista { get; private set; }
    public DateTime? DataProposta { get; private set; }
    public DateTime? DataAprovacao { get; private set; }

    // Navegação
    public Usuario? Usuario { get; private set; }

    protected ProjetoRequisitos() { }

    public ProjetoRequisitos(
        Guid usuarioId,
        string nomeEmpresa,
        string nomeContato,
        string emailContato,
        string descricaoDores,
        string infraAtual,
        bool utilizaNuvem,
        int volumetriaUsuarios,
        string? provedorNuvem = null,
        string? observacoesExtra = null)
    {
        UsuarioId = usuarioId;
        NomeEmpresa = nomeEmpresa;
        NomeContato = nomeContato;
        EmailContato = emailContato;
        DescricaoDores = descricaoDores;
        InfraAtual = infraAtual;
        UtilizaNuvem = utilizaNuvem;
        VolumetriaUsuarios = volumetriaUsuarios;
        ProvedorNuvem = provedorNuvem;
        ObservacoesExtra = observacoesExtra;
    }

    // --- Máquina de estados do workflow ---

    public void IniciarAnalise(string notaAnalista)
    {
        ValidarTransicao(StatusProjeto.Recebido);
        Status = StatusProjeto.EmAnalise;
        NotaAnalista = notaAnalista;
        MarcarAtualizado();
    }

    public void EnviarProposta()
    {
        ValidarTransicao(StatusProjeto.EmAnalise);
        Status = StatusProjeto.PropostaEnviada;
        DataProposta = DateTime.UtcNow;
        MarcarAtualizado();
    }

    public void Aprovar()
    {
        ValidarTransicao(StatusProjeto.PropostaEnviada);
        Status = StatusProjeto.Aprovado;
        DataAprovacao = DateTime.UtcNow;
        MarcarAtualizado();
    }

    public void Cancelar()
    {
        if (Status == StatusProjeto.Aprovado)
            throw new InvalidOperationException("Projeto já aprovado não pode ser cancelado.");
        Status = StatusProjeto.Cancelado;
        MarcarAtualizado();
    }

    private void ValidarTransicao(StatusProjeto statusEsperado)
    {
        if (Status != statusEsperado)
            throw new InvalidOperationException(
                $"Transição inválida: status atual é '{Status}', esperado '{statusEsperado}'.");
    }

    public void AlterarStatus(string novoStatus)
    {
        Status = (StatusProjeto)Enum.Parse(typeof(StatusProjeto), novoStatus, true);
    }
}
