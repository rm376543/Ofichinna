using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses;
using Ofichina.Contracts.Responses.Veiculo;

namespace Ofichina.Application.UseCases.Veiculos.Queries
{
    /// <summary>
    /// Consulta para obter veículos atravez do Id da pessoa.
    /// </summary>
    public sealed class GetVeiculosByPessoaIdQuery : IQuery<Result<PagedResponse<VeiculoListResponse>>>
    {
        public Guid PessoaId { get; }
        public int PageNumber { get; }
        public int PageSize { get; }

        public GetVeiculosByPessoaIdQuery(Guid pessoaId, int pageNumber = 1, int pageSize = 10)
        {
            PessoaId = pessoaId;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}
