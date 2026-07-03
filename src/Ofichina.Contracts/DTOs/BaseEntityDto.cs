namespace Ofichina.Contracts.DTOs;

/// <summary>
/// DTO base para entidades.
/// Contém propriedades comuns de todas as entidades.
/// </summary>
public abstract class BaseEntityDto
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
