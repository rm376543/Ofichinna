using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;

namespace Ofichina.Application.UseCases.Orcamentos.Commands;

/// <summary>
/// Comando para aprovar um orçamento e gerar a ordem de serviço.
/// </summary>
public sealed class AprovarOrcamentoCommand : ICommand<Result>
{
    public Guid Id { get; init; }

    public Guid MecanicoReparoId { get; init; }
}
