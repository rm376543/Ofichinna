namespace Ofichina.Domain.Entities;

/// <summary>
/// Classe base para todas as entidades do domínio.
/// Define a identidade da entidade através do Id.
/// Contém propriedades comuns de auditoria e controle de ciclo de vida.
/// </summary>
public abstract class Entity
{
    /// <summary>
    /// Identificador único da entidade.
    /// </summary>
    public Guid Id { get; protected set; }

    /// <summary>
    /// Data de criação da entidade.
    /// </summary>
    public DateTime CreatedAt { get; protected set; }

    /// <summary>
    /// Data da última atualização da entidade.
    /// </summary>
    public DateTime? UpdatedAt { get; protected set; }

    /// <summary>
    /// Data de exclusão (soft-delete) da entidade.
    /// Quando preenchida, indica que a entidade foi removida logicamente.
    /// </summary>
    public DateTime? DeletedAt { get; protected set; }

    /// <summary>
    /// Construtor protegido utilizado pelas entidades no projeto de testes unitarios.
    /// </summary>
    protected Entity(Guid id)
    {
        Id = id;
    }

    /// <summary>
    /// Construtor protegido utilizado pelas entidades do domínio.
    /// Também permite que o Entity Framework Core materialize a entidade.
    /// </summary>
    protected Entity()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = null;
        DeletedAt = null;
    }

    /// <summary>
    /// Atualiza a data de modificação da entidade.
    /// </summary>
    public void AtualizarDataModificacao()
    {
        UpdatedAt = DateTime.UtcNow;
    }


    /// <summary>
    /// Realiza a exclusão lógica da entidade (soft-delete).
    /// </summary>
    public void Excluir()
    {
        DeletedAt = DateTime.UtcNow;
        AtualizarDataModificacao();
    }


    /// <summary>
    /// Verifica se a entidade foi excluída logicamente.
    /// </summary>
    public bool EstaExcluida()
    {
        return DeletedAt.HasValue;
    }


    /// <summary>
    /// Reativa uma entidade previamente excluída logicamente.
    /// </summary>
    public void Reativar()
    {
        if (!EstaExcluida())
            return;

        DeletedAt = null;
        AtualizarDataModificacao();
    }


    /// <summary>
    /// Compara entidades através do seu identificador único.
    /// Entidades são consideradas iguais quando possuem o mesmo Id.
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj is not Entity entity)
            return false;

        if (ReferenceEquals(this, entity))
            return true;

        if (Id == Guid.Empty || entity.Id == Guid.Empty)
            return false;

        return GetType() == entity.GetType() && Id == entity.Id;
    }


    /// <summary>
    /// Retorna o hash baseado no identificador único da entidade.
    /// </summary>
    public override int GetHashCode()
    {
        return HashCode.Combine(GetType(), Id);
    }
}