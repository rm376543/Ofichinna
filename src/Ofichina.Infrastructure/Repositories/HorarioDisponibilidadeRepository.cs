using Microsoft.EntityFrameworkCore;
using Ofichina.Domain.Entities;
using Ofichina.Infrastructure.Persistence;
using Ofichina.Contracts;
using Ofichina.Contracts.Common;
using Ofichina.Application.Abstractions.Interfaces.Repository;

namespace Ofichina.Infrastructure.Repositories;

public sealed class HorarioDisponibilidadeRepository : Repository<HorarioDisponibilidade>, IHorarioDisponibilidadeRepository
{
    private readonly ApplicationDbContext _context;

    public HorarioDisponibilidadeRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }


    /// <summary>
    /// Busca os horários de disponibilidade por dia de disponibilidade.
    /// </summary>
    /// <param name="diaDisponibilidadeId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IReadOnlyCollection<HorarioDisponibilidade>> GetHorariosPorDiaAsync(Guid diaDisponibilidadeId, CancellationToken cancellationToken = default)
    {
        return await _context.DiasHorariosDisponibilidade
            .AsNoTracking()
            .Where(x => x.DiaDisponibilidadeId == diaDisponibilidadeId)
            .Select(x => x.HorarioDisponibilidade)
            .OrderBy(x => x.Hora)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Busca os horários de disponibilidade paginados.
    /// </summary>
    /// <param name="pagination"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public new async Task<PagedResponse<HorarioDisponibilidade>> GetPagedAsync(Pagination pagination, CancellationToken cancellationToken = default)
    {

        ArgumentNullException.ThrowIfNull(pagination);

        var pageNumber = pagination.PageNumber > 0 ? pagination.PageNumber : 1;
        var pageSize = pagination.PageSize > 0 ? pagination.PageSize : 10;

        var query = _context.Set<HorarioDisponibilidade>()
            .AsNoTracking()
            .OrderBy(x => x.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return items.ToPagedResponse(totalCount, pageNumber, pageSize);
    }

    public async Task<HorarioDisponibilidade?> BuscarPorHorarioAsync(TimeOnly horario, CancellationToken cancellationToken = default)
    {
        var query = _context.Set<HorarioDisponibilidade>()
            .AsNoTracking()
            .Where(x => x.Hora == horario && x.DeletedAt == null);

        return await query.FirstOrDefaultAsync(cancellationToken);
    }
}