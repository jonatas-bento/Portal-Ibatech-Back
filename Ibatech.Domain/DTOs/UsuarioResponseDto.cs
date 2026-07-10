namespace Ibatech.Domain.DTOs;

public class UsuarioResponseDto
{
    public Guid Id { get; set; }
    public string NomeCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public string? Cpf { get; set; }
    public string Role { get; set; } = string.Empty;
}
