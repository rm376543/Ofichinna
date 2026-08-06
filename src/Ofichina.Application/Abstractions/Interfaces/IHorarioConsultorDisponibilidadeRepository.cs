using Ofichina.Domain.Entities;
using Ofichina.Application.Abstractions.Common;

namespace Ofichina.Application.Abstractions.Interfaces;

/// <summary>
/// Interface do repositório para gerenciar HorarioConsultorDisponibilidade.
/// </summary>
public interface IHorarioConsultorDisponibilidadeRepository : IRepository<HorarioConsultorDisponibilidade>
{
    /// <summary>
    /// Busca um slot pela composição de Dia + Horário + Consultor.
    /// </summary>
    Task<HorarioConsultorDisponibilidade?> GetByDiaHorarioConsultorAsync(
        Guid diaDisponibilidadeId,
        Guid horarioDisponibilidadeId,
        Guid consultorPessoaId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca todos os horários de um consultor em um dia específico.
    /// </summary>
    Task<IReadOnlyCollection<HorarioConsultorDisponibilidade>> GetByConsultorAndDiaAsync(
        Guid consultorPessoaId,
        Guid diaDisponibilidadeId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca todos os consultores disponíveis em um dia e horário.
    /// </summary>
    Task<IReadOnlyCollection<HorarioConsultorDisponibilidade>> GetConsultoresByDiaAndHorarioAsync(
        Guid diaDisponibilidadeId,
        Guid horarioDisponibilidadeId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca todos os horários disponíveis de um dia.
    /// </summary>
    Task<IReadOnlyCollection<HorarioConsultorDisponibilidade>> GetByDiaAsync(
        Guid diaDisponibilidadeId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca todos os slots com inclusão de relacionamentos.
    /// </summary>
    Task<IReadOnlyCollection<HorarioConsultorDisponibilidade>> GetAllWithIncludesAsync(
        CancellationToken cancellationToken = default);
}
