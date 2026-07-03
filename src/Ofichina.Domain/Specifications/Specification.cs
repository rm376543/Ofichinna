using System.Linq.Expressions;

namespace Ofichina.Domain.Specifications;

/// <summary>
/// Classe base para Specifications Pattern.
/// Encapsula critérios de query complexos em objetos reutilizáveis.
/// </summary>
public abstract class Specification<T>
{
    /// <summary>
    /// Critério de filtro da query.
    /// </summary>
    public Expression<Func<T, bool>>? Criteria { get; protected set; }

    /// <summary>
    /// Projetos de inclusão (Include) para lazy loading.
    /// </summary>
    public List<Expression<Func<T, object>>> Includes { get; } = [];

    /// <summary>
    /// Ordenação padrão.
    /// </summary>
    public Expression<Func<T, object>>? OrderBy { get; protected set; }

    /// <summary>
    /// Ordenação em ordem reversa.
    /// </summary>
    public Expression<Func<T, object>>? OrderByDescending { get; protected set; }

    /// <summary>
    /// Número de registros a pular (para paginação).
    /// </summary>
    public int? Take { get; protected set; }

    /// <summary>
    /// Número de registros a pegar (para paginação).
    /// </summary>
    public int? Skip { get; protected set; }

    /// <summary>
    /// Se deve retornar apenas um resultado.
    /// </summary>
    public bool IsPagingEnabled { get; protected set; }

    /// <summary>
    /// Adiciona um critério Include para carregamento de relacionamentos.
    /// </summary>
    protected virtual void AddInclude(Expression<Func<T, object>> includeExpression)
    {
        Includes.Add(includeExpression);
    }
}
