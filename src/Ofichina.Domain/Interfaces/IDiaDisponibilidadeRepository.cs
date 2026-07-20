using Ofichina.Domain.Entities;

namespace Ofichina.Domain.Interfaces;

public interface IDiaDisponibilidadeRepository : IRepository<DiaDisponibilidade>
{
    Task<IReadOnlyCollection<DiaDisponibilidade>> GetDiasDisponiveisAsync(CancellationToken cancellationToken = default);
}