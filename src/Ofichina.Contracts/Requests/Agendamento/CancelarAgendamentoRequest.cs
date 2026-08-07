namespace Ofichina.Contracts.Requests.Agendamento
{
    /// <summary>
    /// Representa a solicitação para cancelar um agendamento existente.
    /// </summary>
    public sealed class CancelarAgendamentoRequest
    {
        public Guid AgendamentoId { get; set; }
    }
}
