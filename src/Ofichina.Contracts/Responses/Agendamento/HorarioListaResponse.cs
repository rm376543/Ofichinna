namespace Ofichina.Contracts.Responses.Agendamento
{
    /// <summary>
    /// DTO com informações do horário.
    /// </summary>
    public sealed class HorarioListaResponse
    {
        public Guid Id { get; set; }
        public string Hora { get; set; } = string.Empty; // HH:mm
        public bool Disponivel { get; set; }
    }

}
