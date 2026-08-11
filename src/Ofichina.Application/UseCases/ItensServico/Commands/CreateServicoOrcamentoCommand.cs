using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.ItensServico;

namespace Ofichina.Application.UseCases.ItensServico.Commands;

/// <summary>
/// Comando para criação de um item de serviço somente-serviço em um orçamento.
/// </summary>
public sealed class CreateServicoOrcamentoCommand : ICommand<Result>
{
    public Guid OrcamentoId { get; init; }

    public Guid ServicoId { get; init; }

    public CreateServicoOrcamentoCommand(CreateServicoOrcamentoRequest request)
    {
        OrcamentoId = request.OrcamentoId;
        ServicoId = request.ServicoId;
    }
}