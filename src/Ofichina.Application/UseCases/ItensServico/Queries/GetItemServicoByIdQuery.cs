using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.OrdemServico;

namespace Ofichina.Application.UseCases.ItensServico.Queries;

/// <summary>
/// Query para obter um item de serviço por identificador.
/// </summary>
public sealed class GetItemServicoByIdQuery : IQuery<Result<OrdemServicoItensResponse>>
{
    /// <summary>
    /// Identificador da ordem de serviço.
    /// </summary>
    public Guid OrdemServicoId { get; init; }

    /// <summary>
    /// Identificador do item de serviço.
    /// </summary>
    public Guid Id { get; init; }
}
