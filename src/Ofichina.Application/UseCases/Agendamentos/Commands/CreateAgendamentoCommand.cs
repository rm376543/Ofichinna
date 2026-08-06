using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Agendamento;

namespace Ofichina.Application.UseCases.Agendamentos.Commands;

/// <summary>
/// Comando para criação de agendamento usando o novo modelo com HorarioConsultorDisponibilidade.
/// </summary>
public sealed class CreateAgendamentoCommand : ICommand<Result<AgendamentoResponse>>
{
    public Guid PessoaId { get; init; }

    /// <summary>
    /// ID do slot de disponibilidade (HorarioConsultorDisponibilidade) que consolida dia + horário + consultor.
    /// </summary>
    public Guid HorarioConsultorDisponibilidadeId { get; init; }

    public Guid VeiculoId { get; init; }

    public string? Descricao { get; init; }
}