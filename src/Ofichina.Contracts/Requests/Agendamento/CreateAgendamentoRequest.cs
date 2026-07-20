using Ofichina.Contracts.Requests;

namespace Ofichina.Contracts.Requests.Agendamento;

/// <summary>
/// Requisição para criação de agendamento pelo aplicativo.
/// </summary>
public sealed class CreateAgendamentoRequest : CreateRequest
{
    /// <summary>
    /// Identificador do dia disponível selecionado.
    /// </summary>
    public Guid DiaDisponibilidadeId { get; init; }

    /// <summary>
    /// Identificador do vínculo entre horário e consultor selecionado.
    /// </summary>
    public Guid HorarioConsultorId { get; init; }

    /// <summary>
    /// Identificador do veículo a ser atendido.
    /// </summary>
    public Guid VeiculoId { get; init; }

    /// <summary>
    /// Descrição opcional do agendamento.
    /// </summary>
    public string? Descricao { get; init; }
}