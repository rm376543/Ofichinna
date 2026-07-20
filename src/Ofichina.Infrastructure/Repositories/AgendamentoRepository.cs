using Microsoft.EntityFrameworkCore;
using Ofichina.Domain.Common;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Interfaces;
using Ofichina.Infrastructure.Persistence;

namespace Ofichina.Infrastructure.Repositories;

public sealed class AgendamentoRepository : Repository<Agendamento>, IAgendamentoRepository
{
    private readonly ApplicationDbContext _context;

    public AgendamentoRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<PagedResult<Agendamento>> GetPagedByClientePessoaAsync(Guid pessoaId, Pagination pagination, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(pagination);

        var pageNumber = pagination.PageNumber > 0 ? pagination.PageNumber : 1;
        var pageSize = pagination.PageSize > 0 ? pagination.PageSize : 10;

        var query = _context.Agendamentos
            .AsNoTracking()
            .Where(x => x.DeletedAt == null && x.ClientePessoaId == pessoaId)
            .OrderBy(x => x.DataAgendamento)
            .ThenBy(x => x.HorarioAgendamento)
            .ThenBy(x => x.Id);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Agendamento>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<Agendamento?> GetByIdAndPessoaAsync(Guid agendamentoId, Guid pessoaId, CancellationToken cancellationToken = default)
    {
        return await _context.Agendamentos
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == agendamentoId && x.ClientePessoaId == pessoaId && x.DeletedAt == null, cancellationToken);
    }

    public async Task<bool> ExisteConflitoConsultorAsync(Guid consultorPessoaId, DateOnly dataAgendamento, TimeOnly horarioAgendamento, CancellationToken cancellationToken = default)
    {
        return await _context.Agendamentos
            .AsNoTracking()
            .AnyAsync(x => x.DeletedAt == null && x.ConsultorPessoaId == consultorPessoaId && x.DataAgendamento == dataAgendamento && x.HorarioAgendamento == horarioAgendamento, cancellationToken);
    }

    public async Task<bool> ExisteConflitoVeiculoAsync(Guid veiculoId, DateOnly dataAgendamento, TimeOnly horarioAgendamento, CancellationToken cancellationToken = default)
    {
        return await _context.Agendamentos
            .AsNoTracking()
            .AnyAsync(x => x.DeletedAt == null && x.VeiculoId == veiculoId && x.DataAgendamento == dataAgendamento && x.HorarioAgendamento == horarioAgendamento, cancellationToken);
    }
}
