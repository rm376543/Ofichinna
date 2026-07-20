using Ofichina.Domain.Entities;
using Ofichina.Domain.Common;

namespace Ofichina.Application.Abstractions.Interfaces;

public interface IHorarioDisponibilidadeRepository : IRepository<HorarioDisponibilidade>
{
    Task<IReadOnlyCollection<HorarioDisponibilidade>> GetHorariosPorDiaAsync(Guid diaDisponibilidadeId, CancellationToken cancellationToken = default);
}