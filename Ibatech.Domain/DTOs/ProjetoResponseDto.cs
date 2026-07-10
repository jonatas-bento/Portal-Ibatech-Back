namespace Ibatech.Domain.DTOs;

public class ProjetoResponseDto
{
    public Guid Id { get; set; }
    public string NomeEmpresa { get; set; } = string.Empty;
    public string NomeContato { get; set; } = string.Empty;
    public string EmailContato { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; }
}
