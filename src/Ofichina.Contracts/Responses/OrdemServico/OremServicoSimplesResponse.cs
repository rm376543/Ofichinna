using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Veiculo;

namespace Ofichina.Contracts.Responses.OrdemServico
{
    public sealed class OrdemServicoSimplesResponse : BaseResponse
    {
        public Guid OrdemServicoSimplesId { get; set; }

        public string Cliente { get; set; } = string.Empty;
        public string Funcionario { get; set; } = string.Empty;
        public string ProblemaRelatado { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string DataAbetura { get; set; } = string.Empty;
        public string DataFinalizacao { get; set; } = string.Empty;
        public string? Observacao { get; set; } = string.Empty;
        public string? ValorTotal { get; set; } = string.Empty;
    }
}
