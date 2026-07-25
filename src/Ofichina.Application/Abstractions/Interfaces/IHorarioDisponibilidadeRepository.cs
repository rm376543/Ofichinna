using Ofichina.Contracts;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Entities;

namespace Ofichina.Application.Abstractions.Interfaces;

public interface IHorarioDisponibilidadeRepository : IRepository<HorarioDisponibilidade>
{
    Task<IReadOnlyCollection<HorarioDisponibilidade>> GetHorariosPorDiaAsync(Guid diaDisponibilidadeId, CancellationToken cancellationToken = default);

    Task<PagedResponse<HorarioDisponibilidade>> GetHorariosDisponiveisPaginados(Pagination pagination, CancellationToken cancellationToken = default);

    Task<HorarioDisponibilidade?> BuscarPorHorarioAsync(TimeOnly horario, CancellationToken cancellationToken = default);
}