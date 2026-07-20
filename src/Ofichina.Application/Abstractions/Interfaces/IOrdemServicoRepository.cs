using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Common;

namespace Ofichina.Application.Abstractions.Interfaces;

/// <summary>
/// Repositório específico para consultas da ordem de serviço com seus itens.
/// </summary>
public interface IOrdemServicoRepository : IRepository<OrdemServico>
{
    /// <summary>
    /// Obtém uma ordem de serviço pelo identificador, carregando os itens quando necessário.
    /// </summary>
    /// <param name="id">Identificador da ordem de serviço.</param>
    /// <param name="includeItens">Indica se os itens da ordem devem ser carregados.</param>
    Task<OrdemServico?> GetByIdAsync(Guid id, bool includeItens = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém todas as ordens de serviço, carregando os itens quando necessário.
    /// </summary>
    /// <param name="includeItens">Indica se os itens da ordem devem ser carregados.</param>
    Task<IReadOnlyCollection<OrdemServico>> GetAllAsync(bool includeItens = false, CancellationToken cancellationToken = default);
}
