using Ofichina.Contracts;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Entities;

namespace Ofichina.Application.Abstractions.Interfaces.Repository;

/// <summary>
/// Repositório específico para consultas de serviço com peças.
/// </summary>
public interface IServicoRepository : IRepository<Servico>
{
    /// <summary>
    /// Obtém um serviço pelo identificador, carregando as peças quando necessário.
    /// </summary>
    Task<Servico?> GetByIdAsync(Guid id, bool includePecas = false, CancellationToken cancellationToken = default, bool tracking = false);

    /// <summary>
    /// Obtém todos os serviços, carregando as peças quando necessário.
    /// </summary>
    Task<IReadOnlyCollection<Servico>> GetAllAsync(bool includePecas = false, CancellationToken cancellationToken = default);

}