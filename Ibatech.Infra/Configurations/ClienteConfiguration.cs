using Ibatech.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ibatech.Infra.Configurations;

public sealed class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> b)
    {
        b.ToTable("Clientes");
        b.HasKey(c => c.Id);
        
        b.Property(c => c.Nome).IsRequired().HasMaxLength(150);
        b.Property(c => c.CpfCnpj).HasMaxLength(14);
        b.Property(c => c.Telefone).HasMaxLength(20);
        b.Property(c => c.Email).HasMaxLength(150);
        b.Property(c => c.Endereco).HasMaxLength(300);
        b.Property(c => c.Observacao).HasMaxLength(1000);

        b.HasIndex(c => c.CpfCnpj).IsUnique();
    }
}
