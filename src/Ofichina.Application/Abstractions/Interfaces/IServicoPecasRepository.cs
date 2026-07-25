using Ofichina.Domain.Entities;

namespace Ofichina.Application.Abstractions.Interfaces;

/// <summary>
/// Repositório específico para persistência das peças vinculadas a serviços.
/// </summary>
public interface IServicoPecasRepository : IRepository<ServicoPeca>
{
    /// <summary>
    /// Obtém um vínculo entre serviço e peça.
    /// </summary>
    Task<ServicoPeca?> GetByServicoIdAndPecaIdAsync(
        Guid servicoId,
        Guid pecaId,
        CancellationToken cancellationToken = default,
        bool tracking = false);

    /// <summary>
    /// Obtém todas as peças vinculadas a um serviço.
    /// </summary>
    Task<IReadOnlyCollection<ServicoPeca>> GetByServicoIdAsync(
        Guid servicoId,
        CancellationToken cancellationToken = default,
        bool includePeca = false,
        bool tracking = false);

    /// <summary>
    /// Adiciona uma peça a um serviço.
    /// </summary>
    Task<ServicoPeca> AdicionarAsync(
        Guid servicoId,
        Guid pecaId,
        int quantidade,
        CancellationToken cancellationToken = default);
}