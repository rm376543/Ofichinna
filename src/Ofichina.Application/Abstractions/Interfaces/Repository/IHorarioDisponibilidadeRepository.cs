using Ofichina.Domain.Entities;

namespace Ofichina.Application.Abstractions.Interfaces.Repository;

public interface IHorarioDisponibilidadeRepository : IRepository<HorarioDisponibilidade>
{
    Task<IReadOnlyCollection<HorarioDisponibilidade>> GetHorariosPorDiaAsync(Guid diaDisponibilidadeId, CancellationToken cancellationToken = default);

    Task<HorarioDisponibilidade?> BuscarPorHorarioAsync(TimeOnly horario, CancellationToken cancellationToken = default);
}