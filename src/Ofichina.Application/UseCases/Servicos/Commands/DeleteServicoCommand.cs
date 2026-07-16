using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;

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
}