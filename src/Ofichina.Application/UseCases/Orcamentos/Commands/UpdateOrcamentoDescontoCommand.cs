using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;

namespace Ofichina.Application.UseCases.Orcamentos.Commands;

/// <summary>
/// Comando para atualização de desconto de orçamento.
/// </summary>
public sealed class UpdateOrcamentoDescontoCommand : ICommand<Result>
{
    public Guid OrcamentoId { get; init; }

    public decimal Desconto { get; init; }

    public UpdateOrcamentoDescontoCommand(Guid orcamentoId, decimal desconto)
    {
        OrcamentoId = orcamentoId;
        Desconto = desconto;
    }
}
