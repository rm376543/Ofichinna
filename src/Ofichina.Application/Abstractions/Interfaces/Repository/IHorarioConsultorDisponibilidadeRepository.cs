using Ofichina.Domain.Entities;
using Ofichina.Application.Abstractions.Common;

namespace Ofichina.Application.Abstractions.Interfaces.Repository;

/// <summary>
/// Interface do repositório para gerenciar AgendaConsultor.
/// </summary>
public interface IAgendaConsultorRepository : IRepository<AgendaConsultor>
{
    /// <summary>
    /// Busca um slot pelo Id com a navegação do consultor carregada.
    /// </summary>
    Task<AgendaConsultor?> GetByIdWithConsultorAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca um slot pela composição de Dia + Horário + Consultor.
    /// </summary>
    Task<AgendaConsultor?> GetByDiaHorarioConsultorAsync(
        Guid diaDisponibilidadeId,
        Guid horarioDisponibilidadeId,
        Guid consultorPessoaId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca todos os horários de um consultor em um dia específico.
    /// </summary>
    Task<IReadOnlyCollection<AgendaConsultor>> GetByConsultorAndDiaAsync(
        Guid consultorPessoaId,
        Guid diaDisponibilidadeId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca todos os consultores disponíveis em um dia e horário.
    /// </summary>
    Task<IReadOnlyCollection<AgendaConsultor>> GetConsultoresByDiaAndHorarioAsync(
        Guid diaDisponibilidadeId,
        Guid horarioDisponibilidadeId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca todos os horários disponíveis de um dia.
    /// </summary>
    Task<IReadOnlyCollection<AgendaConsultor>> GetByDiaAsync(
        Guid diaDisponibilidadeId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca todos os slots com inclusão de relacionamentos.
    /// </summary>
    Task<IReadOnlyCollection<AgendaConsultor>> GetAllWithIncludesAsync(
        CancellationToken cancellationToken = default);
}
