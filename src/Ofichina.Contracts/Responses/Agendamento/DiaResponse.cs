namespace Ofichina.Contracts.Responses.Agendamento
{
    /// <summary>
    /// DTO com informações do dia disponível.
    /// </summary>
    public sealed class DiaResponse
    {
        public Guid DiaId { get; set; }
        public string Data { get; set; } = string.Empty; // YYYY-MM-DD
    }

}
