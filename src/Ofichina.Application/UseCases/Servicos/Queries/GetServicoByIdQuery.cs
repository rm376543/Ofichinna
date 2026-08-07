using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Servicos;

namespace Ofichina.Application.UseCases.Servicos.Queries;

/// <summary>
/// Consulta para obter um serviço por Id.
/// </summary>
public sealed class GetServicoByIdQuery : IQuery<Result<ServicoResponse>>
{
    /// <summary>
    /// Identificador do serviço.
    /// </summary>
    public Guid Id { get; init; }

    public GetServicoByIdQuery(Guid servicoId)
    {
        Id = servicoId;
    }
}