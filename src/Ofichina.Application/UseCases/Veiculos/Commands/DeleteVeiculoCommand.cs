using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Veiculo;

namespace Ofichina.Application.UseCases.Veiculos.Commands;

/// <summary>
/// Comando para remoção lógica de veículo.
/// </summary>
public sealed class DeleteVeiculoCommand : ICommand<Result>
{
    public Guid VeiculoId { get; init; }

    public DeleteVeiculoCommand(RemoveVeiculoRequest request)
    {
        VeiculoId = request.VeiculoId;
    }
}