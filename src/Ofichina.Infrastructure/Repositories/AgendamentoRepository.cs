using Microsoft.EntityFrameworkCore;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Enums;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;
using Ofichina.Infrastructure.Persistence;

namespace Ofichina.Infrastructure.Repositories;

public sealed class AgendamentoRepository : Repository<Agendamento>, IAgendamentoRepository
{
    private readonly ApplicationDbContext _context;

    public AgendamentoRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    /// <summary>
    /// Retorna todos os agendamentos de um cliente específico, com paginação.
    /// </summary>
    /// <param name="pessoaId"></param>
    /// <param name="pagination"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<PagedResponse<Agendamento>> GetPagedByClientePessoaAsync(Guid pessoaId, Pagination pagination, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(pagination);

        var pageNumber = pagination.PageNumber > 0 ? pagination.PageNumber : 1;
        var pageSize = pagination.PageSize > 0 ? pagination.PageSize : 10;

        var query = _context.Agendamentos
            .AsNoTracking()
            .Where(x => x.DeletedAt == null && x.ClientePessoaId == pessoaId)
            .Include(x => x.Cliente)
            .Include(x => x.Veiculo)
            .Include(x => x.AgendaConsultor)
                .ThenInclude(x => x.DiaDisponibilidade)
            .Include(x => x.AgendaConsultor)
                .ThenInclude(x => x.HorarioDisponibilidade)
            .Include(x => x.AgendaConsultor)
                .ThenInclude(x => x.Consultor)
            .OrderBy(x => x.AgendaConsultor.DiaDisponibilidade.Data)
            .ThenBy(x => x.AgendaConsultor.HorarioDisponibilidade.Hora)
            .ThenBy(x => x.Id);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return items.ToPagedResponse(totalCount, pageNumber, pageSize);
    }

    /// <summary>
    /// Retorna um agendamento específico de um cliente, com base no ID do agendamento e no ID da pessoa (cliente).
    /// </summary>
    /// <param name="agendamentoId"></param>
    /// <param name="pessoaId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Agendamento?> GetByIdAndPessoaAsync(Guid agendamentoId, Guid pessoaId, CancellationToken cancellationToken = default)
    {
        return await _context.Agendamentos
            .AsNoTracking()
            .Include(x => x.Cliente)
            .Include(x => x.Veiculo)
            .Include(x => x.AgendaConsultor)
                .ThenInclude(x => x.DiaDisponibilidade)
            .Include(x => x.AgendaConsultor)
                .ThenInclude(x => x.HorarioDisponibilidade)
            .Include(x => x.AgendaConsultor)
                .ThenInclude(x => x.Consultor)
            .FirstOrDefaultAsync(x => x.Id == agendamentoId && x.ClientePessoaId == pessoaId && x.DeletedAt == null, cancellationToken);
    }

    /// <summary>
    /// Verifica se existe algum conflito de agendamento para um consultor específico em um determinado horário.
    /// </summary>
    /// <param name="horarioConsultorId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> ExisteConflitoConsultorAsync(Guid horarioConsultorId, CancellationToken cancellationToken = default)
    {
        return await _context.Agendamentos
            .AsNoTracking()
            .AnyAsync(x => x.DeletedAt == null && x.AgendaConsultorId == horarioConsultorId, cancellationToken);
    }

    /// <summary>
    /// Verifica se existe algum conflito de agendamento para um veículo específico em um determinado dia e horário.
    /// </summary>
    /// <param name="veiculoId"></param>
    /// <param name="diaDisponibilidadeId"></param>
    /// <param name="horarioConsultorId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> ExisteConflitoVeiculoAsync(Guid veiculoId, Guid diaDisponibilidadeId, Guid horarioConsultorId, CancellationToken cancellationToken = default)
    {
        return await _context.Agendamentos
            .AsNoTracking()
            .AnyAsync(x => x.DeletedAt == null
                && x.VeiculoId == veiculoId
                && x.AgendaConsultor.DiaDisponibilidadeId == diaDisponibilidadeId
                && x.AgendaConsultor.HorarioDisponibilidadeId == horarioConsultorId, cancellationToken);
    }

    /// <summary>
    /// Retorna todos os agendamentos com suas entidades relacionadas incluídas (Cliente, AgendaConsultor, Veiculo).
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IReadOnlyCollection<Agendamento>> GetAllWithIncludesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Agendamentos
            .AsNoTracking()
            .Include(x => x.Cliente)
            .Include(x => x.AgendaConsultor)
                .ThenInclude(x => x.DiaDisponibilidade)
            .Include(x => x.AgendaConsultor)
                .ThenInclude(x => x.HorarioDisponibilidade)
            .Include(x => x.AgendaConsultor)
                .ThenInclude(x => x.Consultor)
            .Include(x => x.Veiculo)
            .Where(x => x.DeletedAt == null)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Busca um agendamento específico com base no ID da pessoa (cliente) e retorna o agendamento com suas entidades relacionadas incluídas (Cliente, AgendaConsultor, Veiculo).
    /// </summary>
    /// <param name="pessoaId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Agendamento?> BuscarAgendamentosPorPessoaId(Guid pessoaId, CancellationToken cancellationToken = default)
    {
        return await _context.Agendamentos
            .AsNoTracking()
            .Include(x => x.Cliente)
            .Include(x => x.Veiculo)
            .Include(x => x.AgendaConsultor)
                .ThenInclude(x => x.DiaDisponibilidade)
            .Include(x => x.AgendaConsultor)
                .ThenInclude(x => x.HorarioDisponibilidade)
            .Include(x => x.AgendaConsultor)
                .ThenInclude(x => x.Consultor)
            .FirstOrDefaultAsync(x => x.ClientePessoaId == pessoaId && x.DeletedAt == null, cancellationToken);
    }

    /// <summary>
    /// Retorna uma lista de agendamentos de um usuário específico, com base no ID da pessoa (cliente), usando a visão VwAgendamentoPessoa.
    /// </summary>
    /// <param name="pessoaId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IReadOnlyCollection<VwAgendamentoPessoa>> GetAgendamentosUsuarioViewByPessoaAsync(Guid pessoaId, CancellationToken cancellationToken = default)
    {
        return await _context.AgendamentosUsuarioView
            .AsNoTracking()
            .Where(x => x.PessoaId == pessoaId && x.DeletedAt == null)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Retorna um agendamento específico de um usuário com base no ID da pessoa (cliente) e no ID do agendamento, usando a visão VwAgendamentoPessoa.
    /// </summary>
    /// <param name="pessoaId"></param>
    /// <param name="agendamentosId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<VwAgendamentoPessoa?> GetAgendamentoUsuarioViewByIdAsync(Guid pessoaId, Guid agendamentosId, CancellationToken cancellationToken = default)
    {
        return await _context.AgendamentosUsuarioView
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.PessoaId == pessoaId && x.AgendamentosId == agendamentosId && x.DeletedAt == null, cancellationToken);
    }

    /// <summary>
    /// Busca uma lista paginada de agendamentos com base nos parâmetros de paginação fornecidos.
    /// </summary>
    /// <param name="pagination">Os parâmetros de paginação.</param>
    /// <param name="cancellationToken">O token de cancelamento.</param>
    /// <returns>Uma resposta paginada contendo entidades Agendamento.</returns>
    public async Task<PagedResponse<VwAgendamentoPessoa>> GetAllAgendamentosPaginadosAsync(Pagination pagination, CancellationToken cancellationToken = default)
    {

        ArgumentNullException.ThrowIfNull(pagination);

        var pageNumber = pagination.PageNumber > 0 ? pagination.PageNumber : 1;
        var pageSize = pagination.PageSize > 0 ? pagination.PageSize : 10;

        var query = _context.AgendamentosUsuarioView
            .Where(x => x.DeletedAt == null && x.StatusAgendamento == AgendamentoStatus.Agendado)
            .AsNoTracking();

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return items.ToPagedResponse(totalCount, pageNumber, pageSize);
    }

}
