namespace Ofichina.Contracts.Responses.Agendamento
{

    /// <summary>
    /// DTO com slot de agenda do consultor.
    /// </summary>
    public sealed class AgendaSlotResponse
    {
        public Guid SlotId { get; set; }
        public string Hora { get; set; } = string.Empty; // HH:mm
        public string Status { get; set; } = "VAGO"; // VAGO, AGENDADO, INICIADO, FINALIZADO, CANCELADO
        public string? ClienteNome { get; set; }
        public string? Veiculo { get; set; }
    }
}
