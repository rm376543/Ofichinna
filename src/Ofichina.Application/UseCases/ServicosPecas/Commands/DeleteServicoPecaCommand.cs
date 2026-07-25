using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;

namespace Ofichina.Application.UseCases.ServicosPecas.Commands;

/// <summary>
/// Comando para desativar uma peça vinculada a um serviço.
/// </summary>
public sealed class DeleteServicoPecaCommand : ICommand<Result>
{
    /// <summary>
    /// Identificador do serviço.
    /// </summary>
    public Guid ServicoId { get; init; }

    /// <summary>
    /// Identificador da peça do serviço.
    /// </summary>
    public Guid PecaId { get; init; }
}