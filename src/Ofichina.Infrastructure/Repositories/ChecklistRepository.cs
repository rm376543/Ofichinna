namespace Ofichina.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Domain.Entities;
using Ofichina.Infrastructure.Persistence;

public sealed class ChecklistRepository : Repository<Checklist>, IChecklistRepository
{
    private readonly ApplicationDbContext _context;

    public ChecklistRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Checklist?> GetByAgendamentoChecklistIdAsync(
        Guid agendamentoId,
        Guid checklistId,
        CancellationToken cancellationToken = default,
        bool tracking = false)
    {
        var query = _context.Set<Checklist>().AsQueryable();

        if (!tracking)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(x => x.AgendamentoId == agendamentoId && x.Id == checklistId, cancellationToken);
    }
}

