using Ofichina.Contracts.Requests;

namespace Ofichina.Contracts.Responses.Agendamento;

/// <summary>
/// Resposta com os dados de um dia de disponibilidade.
/// </summary>
public sealed class DiaDisponibilidadeResponse : BaseRequest
{
    public Guid DiaDisponibilidadeId { get; set; }
    public DateOnly Data { get; set; }
}