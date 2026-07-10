namespace Ibatech.Domain.DTOs;

public class ProjetoCreateDto
{
    public string NomeEmpresa { get; set; } = string.Empty;
    public string NomeContato { get; set; } = string.Empty;
    public string EmailContato { get; set; } = string.Empty;
    public string DescricaoDores { get; set; } = string.Empty;
    public string InfraAtual { get; set; } = string.Empty;
    public bool UtilizaNuvem { get; set; }
    public int VolumetriaUsuarios { get; set; }
    public string? ProvedorNuvem { get; set; }
    public string? ObservacoesExtra { get; set; }
}
