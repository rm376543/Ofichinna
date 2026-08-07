using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;

namespace Ofichina.Application.UseCases.Agendamentos.Commands;

/// <summary>
/// Comando para iniciar um agendamento existente.
/// </summary>
public sealed class IniciarAgendamentoCommand : ICommand<Result>
{
    public Guid AgendamentoId { get; init; }

    public IniciarAgendamentoCommand(Guid agendamentoId)
    {
        AgendamentoId = agendamentoId;
    }
}
