using Ofichina.Contracts.Requests;

namespace Ofichina.Contracts.Responses.Agendamento
{
    /// <summary>
    /// Representa a resposta para a consulta de horários disponíveis para agendamento.
    /// </summary>
    public sealed class HorarioDisponivelResponse : BaseRequest
    {
    public Guid HorarioDisponivelId { get; set; }
        public TimeOnly Horario { get; set; }
        public bool Disponivel { get; set; } = true;

    }
}
