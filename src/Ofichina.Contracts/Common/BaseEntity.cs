namespace Ofichina.Contracts.Common;

/// <summary>
/// DTO base para entidades.
/// Contém o identificador e herda propriedades de auditoria.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Identificador único da entidade.
    /// </summary>
    public Guid Id { get; set; }

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
