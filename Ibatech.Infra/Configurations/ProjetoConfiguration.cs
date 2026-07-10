// Ibatech.Infra/Configurations/ProjetoConfiguration.cs
using Ibatech.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class ProjetoConfiguration : IEntityTypeConfiguration<ProjetoRequisitos>
{
    public void Configure(EntityTypeBuilder<ProjetoRequisitos> b)
    {
        b.ToTable("projetos_requisitos");
        b.HasKey(p => p.Id);
        b.Property(p => p.NomeEmpresa).IsRequired().HasMaxLength(200);
        b.Property(p => p.NomeContato).IsRequired().HasMaxLength(150);
        b.Property(p => p.EmailContato).IsRequired().HasMaxLength(200);
        b.Property(p => p.DescricaoDores).IsRequired().HasColumnType("text");
        b.Property(p => p.InfraAtual).IsRequired().HasColumnType("text");
        b.Property(p => p.Status)
            .HasConversion<string>()
            .HasMaxLength(30);
        b.Property(p => p.NotaAnalista).HasColumnType("text");
    }
}

