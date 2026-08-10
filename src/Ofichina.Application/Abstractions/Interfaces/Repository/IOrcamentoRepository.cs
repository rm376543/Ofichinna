using Ofichina.Domain.Aggregates;

namespace Ofichina.Application.Abstractions.Interfaces.Repository;

/// <summary>
/// Repositório específico para consultas de orçamento com seus itens e checklist.
/// </summary>
public interface IOrcamentoRepository : IRepository<Orcamento>
{
    Task<Orcamento?> GetByIdAsync(Guid id, bool includeItens = false, CancellationToken cancellationToken = default, bool tracking = false);

    Task<IReadOnlyCollection<Orcamento>> GetAllAsync(bool includeItens = false, CancellationToken cancellationToken = default);

}
