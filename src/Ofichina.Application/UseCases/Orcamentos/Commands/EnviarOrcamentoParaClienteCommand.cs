using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;

namespace Ofichina.Application.UseCases.Orcamentos.Commands;

/// <summary>
/// Comando para enviar o orçamento ao cliente.
/// </summary>
public sealed class EnviarOrcamentoParaClienteCommand : ICommand<Result>
{
    public Guid Id { get; init; }

    public EnviarOrcamentoParaClienteCommand(Guid orcamentoId)
    {
        Id = orcamentoId;
    }
}
