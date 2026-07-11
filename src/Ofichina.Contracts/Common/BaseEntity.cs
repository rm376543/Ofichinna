namespace Ofichina.Contracts.Common;

/// <summary>
/// DTO base para entidades.
/// Contém propriedades comuns de todas as entidades.
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
