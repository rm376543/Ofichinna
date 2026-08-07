using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;

namespace Ofichina.Application.UseCases.Pecas.Commands;

/// <summary>
/// Comando para exclusão lógica de peça.
/// </summary>
public sealed class DeletePecaCommand : ICommand<Result>
{
    /// <summary>
    /// Identificador da peça.
    /// </summary>
    public Guid PecaId { get; init; }

    public DeletePecaCommand(Guid pecaId)
    {
        PecaId = pecaId;
    }
}