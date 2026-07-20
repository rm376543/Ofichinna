using Ofichina.Domain.Entities;

namespace Ofichina.Domain.Interfaces;

public interface IHorarioDisponibilidadeRepository : IRepository<HorarioDisponibilidade>
{
    Task<IReadOnlyCollection<HorarioDisponibilidade>> GetHorariosPorDiaAsync(Guid diaDisponibilidadeId, CancellationToken cancellationToken = default);
}