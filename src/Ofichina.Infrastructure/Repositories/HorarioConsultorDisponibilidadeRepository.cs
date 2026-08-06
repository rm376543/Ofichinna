using Microsoft.EntityFrameworkCore;
using Ofichina.Domain.Entities;
using Ofichina.Application.Abstractions.Interfaces;
using Ofichina.Infrastructure.Persistence;

namespace Ofichina.Infrastructure.Repositories;

/// <summary>
/// Repositório para gerenciar a entidade HorarioConsultorDisponibilidade.
/// </summary>
public sealed class HorarioConsultorDisponibilidadeRepository : Repository<HorarioConsultorDisponibilidade>, IHorarioConsultorDisponibilidadeRepository
{
    private readonly ApplicationDbContext _context;

    public HorarioConsultorDisponibilidadeRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    /// <summary>
    /// Busca um slot de disponibilidade pela composição de Dia + Horário + Consultor.
    /// </summary>
    public async Task<HorarioConsultorDisponibilidade?> GetByDiaHorarioConsultorAsync(
        Guid diaDisponibilidadeId,
        Guid horarioDisponibilidadeId,
        Guid consultorPessoaId,
        CancellationToken cancellationToken = default)
    {
        return await _context.HorariosConsultorDisponibilidade
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.DiaDisponibilidadeId == diaDisponibilidadeId 
                    && x.HorarioDisponibilidadeId == horarioDisponibilidadeId
                    && x.ConsultorPessoaId == consultorPessoaId
                    && !x.EstaExcluida(),
                cancellationToken);
    }

    /// <summary>
    /// Busca todos os horários disponíveis de um consultor em um dia específico.
    /// </summary>
    public async Task<IReadOnlyCollection<HorarioConsultorDisponibilidade>> GetByConsultorAndDiaAsync(
        Guid consultorPessoaId,
        Guid diaDisponibilidadeId,
        CancellationToken cancellationToken = default)
    {
        return await _context.HorariosConsultorDisponibilidade
            .AsNoTracking()
            .Where(x => x.ConsultorPessoaId == consultorPessoaId
                && x.DiaDisponibilidadeId == diaDisponibilidadeId
                && !x.EstaExcluida())
            .OrderBy(x => x.HorarioDisponibilidade.Hora)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Busca todos os consultores disponíveis em um dia e horário específico.
    /// </summary>
    public async Task<IReadOnlyCollection<HorarioConsultorDisponibilidade>> GetConsultoresByDiaAndHorarioAsync(
        Guid diaDisponibilidadeId,
        Guid horarioDisponibilidadeId,
        CancellationToken cancellationToken = default)
    {
        return await _context.HorariosConsultorDisponibilidade
            .AsNoTracking()
            .Where(x => x.DiaDisponibilidadeId == diaDisponibilidadeId
                && x.HorarioDisponibilidadeId == horarioDisponibilidadeId
                && !x.EstaExcluida())
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Busca todos os horários disponíveis de um dia.
    /// </summary>
    public async Task<IReadOnlyCollection<HorarioConsultorDisponibilidade>> GetByDiaAsync(
        Guid diaDisponibilidadeId,
        CancellationToken cancellationToken = default)
    {
        return await _context.HorariosConsultorDisponibilidade
            .AsNoTracking()
            .Where(x => x.DiaDisponibilidadeId == diaDisponibilidadeId && !x.EstaExcluida())
            .Include(x => x.HorarioDisponibilidade)
            .OrderBy(x => x.HorarioDisponibilidade.Hora)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Busca todos os slots com inclusão de relacionamentos.
    /// </summary>
    public async Task<IReadOnlyCollection<HorarioConsultorDisponibilidade>> GetAllWithIncludesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.HorariosConsultorDisponibilidade
            .AsNoTracking()
            .Include(x => x.DiaDisponibilidade)
            .Include(x => x.HorarioDisponibilidade)
            .Include(x => x.Consultor)
            .Where(x => !x.EstaExcluida())
            .ToListAsync(cancellationToken);
    }
}
