namespace Ofichina.Contracts.Common;

/// <summary>
/// DTO base para entidades.
/// Contém apenas propriedades de auditoria compartilhadas.
/// </summary>
public abstract class BaseResponse
{
    /// <summary>
    /// Data de criação da entidade.
    /// </summary>
    public string CreatedAt { get; set; } = string.Empty;

    /// <summary>
    /// Data da última atualização da entidade.
    /// </summary>
    public string? UpdatedAt { get; set; }

    /// <summary>
    /// Data de exclusão (soft-delete) da entidade.
    /// </summary>
    public string? DeletedAt { get; set; }
}
