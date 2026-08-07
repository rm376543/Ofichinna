using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Servicos;

namespace Ofichina.Application.UseCases.Servicos.Commands;

/// <summary>
/// Comando para exclusão lógica de serviço.
/// </summary>
public sealed class DeleteServicoCommand : ICommand<Result>
{
    /// <summary>
    /// Identificador do serviço.
    /// </summary>
    public Guid Id { get; init; }

    public DeleteServicoCommand(RemoveServicoRequest request)
    {
        Id = request.ServicoId;
    }
}