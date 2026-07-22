using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;

namespace Ofichina.Application.UseCases.ServicosPecas.Commands;

/// <summary>
/// Comando para adicionar uma peça a um serviço.
/// </summary>
public sealed class CreateServicoPecaCommand : ICommand<Result<Guid>>
{
    /// <summary>
    /// Identificador do serviço.
    /// </summary>
    public Guid ServicoId { get; init; }

    /// <summary>
    /// Identificador da peça.
    /// </summary>
    public Guid PecaId { get; init; }

    /// <summary>
    /// Quantidade da peça no serviço.
    /// </summary>
    public int Quantidade { get; init; }
}