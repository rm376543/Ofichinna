using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.ItensServico;

namespace Ofichina.Application.UseCases.ItensServico.Commands;

/// <summary>
/// Comando para remoção de um item de serviço de uma ordem de serviço.
/// </summary>
public sealed class DeleteItemServicoCommand : ICommand<Result>
{
    /// <summary>
    /// Identificador do item de serviço.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Identificador da ordem de serviço.
    /// </summary>
    public Guid OrdemServicoId { get; init; }

    public DeleteItemServicoCommand(DeleteItemServicoRequest request)
    {
        Id = request.Id;
        OrdemServicoId = request.OrdemServicoId;
    }
}
