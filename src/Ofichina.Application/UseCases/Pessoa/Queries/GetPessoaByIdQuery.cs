using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Pessoa;

namespace Ofichina.Application.UseCases.Pessoas.Queries;

/// <summary>
/// Query para obter uma pessoa por identificador.
/// </summary>
public sealed class GetPessoaByIdQuery : IQuery<Result<PessoaResponse>>
{
    /// <summary>
    /// Identificador da pessoa.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Cria uma nova query por Id.
    /// </summary>
    public GetPessoaByIdQuery(Guid id)
    {
        Id = id;
    }
}
