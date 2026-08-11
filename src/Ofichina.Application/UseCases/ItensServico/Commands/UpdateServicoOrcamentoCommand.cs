using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.ItensServico;

namespace Ofichina.Application.UseCases.ItensServico.Commands;

/// <summary>
/// Comando para atualização de um item de serviço somente-serviço em um orçamento.
/// </summary>
public sealed class UpdateServicoOrcamentoCommand : ICommand<Result>
{
    public Guid ItemServicoId { get; init; }

    public Guid OrcamentoId { get; init; }

    public Guid ServicoId { get; init; }

    public UpdateServicoOrcamentoCommand(UpdateServicoOrcamentoRequest request)
    {
        ItemServicoId = request.ItemServicoId;
        OrcamentoId = request.OrcamentoId;
        ServicoId = request.ServicoId;
    }
}