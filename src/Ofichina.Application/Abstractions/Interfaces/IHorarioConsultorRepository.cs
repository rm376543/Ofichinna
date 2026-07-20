using Ofichina.Domain.Entities;
using Ofichina.Domain.Common;

namespace Ofichina.Application.Abstractions.Interfaces;

public interface IHorarioConsultorRepository : IRepository<HorarioConsultor>
{
    Task<IReadOnlyCollection<HorarioConsultor>> GetConsultoresPorHorarioAsync(Guid horarioDisponibilidadeId, CancellationToken cancellationToken = default);
}