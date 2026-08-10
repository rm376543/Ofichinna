using Ofichina.Contracts.Common;

namespace Ofichina.Contracts.Responses.Agendamento;

/// <summary>
/// Resposta com os dados de um dia de disponibilidade.
/// </summary>
public sealed class DiaDisponibilidadeResponse : BaseRequest
{
    public Guid DiaId { get; set; }
    public DateOnly Dia { get; set; }
}