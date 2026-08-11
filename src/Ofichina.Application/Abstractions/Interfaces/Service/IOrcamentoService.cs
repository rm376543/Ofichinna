using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Orcamento;

namespace Ofichina.Application.Abstractions.Interfaces.Service;

/// <summary>
/// Serviço de leitura para orçamentos.
/// </summary>
public interface IOrcamentoService
{
    Task<PagedResponse<OrcamentoDetalheResponse>> GetAllPagedAsync(Pagination pagination, CancellationToken cancellationToken = default);
}
