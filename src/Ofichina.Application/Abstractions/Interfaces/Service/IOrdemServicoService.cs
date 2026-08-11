using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.OrdemServico;

namespace Ofichina.Application.Abstractions.Interfaces.Service;

/// <summary>
/// Serviço de leitura para ordens de serviço.
/// </summary>
public interface IOrdemServicoService
{
    /// <summary>
    /// Obtém ordens de serviço de forma paginada com dados simples para listagem.
    /// </summary>
    Task<PagedResponse<OrdemServicoDetalheResponse>> GetAllPagedAsync(Pagination pagination, CancellationToken cancellationToken = default);
}