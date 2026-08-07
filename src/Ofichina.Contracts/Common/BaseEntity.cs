namespace Ofichina.Contracts.Common;

/// <summary>
/// DTO base para entidades.
/// Contém apenas propriedades de auditoria compartilhadas.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Data de criação da entidade.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Data da última atualização da entidade.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Data de exclusão (soft-delete) da entidade.
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}
