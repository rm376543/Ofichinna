using Ofichina.Domain.Aggregates;

namespace Ofichina.Application.Abstractions.Interfaces;

/// <summary>
/// Repositório específico para consultas da ordem de serviço com seus itens.
/// </summary>
public interface IOrdemServicoRepository : IRepository<OrdemServico>
{
    /// <summary>
    /// Obtém uma ordem de serviço pelo identificador, carregando os itens quando necessário.
    /// </summary>
    Task<OrdemServico?> GetByIdAsync(Guid id, bool includeItens = false, CancellationToken cancellationToken = default, bool tracking = false);

    /// <summary>
    /// Obtém todas as ordens de serviço, carregando os itens quando necessário.
    /// </summary>
    Task<IReadOnlyCollection<OrdemServico>> GetAllAsync(bool includeItens = false, CancellationToken cancellationToken = default);

}
