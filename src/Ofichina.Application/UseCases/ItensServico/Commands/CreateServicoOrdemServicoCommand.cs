using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.ItensServico;

namespace Ofichina.Application.UseCases.ItensServico.Commands;

/// <summary>
/// Comando para criação de um item de serviço somente-serviço em uma ordem de serviço.
/// </summary>
public sealed class CreateServicoOrdemServicoCommand : ICommand<Result>
{
    public Guid OrdemServicoId { get; init; }

    public Guid ServicoId { get; init; }

    public CreateServicoOrdemServicoCommand(CreateServicoOrdemServicoRequest request)
    {
        OrdemServicoId = request.OrdemServicoId;
        ServicoId = request.ServicoId;
    }
}