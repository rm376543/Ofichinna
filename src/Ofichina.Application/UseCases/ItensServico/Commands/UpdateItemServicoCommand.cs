using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.ItensServico;

namespace Ofichina.Application.UseCases.ItensServico.Commands;

/// <summary>
/// Comando para atualização de um item de serviço em uma ordem de serviço.
/// </summary>
public sealed class UpdateItemServicoCommand : ICommand<Result>
{
    /// <summary>
    /// Identificador do item de serviço.
    /// </summary>
    public Guid ItemServicoId { get; init; }

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

    public UpdateItemServicoCommand(UpdateItemServicoRequest request)
    {
        ItemServicoId = request.ItemServicoId;
        OrdemServicoId = request.OrdemServicoId;
        ServicoId = request.ServicoId;
        PecaId = request.PecaId;
        Quantidade = request.Quantidade;
    }
}
