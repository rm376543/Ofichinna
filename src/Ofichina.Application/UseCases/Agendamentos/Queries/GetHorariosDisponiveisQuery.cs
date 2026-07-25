using Ofichina.Application.Abstractions;
using Ofichina.Contracts;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Agendamento;

namespace Ofichina.Application.UseCases.Agendamentos.Queries
{
    /// <summary>
    /// Representa uma consulta para obter horários disponíveis para agendamento.
    /// </summary>
    public class GetHorariosDisponiveisQuery : IQuery<Result<PagedResponse<HorarioDisponivelResponse>>>
    {
        public Pagination Pagination { get; }

        public GetHorariosDisponiveisQuery(Pagination pagination)
        {
            Pagination = pagination;
        }
    }
}