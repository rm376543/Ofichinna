namespace Ofichina.Application;

using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Agendamento;

/// <summary>
/// Busca todos os agendamentos cadastrados no sistema, de forma paginada.
/// </summary>
public sealed class GetAllAgendamentosPaginadosQuery : IQuery<Result<PagedResponse<AgendamentoUsuarioResponse>>>
{
    public Pagination Pagination { get; set; }

    public GetAllAgendamentosPaginadosQuery(Pagination pagination)
    {
        Pagination = pagination;

    }
}

