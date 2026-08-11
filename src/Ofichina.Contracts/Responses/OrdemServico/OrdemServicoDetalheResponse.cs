using Ofichina.Contracts.Common;

namespace Ofichina.Contracts.Responses.OrdemServico
{
    public sealed class OrdemServicoDetalheResponse : BaseResponse
    {
        public Guid OrdemServicoId { get; set; }
        public string Cliente { get; set; } = string.Empty;
        public string Consultor { get; set; } = string.Empty;
        public string ProblemaRelatado { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string DataAbetura { get; set; } = string.Empty;
        public string DataFinalizacao { get; set; } = string.Empty;
        public string? Observacao { get; set; } = string.Empty;
        public string? ValorTotal { get; set; } = string.Empty;
    }
}
