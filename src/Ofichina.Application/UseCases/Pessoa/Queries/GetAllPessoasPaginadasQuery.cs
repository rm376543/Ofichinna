using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Pessoa;
using Ofichina.Domain.Common;

namespace Ofichina.Application.UseCases.Pessoas.Queries;

/// <summary>
/// Query para listar pessoas.
/// </summary>
public sealed class GetAllPessoasPaginadasQuery : IQuery<Result<PagedResponse<PessoaResponse>>>
{
    public Pagination Pagination { get; }

    public GetAllPessoasPaginadasQuery(Pagination pagination)
    {
        Pagination = pagination;
    }
}
