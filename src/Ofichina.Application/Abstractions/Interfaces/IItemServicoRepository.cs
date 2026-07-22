using Ofichina.Domain.Common;
using Ofichina.Domain.Entities;

namespace Ofichina.Application.Abstractions.Interfaces;

/// <summary>
/// Repositório específico para persistência de itens de serviço.
/// </summary>
public interface IItemServicoRepository : IRepository<ItemServico>
{
    /// <summary>
    /// Obtém um item de serviço pela ordem de serviço e identificador.
    /// </summary>
    Task<ItemServico?> GetByOrdemServicoIdAndIdAsync(
        Guid ordemServicoId,
        Guid id,
        CancellationToken cancellationToken = default,
        bool tracking = false,
        bool includeRelacionados = false);

    /// <summary>
    /// Obtém um item de serviço pela ordem de serviço e pela peça de serviço.
    /// </summary>
    Task<ItemServico?> GetByOrdemServicoIdAndPecaServicoIdAsync(
        Guid ordemServicoId,
        Guid pecaServicoId,
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
    /// Adiciona um novo item de serviço.
    /// </summary>
    Task<ItemServico> AdicionarAsync(
        Guid ordemServicoId,
        Guid pecaServicoId,
        CancellationToken cancellationToken = default);
}
