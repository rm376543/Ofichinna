using Ofichina.Domain.Entities;

namespace Ofichina.Application.Abstractions.Interfaces.Repository;

/// <summary>
/// Repositório específico para persistência de itens de serviço.
/// </summary>
public interface IItemServicoRepository : IRepository<ItemServico>
{
    /// <summary>
    /// Obtém um item de serviço pela ordem de serviço e identificador.
    /// </summary>
    Task<ItemServico?> GetByOrdemServicoIdAndItemServicoIdAsync(
        Guid ordemServicoId,
        Guid itemServicoId,
        CancellationToken cancellationToken = default,
        bool tracking = false,
        bool includeRelacionados = false);

    /// <summary>
    /// Obtém um item de serviço pela ordem de serviço, serviço e peça.
    /// </summary>
    Task<ItemServico?> GetByOrdemServicoIdAndServicoIdAndPecaIdAsync(
        Guid ordemServicoId,
        Guid servicoId,
        Guid pecaId,
        CancellationToken cancellationToken = default,
        bool tracking = false);

    /// <summary>
    /// Obtém todos os itens de serviço de uma ordem.
    /// </summary>
    Task<IReadOnlyCollection<ItemServico>> GetByOrdemServicoIdAsync(
        Guid ordemServicoId,
        CancellationToken cancellationToken = default,
        bool includeRelacionados = false,
        bool tracking = false);

    /// <summary>
    /// Busca um item de serviço pelo identificador da ordem de serviço e do item de serviço.
    /// </summary>
    /// <param name="ordemServicoId"></param>
    /// <param name="servicoId"></param>
    /// <param name="pecaId"></param>
    /// <param name="quantidade"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ItemServico> AddAsync(
        Guid ordemServicoId,
        Guid servicoId,
        Guid pecaId,
        int quantidade,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<ItemServico>> GetByOrdemServicoIdAndServicoIdAsync(
        Guid ordemServicoId,
        Guid servicoId,
        CancellationToken cancellationToken = default,
        bool tracking = false,
        bool includeRelacionados = false);

    Task<IReadOnlyCollection<ItemServico>> GetByOrcamentoIdAsync(
        Guid orcamentoId,
        CancellationToken cancellationToken = default,
        bool includeRelacionados = false,
        bool tracking = false);

    Task<ItemServico?> GetByOrcamentoIdAndItemServicoIdAsync(
        Guid orcamentoId,
        Guid itemServicoId,
        CancellationToken cancellationToken = default,
        bool includeRelacionados = false,
        bool tracking = false);

    /// <summary>
    /// Obtém um item de serviço pelo identificador do orçamento, serviço e peça.
    /// </summary>
    /// <param name="orcamentoId"></param>
    /// <param name="servicoId"></param>
    /// <param name="pecaId"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="tracking"></param>
    /// <returns></returns>
    Task<ItemServico?> GetByOrcamentoServicoPecaIdAsync(
        Guid orcamentoId,
        Guid servicoId,
        Guid? pecaId,
        CancellationToken cancellationToken = default,
        bool tracking = false);
}
