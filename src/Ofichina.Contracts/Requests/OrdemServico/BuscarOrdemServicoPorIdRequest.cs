namespace Ofichina.Contracts.Requests.OrdemServico
{
    public class BuscarOrdemServicoPorIdRequest : BaseRequest
    {
        public Guid Id { get; set; }
    }
}
