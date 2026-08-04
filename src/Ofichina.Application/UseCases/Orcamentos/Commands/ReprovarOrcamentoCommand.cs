using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;

namespace Ofichina.Application.UseCases.Orcamentos.Commands;

/// <summary>
/// Comando para reprovar um orçamento.
/// </summary>
public sealed class ReprovarOrcamentoCommand : ICommand<Result>
{
    public Guid Id { get; init; }

    public string? Motivo { get; init; }
}
