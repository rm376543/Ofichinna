using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;

namespace Ofichina.Application.UseCases.OrdensServico.ItemServico.Commands;

/// <summary>
/// Comando para adicionar uma peça a um item de serviço.
/// </summary>
public sealed class CreateItemServicoPecaCommand : ICommand<Result<Guid>>
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
    /// Identificador da peça cadastrada.
    /// </summary>
    public Guid PecaId { get; init; }

    /// <summary>
    /// Quantidade utilizada.
    /// </summary>
    public int Quantidade { get; init; }
}