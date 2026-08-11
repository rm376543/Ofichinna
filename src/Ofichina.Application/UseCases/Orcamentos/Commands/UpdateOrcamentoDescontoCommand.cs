using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.Orcamento;

namespace Ofichina.Application.UseCases.Orcamentos.Commands;

/// <summary>
/// Comando para atualização de desconto de orçamento.
/// </summary>
public sealed class UpdateOrcamentoDescontoCommand : ICommand<Result>
{
    public Guid OrcamentoId { get; init; }

    public decimal Desconto { get; init; }

    public bool DescontoEmDinheiro { get; init; }

    public UpdateOrcamentoDescontoCommand(UpdateOrcamentoDescontoRequest request)
    {
        OrcamentoId = request.OrcamentoId;
        Desconto = request.Desconto;
        DescontoEmDinheiro = request.DescontoEmDinheiro;
    }
}
