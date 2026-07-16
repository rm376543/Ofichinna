using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Aggregates;

namespace Ofichina.Application.UseCases.OrdensServico.Commands;

/// <summary>
/// Comando para alteração de status da ordem de serviço.
/// </summary>
public sealed class AlterarStatusOrdemServicoCommand : ICommand<Result>
{
    /// <summary>
    /// Identificador da ordem de serviço.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Status de destino da ordem de serviço.
    /// </summary>
    public StatusOrdemServico StatusDestino { get; init; }
}
