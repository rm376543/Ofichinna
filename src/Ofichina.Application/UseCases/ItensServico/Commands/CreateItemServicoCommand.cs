using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;

namespace Ofichina.Application.UseCases.ItensServico.Commands;

/// <summary>
/// Comando para criação de um item de serviço em uma ordem de serviço.
/// </summary>
public sealed class CreateItemServicoCommand : ICommand<Result<Guid>>
{
    /// <summary>
    /// Identificador da ordem de serviço.
    /// </summary>
    public Guid OrdemServicoId { get; init; }

    /// <summary>
    /// Peças vinculadas ao item.
    /// </summary>
    public IReadOnlyCollection<CreateItemServicoPecaCommand> Pecas { get; init; } = [];
}

/// <summary>
/// Dados de uma peça informada na criação do item.
/// </summary>
public sealed class CreateItemServicoPecaCommand
{
    public Guid ServicoPecaId { get; init; }
    public int Quantidade { get; init; }
}
