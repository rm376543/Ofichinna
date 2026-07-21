using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;

namespace Ofichina.Application.UseCases.OrdensServico.Peca.Commands;

/// <summary>
/// Comando para marcar uma peça vinculada ao serviço como utilizada na ordem de serviço.
/// </summary>
public sealed class UtilizarPecaCommand : ICommand<Result>
{
    /// <summary>
    /// Identificador da ordem de serviço.
    /// </summary>
    public Guid OrdemServicoId { get; init; }

    /// <summary>
    /// Identificador do item de serviço.
    /// </summary>
    public Guid ItemServicoId { get; init; }

    /// <summary>
    /// Identificador da peça vinculada ao serviço.
    /// </summary>
    public Guid Id { get; init; }
}
