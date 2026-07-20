using Microsoft.EntityFrameworkCore;
using Ofichina.Domain.Entities;
using Ofichina.Application.Abstractions.Interfaces;
using Ofichina.Infrastructure.Persistence;

namespace Ofichina.Infrastructure.Repositories;

public sealed class HorarioConsultorRepository : Repository<HorarioConsultor>, IHorarioConsultorRepository
{
    private readonly ApplicationDbContext _context;

    public HorarioConsultorRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<HorarioConsultor>> GetConsultoresPorHorarioAsync(Guid horarioDisponibilidadeId, CancellationToken cancellationToken = default)
    {
        return await _context.HorariosConsultores
            .AsNoTracking()
            .Include(x => x.Pessoa)
            .Where(x => x.HorarioDisponibilidadeId == horarioDisponibilidadeId)
            .OrderBy(x => x.Pessoa.Nome)
            .ToListAsync(cancellationToken);
    }
}