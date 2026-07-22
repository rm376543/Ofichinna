using Ofichina.Domain.Common;
using Ofichina.Domain.Entities;

namespace Ofichina.Application.Abstractions.Interfaces;

/// <summary>
/// Repositório específico para persistência das peças vinculadas a serviços.
/// </summary>
public interface IServicoPecasRepository : IRepository<PecaServico>
{
    /// <summary>
    /// Obtém um vínculo entre serviço e peça.
    /// </summary>
    Task<PecaServico?> GetByServicoIdAndPecaIdAsync(
        Guid servicoId,
        Guid pecaId,
        CancellationToken cancellationToken = default,
        bool tracking = false);

    /// <summary>
    /// Obtém todas as peças vinculadas a um serviço.
    /// </summary>
    Task<IReadOnlyCollection<PecaServico>> GetByServicoIdAsync(
        Guid servicoId,
        CancellationToken cancellationToken = default,
        bool includePeca = false,
        bool tracking = false);

    /// <summary>
    /// Adiciona uma peça a um serviço.
    /// </summary>
    Task<PecaServico> AdicionarAsync(
        Guid servicoId,
        Guid pecaId,
        int quantidade,
        CancellationToken cancellationToken = default);
}