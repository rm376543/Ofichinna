using Microsoft.EntityFrameworkCore;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Interfaces;
using Ofichina.Infrastructure.Persistence;

namespace Ofichina.Infrastructure.Repositories;

public sealed class DiaDisponibilidadeRepository : Repository<DiaDisponibilidade>, IDiaDisponibilidadeRepository
{
    private readonly ApplicationDbContext _context;

    public DiaDisponibilidadeRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<DiaDisponibilidade>> GetDiasDisponiveisAsync(CancellationToken cancellationToken = default)
    {
        return await _context.DiasDisponibilidade
            .AsNoTracking()
            .OrderBy(x => x.Data)
            .ToListAsync(cancellationToken);
    }
}