using Ofichina.Application.Abstractions;
using Ofichina.Contracts;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.OrdemServico;
using Ofichina.Contracts.Responses.OrdensServico;
using Ofichina.Domain.Common;

namespace Ofichina.Application.UseCases.OrdensServico.Queries;

/// <summary>
/// Consulta para listar ordens de serviço.
/// </summary>
public sealed class GetAllOrdensServicoPaginadasQuery : IQuery<Result<PagedResponse<OrdemServicoSimplesResponse>>>
{
    public Pagination Pagination { get; }

    public GetAllOrdensServicoPaginadasQuery(Pagination pagination)
    {
        Pagination = pagination;
    }
}
