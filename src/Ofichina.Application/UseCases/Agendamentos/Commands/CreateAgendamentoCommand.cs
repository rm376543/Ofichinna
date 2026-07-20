using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Agendamento;

namespace Ofichina.Application.UseCases.Agendamentos.Commands;

/// <summary>
/// Comando para criação de agendamento.
/// </summary>
public sealed class CreateAgendamentoCommand : ICommand<Result<AgendamentoResponse>>
{
    public Guid PessoaId { get; init; }

    public Guid DiaDisponibilidadeId { get; init; }

    public Guid HorarioConsultorId { get; init; }

    public Guid VeiculoId { get; init; }

    public string? Descricao { get; init; }
}