using Ofichina.Contracts.Common;

namespace Ofichina.Contracts.Requests.OrdemServico
{
    public class BuscarOrdemServicoPorIdRequest : BaseRequest
    {
        public Guid OrdemServicoId { get; set; }
    }
}
