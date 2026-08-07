using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.Orcamento;

namespace Ofichina.Application.UseCases.Orcamentos.Commands;

/// <summary>
/// Comando para reprovar um orçamento.
/// </summary>
public sealed class ReprovarOrcamentoCommand : ICommand<Result>
{
    public Guid Id { get; init; }

    public string? Motivo { get; init; }

    public ReprovarOrcamentoCommand(ReprovarOrcamentoRequest request)
    {
        Id = request.OrcamentoId;
        Motivo = request.Motivo;
    }
}
