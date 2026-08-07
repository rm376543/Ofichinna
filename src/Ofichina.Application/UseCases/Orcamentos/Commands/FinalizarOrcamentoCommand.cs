using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;

namespace Ofichina.Application.UseCases.Orcamentos.Commands;

/// <summary>
/// Comando para finalizar o orçamento.
/// </summary>
public sealed class FinalizarOrcamentoCommand : ICommand<Result>
{
    public Guid Id { get; init; }

    public FinalizarOrcamentoCommand(Guid orcamentoId)
    {
        Id = orcamentoId;
    }
}
