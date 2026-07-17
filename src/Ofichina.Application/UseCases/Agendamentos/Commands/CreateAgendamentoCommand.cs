using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Agendamento;

namespace Ofichina.Application.UseCases.Agendamentos.Commands;

/// <summary>
/// Comando para criação de agendamento.
/// </summary>
public sealed class CreateAgendamentoCommand : ICommand<Result<AgendamentoResponse>>
{
    public Guid VeiculoId { get; init; }

    public DateTime DataHoraPreferida { get; init; }

    public string Motivo { get; init; } = string.Empty;

    public string? Observacoes { get; init; }
}