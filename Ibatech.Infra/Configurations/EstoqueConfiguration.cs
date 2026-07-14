// Ibatech.Infra/Configurations/EstoqueConfiguration.cs
using Ibatech.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class EstoqueConfiguration : IEntityTypeConfiguration<Estoque>
{
    public void Configure(EntityTypeBuilder<Estoque> b)
    {
        b.ToTable("estoques");
        b.HasKey(e => e.Id);
        b.Property(e => e.QuantidadeAtual).IsRequired().IsConcurrencyToken();
        b.Property(e => e.QuantidadeMinima).IsRequired().HasDefaultValue(5);
        // Coluna computada para alerta (lida no domínio, não persiste)
        b.Ignore(e => e.EstaBaixoDoMinimo);
    }
}