using Ofichina.Domain.Entities;

namespace Ofichina.Application.Abstractions.Interfaces.Repository;

public interface IHorarioConsultorRepository : IRepository<HorarioConsultor>
{
    Task<IReadOnlyCollection<HorarioConsultor>> GetConsultoresPorHorarioAsync(Guid horarioDisponibilidadeId, CancellationToken cancellationToken = default);
}