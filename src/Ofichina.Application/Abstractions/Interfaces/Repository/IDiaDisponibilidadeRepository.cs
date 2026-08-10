using Ofichina.Domain.Entities;

namespace Ofichina.Application.Abstractions.Interfaces.Repository;

public interface IDiaDisponibilidadeRepository : IRepository<DiaDisponibilidade>
{
    Task<IReadOnlyCollection<DiaDisponibilidade>> GetDiasDisponiveisAsync(CancellationToken cancellationToken = default);
}