using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Pessoa;
using Ofichina.Domain.Common;

namespace Ofichina.Application.UseCases.Pessoas.Queries;

/// <summary>
/// Query para listar pessoas.
/// </summary>
public sealed class GetPessoasQuery : IQuery<Result<IReadOnlyCollection<PessoaResponse>>>
{
    public Pagination Pagination { get; init; } = new();
}
