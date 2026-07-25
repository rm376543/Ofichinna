namespace Ofichina.Contracts.Responses.Agendamento
{
    /// <summary>
    /// Representa a solicitação para cancelar um agendamento existente.
    /// </summary>
    public sealed class CancelarAgendamentoRequest
    {
        public Guid PessoaId { get; set; }
        public Guid AgendamentoId { get; set; }
    }
}
