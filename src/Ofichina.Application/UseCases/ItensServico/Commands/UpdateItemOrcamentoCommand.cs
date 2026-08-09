using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.ItensServico;

namespace Ofichina.Application.UseCases.ItensServico.Commands;

/// <summary>
/// Comando para atualização de um item de serviço em um orçamento.
/// </summary>
public sealed class UpdateItemOrcamentoCommand : ICommand<Result>
{
    public Guid ItemServicoId { get; init; }

    public Guid OrcamentoId { get; init; }

    public Guid ServicoId { get; init; }

    public Guid? PecaId { get; init; }

    public int Quantidade { get; init; }

    public UpdateItemOrcamentoCommand(UpdateItemOrcamentoRequest request)
    {
        ItemServicoId = request.ItemServicoId;
        OrcamentoId = request.OrcamentoId;
        ServicoId = request.ServicoId;
        PecaId = request.PecaId;
        Quantidade = request.Quantidade;
    }
}
