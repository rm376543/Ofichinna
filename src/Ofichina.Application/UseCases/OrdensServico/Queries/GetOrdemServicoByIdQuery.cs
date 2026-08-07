using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.OrdensServico;

namespace Ofichina.Application.UseCases.OrdensServico.Queries;

/// <summary>
/// Consulta para obter uma ordem de serviço pelo identificador.
/// </summary>
public sealed class GetOrdemServicoByIdQuery : IQuery<Result<OrdemServicoResponse>>
{
    /// <summary>
    /// Identificador da ordem de serviço.
    /// </summary>
    public Guid Id { get; init; }

    public GetOrdemServicoByIdQuery(Guid ordemServicoId)
    {
        Id = ordemServicoId;
    }
}
