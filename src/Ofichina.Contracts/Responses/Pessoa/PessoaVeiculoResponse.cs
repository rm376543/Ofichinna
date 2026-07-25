using Ofichina.Contracts.Responses.Veiculo;

namespace Ofichina.Contracts.Responses.Pessoa
{
    public class PessoaVeiculoResponse : PessoaResponse
    {
        public List<VeiculoResponse> Veiculo { get; set; } = new List<VeiculoResponse>();
    }
}
