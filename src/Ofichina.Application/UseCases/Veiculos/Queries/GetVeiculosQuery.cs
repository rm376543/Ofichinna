using Ofichina.Application.Abstractions;
using Ofichina.Contracts;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses;
using Ofichina.Contracts.Responses.Veiculo;
using Ofichina.Domain.Common;

namespace Ofichina.Application.UseCases.Veiculos.Queries;

/// <summary>
/// Consulta para listar veículos.
/// </summary>
public sealed class GetAllVeiculosPaginadosQuery : IQuery<Result<PagedResponse<VeiculoResponse>>>
{
    public Pagination Pagination { get; init; } = new();
}
