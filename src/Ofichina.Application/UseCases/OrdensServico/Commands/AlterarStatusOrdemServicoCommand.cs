using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;

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
    public string StatusDestino { get; init; }

    public AlterarStatusOrdemServicoCommand(Guid ordemServicoId, string statusDestino)
    {
        Id = ordemServicoId;
        StatusDestino = statusDestino;
    }
}
