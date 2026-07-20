using Microsoft.EntityFrameworkCore;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Interfaces;
using Ofichina.Infrastructure.Persistence;

namespace Ofichina.Infrastructure.Repositories;

public sealed class HorarioDisponibilidadeRepository : Repository<HorarioDisponibilidade>, IHorarioDisponibilidadeRepository
{
    private readonly ApplicationDbContext _context;

    public HorarioDisponibilidadeRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<HorarioDisponibilidade>> GetHorariosPorDiaAsync(Guid diaDisponibilidadeId, CancellationToken cancellationToken = default)
    {
        return await _context.DiasHorariosDisponibilidade
            .AsNoTracking()
            .Where(x => x.DiaDisponibilidadeId == diaDisponibilidadeId)
            .Select(x => x.HorarioDisponibilidade)
            .OrderBy(x => x.Hora)
            .ToListAsync(cancellationToken);
    }
}