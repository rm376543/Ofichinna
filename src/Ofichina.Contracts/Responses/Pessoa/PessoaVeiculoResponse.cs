using Ofichina.Contracts.Responses.Veiculo;

namespace Ofichina.Contracts.Responses.Pessoa
{
    public class PessoaVeiculoResponse : PessoaSimplesResponse
    {
        public List<VeiculoResponse> Veiculo { get; set; } = new List<VeiculoResponse>();
    }
}
