using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;

namespace Ofichina.Application.UseCases.OrdensServico.ItemServico.Commands;

/// <summary>
/// Comando para remoção de um item de serviço de uma ordem de serviço.
/// </summary>
public sealed class DeleteItemServicoCommand : ICommand<Result>
{
    /// <summary>
    /// Identificador da ordem de serviço.
    /// </summary>
    public Guid OrdemServicoId { get; init; }

    /// <summary>
    /// Identificador do item de serviço.
    /// </summary>
    public Guid Id { get; init; }
}
