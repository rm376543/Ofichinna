using Ofichina.Domain.Entities;

namespace Ofichina.Application.Abstractions.Interfaces;

public interface IHorarioConsultorRepository : IRepository<HorarioConsultor>
{
    Task<IReadOnlyCollection<HorarioConsultor>> GetConsultoresPorHorarioAsync(Guid horarioDisponibilidadeId, CancellationToken cancellationToken = default);
}