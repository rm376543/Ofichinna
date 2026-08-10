using Microsoft.EntityFrameworkCore;
using Ofichina.Domain.Entities;
using Ofichina.Infrastructure.Persistence;
using Ofichina.Application.Abstractions.Interfaces.Repository;

namespace Ofichina.Infrastructure.Repositories;

/// <summary>
/// Repositório para gerenciar a entidade AgendaConsultor.
/// </summary>
public sealed class AgendaConsultorRepository : Repository<AgendaConsultor>, IAgendaConsultorRepository
{
    private readonly ApplicationDbContext _context;

    public AgendaConsultorRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    /// <summary>
    /// Busca um slot de disponibilidade pelo Id com a navegação do consultor carregada.
    /// </summary>
    public async Task<AgendaConsultor?> GetByIdWithConsultorAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.HorariosConsultorDisponibilidade
            .AsNoTracking()
            .Include(x => x.Consultor)
            .FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null, cancellationToken);
    }

    /// <summary>
    /// Busca um slot de disponibilidade pela composição de Dia + Horário + Consultor.
    /// </summary>
    public async Task<AgendaConsultor?> GetByDiaHorarioConsultorAsync(
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
                    && x.DeletedAt == null,
                cancellationToken);
    }

    /// <summary>
    /// Busca todos os horários disponíveis de um consultor em um dia específico.
    /// </summary>
    public async Task<IReadOnlyCollection<AgendaConsultor>> GetByConsultorAndDiaAsync(
        Guid consultorPessoaId,
        Guid diaDisponibilidadeId,
        CancellationToken cancellationToken = default)
    {
        return await _context.HorariosConsultorDisponibilidade
            .AsNoTracking()
            .Where(x => x.ConsultorPessoaId == consultorPessoaId
                && x.DiaDisponibilidadeId == diaDisponibilidadeId
                && x.DeletedAt == null)
            .OrderBy(x => x.HorarioDisponibilidade.Hora)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Busca todos os consultores disponíveis em um dia e horário específico.
    /// </summary>
    public async Task<IReadOnlyCollection<AgendaConsultor>> GetConsultoresByDiaAndHorarioAsync(
        Guid diaDisponibilidadeId,
        Guid horarioDisponibilidadeId,
        CancellationToken cancellationToken = default)
    {
        return await _context.HorariosConsultorDisponibilidade
            .AsNoTracking()
            .Where(x => x.DiaDisponibilidadeId == diaDisponibilidadeId
                && x.HorarioDisponibilidadeId == horarioDisponibilidadeId
                && x.DeletedAt == null)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Busca todos os horários disponíveis de um dia.
    /// </summary>
    public async Task<IReadOnlyCollection<AgendaConsultor>> GetByDiaAsync(
        Guid diaDisponibilidadeId,
        CancellationToken cancellationToken = default)
    {
        return await _context.HorariosConsultorDisponibilidade
            .AsNoTracking()
            .Where(x => x.DiaDisponibilidadeId == diaDisponibilidadeId && x.DeletedAt == null)
            .Include(x => x.HorarioDisponibilidade)
            .OrderBy(x => x.HorarioDisponibilidade.Hora)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Busca todos os slots com inclusão de relacionamentos.
    /// </summary>
    public async Task<IReadOnlyCollection<AgendaConsultor>> GetAllWithIncludesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.HorariosConsultorDisponibilidade
            .AsNoTracking()
            .Include(x => x.DiaDisponibilidade)
            .Include(x => x.HorarioDisponibilidade)
            .Include(x => x.Consultor)
            .Where(x => x.DeletedAt == null)
            .ToListAsync(cancellationToken);
    }
}
