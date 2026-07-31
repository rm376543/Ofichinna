using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;

namespace Ofichina.Application.UseCases.ItensServico.Commands;

/// <summary>
/// Comando para atualização de um item de serviço em uma ordem de serviço.
/// </summary>
public sealed class UpdateItemServicoCommand : ICommand<Result>
{
    /// <summary>
    /// Identificador do item de serviço.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Identificador da ordem de serviço.
    /// </summary>
    public Guid OrdemServicoId { get; init; }

    /// <summary>
    /// Identificador do serviço executado.
    /// </summary>
    public Guid ServicoId { get; init; }

    /// <summary>
    /// Identificador da peça utilizada.
    /// </summary>
    public Guid PecaId { get; init; }

    /// <summary>
    /// Quantidade de peças utilizadas.
    /// </summary>
    public int Quantidade { get; init; }
}
