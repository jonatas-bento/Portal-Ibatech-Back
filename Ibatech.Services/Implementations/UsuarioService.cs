// Ibatech.Services/Implementations/UsuarioService.cs
using Ibatech.Domain.DTOs;
using Ibatech.Domain.Entities;
using Ibatech.Domain.Enums;
using Ibatech.Domain.Interfaces.Repositories;
using Ibatech.Domain.Interfaces.Services;
using Ibatech.Repository.UnitOfWork;
using Ibatech.Services.Mappers;
using Ibatech.Services.Security;
using Microsoft.EntityFrameworkCore;

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

    public async Task<IEnumerable<UsuarioResumoDto>> ListarAsync(
        string? nome, string? email, RoleUsuario? role, bool? ativo, CancellationToken ct = default)
    {
        var query = usuarioRepo.ObterQueryable().IgnoreQueryFilters();

        if (!string.IsNullOrWhiteSpace(nome))
            query = query.Where(u => u.NomeCompleto.Contains(nome));
        if (!string.IsNullOrWhiteSpace(email))
            query = query.Where(u => u.Email == email);
        if (role.HasValue)
            query = query.Where(u => u.Role == role.Value);
        if (ativo.HasValue)
            query = query.Where(u => u.Ativo == ativo.Value);

        var usuarios = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync(query, ct);
        return usuarios.ToResumoDtoList();
    }

    public async Task AtualizarAsync(Guid id, AtualizarUsuarioDto dto, CancellationToken ct = default)
    {
        var usuario = await usuarioRepo.ObterQueryable().IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == id, ct)
            ?? throw new KeyNotFoundException("Usuário não encontrado.");

        var emailEmUso = await usuarioRepo.ObterQueryable().IgnoreQueryFilters()
            .AnyAsync(u => u.Email == dto.Email && u.Id != id, ct);
        if (emailEmUso)
            throw new InvalidOperationException("E-mail já em uso.");

        usuario.Atualizar(dto.NomeCompleto, dto.Email, dto.Telefone, dto.Cpf, Enum.Parse<RoleUsuario>(dto.Role));
        usuarioRepo.Atualizar(usuario);
        await uow.CommitAsync(ct);
    }

    public async Task AlterarStatusAsync(Guid id, bool ativo, CancellationToken ct = default)
    {
        var usuario = await usuarioRepo.ObterQueryable().IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == id, ct)
            ?? throw new KeyNotFoundException("Usuário não encontrado.");

        if (ativo == false)
        {
            var totalAdmins = await usuarioRepo.ObterQueryable().IgnoreQueryFilters()
                .CountAsync(u => u.Ativo && u.Role == RoleUsuario.Admin, ct);
            if (totalAdmins <= 1 && usuario.Role == RoleUsuario.Admin)
                throw new InvalidOperationException("Não é possível desativar o último administrador.");
        }

        if (ativo == false) usuario.Desativar();
        else usuario.Ativar();

        usuarioRepo.Atualizar(usuario);
        await uow.CommitAsync(ct);
    }

    public async Task RedefinirSenhaAsync(Guid id, string novaSenha, CancellationToken ct = default)
    {
        var usuario = await usuarioRepo.ObterQueryable().IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == id, ct)
            ?? throw new KeyNotFoundException("Usuário não encontrado.");
        
        usuario.AlterarSenha(PasswordHasher.Hash(novaSenha));
        usuarioRepo.Atualizar(usuario);
        await uow.CommitAsync(ct);
    }

    public async Task AlterarMinhaSenhaAsync(Guid id, string senhaAtual, string novaSenha, CancellationToken ct = default)
    {
        var usuario = await usuarioRepo.ObterPorIdAsync(id, ct)
            ?? throw new KeyNotFoundException("Usuário não encontrado.");

        if (!PasswordHasher.Verify(senhaAtual, usuario.SenhaHash))
            throw new InvalidOperationException("Senha atual incorreta.");

        usuario.AlterarSenha(PasswordHasher.Hash(novaSenha));
        usuarioRepo.Atualizar(usuario);
        await uow.CommitAsync(ct);
    }
}
