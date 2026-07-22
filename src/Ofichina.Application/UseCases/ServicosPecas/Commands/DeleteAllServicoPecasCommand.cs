using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;

namespace Ofichina.Application.UseCases.ServicosPecas.Commands;

/// <summary>
/// Comando para desativar todas as peças vinculadas a um serviço.
/// </summary>
public sealed class DeleteAllServicoPecasCommand : ICommand<Result>
{
    /// <summary>
    /// Identificador do serviço.
    /// </summary>
    public Guid ServicoId { get; init; }
}