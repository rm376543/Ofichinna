namespace Ofichina.Contracts.Responses.Pessoa
{
    public class PessoaSimplesResponse
    {
        public Guid PessoaId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
    }
}
