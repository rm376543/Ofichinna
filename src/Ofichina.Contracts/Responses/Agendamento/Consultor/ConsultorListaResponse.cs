namespace Ofichina.Contracts.Responses.Agendamento.Consultor
{

    /// <summary>
    /// DTO com informações do consultor.
    /// </summary>
    public sealed class ConsultorListaResponse
    {
        public Guid PessoaId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Documento { get; set; } = string.Empty;
    }
}
