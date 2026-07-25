using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Pessoa;

namespace Ofichina.Application.UseCases.Veiculos.Queries
{
    /// <summary>
    /// Consulta para obter veículos atravez do Id da pessoa.
    /// </summary>
    public sealed class GetVeiculosByPessoaIdQuery : IQuery<Result<PessoaVeiculoResponse>>
    {
        public Guid PessoaId { get; }

        public GetVeiculosByPessoaIdQuery(Guid pessoaId)
        {
            PessoaId = pessoaId;
        }
    }
}
