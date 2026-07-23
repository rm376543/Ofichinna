using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;

namespace Ofichina.Application.UseCases.ItensServico.Commands;

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
    /// Peças vinculadas ao item.
    /// </summary>
    public IReadOnlyCollection<UpdateItemServicoPecaCommand> Pecas { get; init; } = [];
}

/// <summary>
/// Dados de uma peça informada na atualização do item.
/// </summary>
public sealed class UpdateItemServicoPecaCommand
{
    public Guid ServicoPecaId { get; init; }
    public int Quantidade { get; init; }
}
