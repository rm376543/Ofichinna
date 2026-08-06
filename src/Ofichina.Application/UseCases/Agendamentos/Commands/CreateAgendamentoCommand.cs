using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Agendamento;

namespace Ofichina.Application.UseCases.Agendamentos.Commands;

/// <summary>
/// Comando para criação de agendamento usando o novo modelo com HorarioConsultorDisponibilidade.
/// </summary>
public sealed class CreateAgendamentoCommand : ICommand<Result<AgendamentoResponse>>
{
    /// <summary>
    /// Identificador único do agendamento.
    /// </summary>
    public Guid PessoaId { get; init; }
    /// <summary>
    /// ID do slot de disponibilidade (HorarioConsultorDisponibilidade) que consolida dia + horário + consultor.
    /// </summary>
    public Guid HorarioConsultorDisponibilidadeId { get; init; }

    /// <summary>
    /// Identificador do veículo a ser atendido.
    /// </summary>
    public Guid VeiculoId { get; init; }

    /// <summary>
    /// Descrição opcional do agendamento.
    /// </summary>
    public string? Descricao { get; init; }

    public CreateAgendamentoCommand(
        Guid pessoaId,
        Guid horarioConsultorDisponibilidadeId,
        Guid veiculoId,
        string? descricao)
    {
        PessoaId = pessoaId;
        HorarioConsultorDisponibilidadeId = horarioConsultorDisponibilidadeId;
        VeiculoId = veiculoId;
        Descricao = descricao;
    }
}