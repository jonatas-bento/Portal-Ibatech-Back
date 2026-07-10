// Ibatech.Services/Mappers/UsuarioMapper.cs
using Ibatech.Domain.DTOs;
using Ibatech.Domain.Entities;
using UsuarioServiceDto = Ibatech.Services.DTOs.Usuario.UsuarioResponseDto;

namespace Ibatech.Services.Mappers;

public static class UsuarioMapper
{
    public static UsuarioServiceDto ToDto(this Usuario u) => new(
        u.Id,
        u.NomeCompleto,
        u.Email,
        u.Telefone,
        u.Cpf,
        u.Role,
        u.Ativo,
        u.CriadoEm
    );

    public static IEnumerable<UsuarioServiceDto> ToDtoList(
        this IEnumerable<Usuario> lista) =>
        lista.Select(u => u.ToDto());

    // Mapeamento para DTOs do Domain
    public static UsuarioResponseDto ToDomainDto(this Usuario u) =>
        new()
        {
            Id = u.Id,
            NomeCompleto = u.NomeCompleto,
            Email = u.Email,
            Telefone = u.Telefone,
            Cpf = u.Cpf,
            Role = u.Role.ToString()
        };

    public static IEnumerable<UsuarioResponseDto> ToDomainDtoList(
        this IEnumerable<Usuario> lista) =>
        lista.Select(u => u.ToDomainDto());
}
