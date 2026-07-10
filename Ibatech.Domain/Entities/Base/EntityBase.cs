// Ibatech.Domain/Entities/Base/EntityBase.cs
namespace Ibatech.Domain.Entities.Base;

public abstract class EntityBase
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTime CriadoEm { get; protected set; } = DateTime.UtcNow;
    public DateTime? AtualizadoEm { get; protected set; }
    public bool Ativo { get; protected set; } = true;

    public void Desativar() => Ativo = false;
    public void MarcarAtualizado() => AtualizadoEm = DateTime.UtcNow;
}