// Ibatech.Services/Implementations/UsuarioService.cs
using Ibatech.Domain.DTOs;
using Ibatech.Domain.Entities;
using Ibatech.Domain.Enums;
using Ibatech.Domain.Interfaces.Repositories;
using Ibatech.Domain.Interfaces.Services;
using Ibatech.Repository.UnitOfWork;
using Ibatech.Services.Mappers;
using Ibatech.Services.Security;

namespace Ibatech.Services.Implementations;

public sealed class UsuarioService(
    IUsuarioRepository usuarioRepo,
    IUnitOfWork uow) : IUsuarioService
{
    public async Task<UsuarioResponseDto> CriarAsync(
        UsuarioCreateDto dto,
        CancellationToken ct = default)
    {
        var existe = await usuarioRepo.ExisteAsync(
            u => u.Email == dto.Email, ct);

        if (existe)
            throw new InvalidOperationException($"E-mail '{dto.Email}' já cadastrado.");

        var senhaHash = PasswordHasher.Hash(dto.Senha);
        var role = Enum.Parse<RoleUsuario>(dto.Role);
        var usuario = new Usuario(
            dto.NomeCompleto,
            dto.Email,
            senhaHash,
            role,
            dto.Telefone,
            dto.Cpf);

        await usuarioRepo.AdicionarAsync(usuario, ct);
        await uow.CommitAsync(ct);

        return usuario.ToDomainDto();
    }

    public async Task<UsuarioResponseDto?> ObterPorIdAsync(
        Guid id,
        CancellationToken ct = default)
    {
        var u = await usuarioRepo.ObterPorIdAsync(id, ct);
        return u?.ToDomainDto();
    }

    public async Task<IEnumerable<UsuarioResponseDto>> ListarAsync(
        CancellationToken ct = default)
    {
        var usuarios = await usuarioRepo.ObterTodosAsync(ct);
        return usuarios.ToDomainDtoList();
    }

    public async Task DesativarAsync(Guid id, CancellationToken ct = default)
    {
        var usuario = await usuarioRepo.ObterPorIdAsync(id, ct)
            ?? throw new KeyNotFoundException("Usuário não encontrado.");
        usuario.Desativar();
        usuarioRepo.Atualizar(usuario);
        await uow.CommitAsync(ct);
    }
}
