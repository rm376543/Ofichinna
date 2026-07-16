using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Veiculo;

namespace Ofichina.Application.UseCases.Veiculos.Queries;

/// <summary>
/// Consulta para obter um veículo por Id.
/// </summary>
public sealed class GetVeiculoByIdQuery : IQuery<Result<VeiculoResponse>>
{
    public Guid Id { get; init; }

    public GetVeiculoByIdQuery(Guid id)
    {
        Id = id;
    }
}