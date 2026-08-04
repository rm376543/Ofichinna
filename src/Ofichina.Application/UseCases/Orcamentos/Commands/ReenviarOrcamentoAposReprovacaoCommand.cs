using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;

namespace Ofichina.Application.UseCases.Orcamentos.Commands;

/// <summary>
/// Comando para reenviar um orçamento após reprovação.
/// </summary>
public sealed class ReenviarOrcamentoAposReprovacaoCommand : ICommand<Result>
{
    public Guid Id { get; init; }
}