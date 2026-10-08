using Ibatech.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ibatech.Infra.Configurations;

public sealed class FornecedorConfiguration
    : IEntityTypeConfiguration<Fornecedor>
{
    public void Configure(
        EntityTypeBuilder<Fornecedor> b)
    {
        b.ToTable("Fornecedores");

        b.HasKey(x => x.Id);

        b.Property(x => x.Nome)
            .IsRequired()
            .HasMaxLength(200);

        b.Property(x => x.NomeFantasia)
            .HasMaxLength(200);

        b.Property(x => x.Documento)
            .HasMaxLength(30);

        b.Property(x => x.Email)
            .HasMaxLength(200);

        b.Property(x => x.Telefone)
            .HasMaxLength(30);

        b.Property(x => x.Observacao)
            .HasMaxLength(1000);

        b.HasIndex(x => x.Documento);
        b.HasIndex(x => x.Nome);
    }
}
