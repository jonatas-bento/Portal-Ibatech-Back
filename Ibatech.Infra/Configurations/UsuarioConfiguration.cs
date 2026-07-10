// Ibatech.Infra/Configurations/UsuarioConfiguration.cs
using Ibatech.Domain.Entities;
using Ibatech.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ibatech.Infra.Configurations;

public sealed class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> b)
    {
        b.ToTable("usuarios");
        b.HasKey(u => u.Id);
        b.Property(u => u.NomeCompleto).IsRequired().HasMaxLength(150);
        b.Property(u => u.Email).IsRequired().HasMaxLength(200);
        b.HasIndex(u => u.Email).IsUnique();
        b.Property(u => u.SenhaHash).IsRequired().HasMaxLength(500);
        b.Property(u => u.Cpf).HasMaxLength(14);
        b.Property(u => u.Telefone).HasMaxLength(20);
        b.Property(u => u.Role)
            .HasConversion<string>()
            .HasMaxLength(30);

        b.HasMany(u => u.Projetos)
            .WithOne(p => p.Usuario)
            .HasForeignKey(p => p.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}



