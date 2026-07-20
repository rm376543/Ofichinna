using Ofichina.Contracts.Requests;

namespace Ofichina.Contracts.Requests.Agendamento;

/// <summary>
/// Requisição para cadastrar um dia de disponibilidade com horários.
/// </summary>
public sealed class CreateDiaDisponibilidadeRequest : CreateRequest
{
    /// <summary>
    /// Data disponível para atendimento.
    /// </summary>
    public DateOnly Data { get; init; }

    /// <summary>
    /// Horários disponíveis no dia informado.
    /// </summary>
    public IReadOnlyCollection<TimeOnly> Horarios { get; init; } = [];
}