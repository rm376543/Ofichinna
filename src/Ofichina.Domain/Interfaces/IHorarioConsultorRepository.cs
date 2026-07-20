using Ofichina.Domain.Entities;

namespace Ofichina.Domain.Interfaces;

public interface IHorarioConsultorRepository : IRepository<HorarioConsultor>
{
    Task<IReadOnlyCollection<HorarioConsultor>> GetConsultoresPorHorarioAsync(Guid horarioDisponibilidadeId, CancellationToken cancellationToken = default);
}