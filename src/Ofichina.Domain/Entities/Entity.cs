namespace Ofichina.Domain.Entities;

/// <summary>
/// Classe base para todas as entidades do domínio.
/// Define a identidade da entidade através do Id.
/// </summary>
public abstract class Entity
{
    /// <summary>
    /// Identificador único da entidade.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Data de criação da entidade.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Data da última atualização da entidade.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    protected Entity()
    {
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Entity entity)
        {
            return false;
        }

        return Id == entity.Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}
