using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;

namespace Ofichina.Application.UseCases.Agendamentos.Commands;

/// <summary>
/// Comando para criação de agendamento usando o novo modelo com AgendaConsultor.
/// </summary>
public sealed class CreateAgendamentoCommand : ICommand<Result>
{
    /// <summary>
    /// Identificador único do agendamento.
    /// </summary>
    public Guid PessoaId { get; init; }
    /// <summary>
    /// ID do slot de disponibilidade (AgendaConsultor) que consolida dia + horário + consultor.
    /// </summary>
    public Guid AgendaConsultorId { get; init; }

    /// <summary>
    /// Identificador do veículo a ser atendido.
    /// </summary>
    public Guid VeiculoId { get; init; }

    /// <summary>
    /// Hodômetro de entrada informado no agendamento.
    /// </summary>
    public int Hodometro { get; init; }

    /// <summary>
    /// Descrição opcional do agendamento.
    /// </summary>
    public string? Descricao { get; init; }

    public CreateAgendamentoCommand(
        Guid pessoaId,
        Guid agendaConsultorId,
        Guid veiculoId,
        int hodometro,
        string? descricao)
    {
        PessoaId = pessoaId;
        AgendaConsultorId = agendaConsultorId;
        VeiculoId = veiculoId;
        Hodometro = hodometro;
        Descricao = descricao;
    }
}