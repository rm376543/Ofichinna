using Ofichina.Contracts;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Orcamento;

namespace Ofichina.Application.Abstractions.Interfaces;

/// <summary>
/// Serviço de leitura para orçamentos.
/// </summary>
public interface IOrcamentoService
{
    Task<PagedResponse<OrcamentoSimplesResponse>> GetAllPaginadasAsync(Pagination pagination, CancellationToken cancellationToken = default);
}
