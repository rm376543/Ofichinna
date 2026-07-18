using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;

namespace Ofichina.Application.UseCases.OrdensServico.ItemPeca.Commands;

/// <summary>
/// Comando para marcar um item de peça como utilizado na ordem de serviço.
/// </summary>
public sealed class UtilizarItemPecaCommand : ICommand<Result>
{
    /// <summary>
    /// Identificador da ordem de serviço.
    /// </summary>
    public Guid OrdemServicoId { get; init; }

    /// <summary>
    /// Identificador do item de peça.
    /// </summary>
    public Guid Id { get; init; }
}
