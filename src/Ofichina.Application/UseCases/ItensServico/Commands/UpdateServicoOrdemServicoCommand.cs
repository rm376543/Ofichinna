using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.ItensServico;

namespace Ofichina.Application.UseCases.ItensServico.Commands;

/// <summary>
/// Comando para atualização de um item de serviço somente-serviço em uma ordem de serviço.
/// </summary>
public sealed class UpdateServicoOrdemServicoCommand : ICommand<Result>
{
    public Guid ItemServicoId { get; init; }

    public Guid OrdemServicoId { get; init; }

    public Guid ServicoId { get; init; }

    public UpdateServicoOrdemServicoCommand(UpdateServicoOrdemServicoRequest request)
    {
        ItemServicoId = request.ItemServicoId;
        OrdemServicoId = request.OrdemServicoId;
        ServicoId = request.ServicoId;
    }
}