using Microsoft.EntityFrameworkCore;
using Ofichina.Domain.Common;
using Ofichina.Domain.Aggregates;
using Ofichina.Application.Abstractions.Interfaces;
using Ofichina.Infrastructure.Persistence;
using Ofichina.Contracts;
using Ofichina.Contracts.Common;

namespace Ofichina.Infrastructure.Repositories;

public sealed class AgendamentoRepository : Repository<Agendamento>, IAgendamentoRepository
{
    private readonly ApplicationDbContext _context;

    public AgendamentoRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<PagedResponse<Agendamento>> GetPagedByClientePessoaAsync(Guid pessoaId, Pagination pagination, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(pagination);

        var pageNumber = pagination.PageNumber > 0 ? pagination.PageNumber : 1;
        var pageSize = pagination.PageSize > 0 ? pagination.PageSize : 10;

        var query = _context.Agendamentos
            .AsNoTracking()
            .Where(x => x.DeletedAt == null && x.ClientePessoaId == pessoaId)
            .Include(x => x.Cliente)
            .Include(x => x.Consultor)
            .Include(x => x.Veiculo)
            .Include(x => x.DiaDisponibilidade)
            .Include(x => x.HorarioConsultor)
                .ThenInclude(x => x.Pessoa)
            .OrderBy(x => x.DiaDisponibilidade.Data)
            .ThenBy(x => x.HorarioConsultor.HorarioDisponibilidade.Hora)
            .ThenBy(x => x.Id);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return items.ToPagedResponse(totalCount, pageNumber, pageSize);
    }

    public async Task<Agendamento?> GetByIdAndPessoaAsync(Guid agendamentoId, Guid pessoaId, CancellationToken cancellationToken = default)
    {
        return await _context.Agendamentos
            .AsNoTracking()
            .Include(x => x.Cliente)
            .Include(x => x.Consultor)
            .Include(x => x.Veiculo)
            .Include(x => x.DiaDisponibilidade)
            .Include(x => x.HorarioConsultor)
                .ThenInclude(x => x.Pessoa)
            .FirstOrDefaultAsync(x => x.Id == agendamentoId && x.ClientePessoaId == pessoaId && x.DeletedAt == null, cancellationToken);
    }

    public async Task<bool> ExisteConflitoConsultorAsync(Guid horarioConsultorId, CancellationToken cancellationToken = default)
    {
        return await _context.Agendamentos
            .AsNoTracking()
            .AnyAsync(x => x.DeletedAt == null && x.HorarioConsultorId == horarioConsultorId, cancellationToken);
    }

    public async Task<bool> ExisteConflitoVeiculoAsync(Guid veiculoId, Guid diaDisponibilidadeId, Guid horarioConsultorId, CancellationToken cancellationToken = default)
    {
        return await _context.Agendamentos
            .AsNoTracking()
            .AnyAsync(x => x.DeletedAt == null && x.VeiculoId == veiculoId && x.DiaDisponibilidadeId == diaDisponibilidadeId && x.HorarioConsultorId == horarioConsultorId, cancellationToken);
    }

    public async Task<Agendamento?> BuscarAgendamentosPorPessoaId(Guid pessoaId, CancellationToken cancellationToken = default)
    {
        return await _context.Agendamentos
            .AsNoTracking()
            .Include(x => x.Cliente)
            .Include(x => x.Consultor)
            .Include(x => x.Veiculo)
            .Include(x => x.DiaDisponibilidade)
            .Include(x => x.HorarioConsultor)
                .ThenInclude(x => x.Pessoa)
            .FirstOrDefaultAsync(x => x.ClientePessoaId == pessoaId && x.DeletedAt == null, cancellationToken);
    }
}
