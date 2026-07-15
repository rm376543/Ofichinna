using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Pessoa;

namespace Ofichina.Application.UseCases.Pessoas.Queries;

/// <summary>
/// Query para listar pessoas.
/// </summary>
public sealed class GetPessoasQuery : IQuery<Result<IReadOnlyCollection<PessoaResponse>>>
{
}
