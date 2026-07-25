using Ofichina.Contracts.Common;

namespace Ofichina.Contracts.Responses.Agendamento
{
    /// <summary>
    /// Representa a resposta para a consulta de horários disponíveis para agendamento.
    /// </summary>
    public sealed class HorarioDisponivelResponse : BaseEntity
    {
        public TimeOnly Horario { get; set; }
        public bool Disponivel { get; set; } = true;
    }
}
