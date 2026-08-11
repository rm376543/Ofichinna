using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Veiculo;

namespace Ofichina.Application.UseCases.Veiculos.Queries;

public sealed class GetAllVeiculosPaginadosQuery : IQuery<Result<PagedResponse<VeiculoResponse>>>
{
    /// <summary>
    /// Consulta para listar veículos.
    /// </summary>
    public Pagination Pagination { get; }

    public GetAllVeiculosPaginadosQuery(Pagination pagination)
    {
        Pagination = pagination;
    }
}
