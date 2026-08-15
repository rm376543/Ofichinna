using Ofichina.Contracts.Common;

namespace Ofichina.Contracts.Responses.Agendamento;

/// <summary>
/// Resposta com os dados de um dia de disponibilidade.
/// </summary>
public sealed class DiaDisponibilidadeResponse : BaseResponse
{
    public Guid DiaId { get; set; }
    public string Dia { get; set; } = string.Empty;
}