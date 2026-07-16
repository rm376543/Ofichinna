using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;

namespace Ofichina.Application.UseCases.OrdensServico.Commands;

/// <summary>
/// Comando para remoção lógica de uma ordem de serviço.
/// </summary>
public sealed class DeleteOrdemServicoCommand : ICommand<Result>
{
    /// <summary>
    /// Identificador da ordem de serviço.
    /// </summary>
    public Guid Id { get; init; }
}
