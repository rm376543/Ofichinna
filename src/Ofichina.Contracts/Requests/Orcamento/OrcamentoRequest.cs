namespace Ofichina.Contracts.Requests.Orcamento
{
    public class OrcamentoRequest
    {
        public Guid OrcamentoId { get; set; }

        public OrcamentoRequest(Guid orcamentoId)
        {
            OrcamentoId = orcamentoId;
        }
    }
}
