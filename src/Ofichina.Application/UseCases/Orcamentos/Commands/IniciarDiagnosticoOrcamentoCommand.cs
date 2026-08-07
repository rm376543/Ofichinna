using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;

namespace Ofichina.Application.UseCases.Orcamentos.Commands;

/// <summary>
/// Comando para iniciar o diagnóstico de um orçamento.
/// </summary>
public sealed class IniciarDiagnosticoOrcamentoCommand : ICommand<Result>
{
    public Guid Id { get; init; }

    public IniciarDiagnosticoOrcamentoCommand(Guid orcamentoId)
    {
        Id = orcamentoId;
    }
}
