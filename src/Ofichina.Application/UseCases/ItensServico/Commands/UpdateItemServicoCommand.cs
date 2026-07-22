using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;

namespace Ofichina.Application.UseCases.ItemServico.Commands;

/// <summary>
/// Comando para atualização de um item de serviço em uma ordem de serviço.
/// </summary>
public sealed class UpdateItemServicoCommand : ICommand<Result>
{
    /// <summary>
    /// Identificador da ordem de serviço.
    /// </summary>
    public Guid OrdemServicoId { get; init; }

    /// <summary>
    /// Identificador do item de serviço.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Identificador da peça de serviço vinculada.
    /// </summary>
    public Guid PecaServicoId { get; init; }
}
