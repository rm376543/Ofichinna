using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.Abstractions.Interfaces;
using Ofichina.Application.UseCases.Agendamentos.Queries;

namespace Ofichina.Application.UseCases.Agendamentos.Handlers;

/// <summary>
/// Handler para listar horários disponíveis de um dia.
/// </summary>
public sealed class ListarHorariosPorDiaQueryHandler : IQueryHandler<ListarHorariosPorDiaQuery, IEnumerable<HorarioListaDto>>
{
    private readonly IHorarioDisponibilidadeRepository _horarioRepository;
    private readonly IHorarioConsultorDisponibilidadeRepository _slotRepository;
    private readonly ILogger<ListarHorariosPorDiaQueryHandler> _logger;

    public ListarHorariosPorDiaQueryHandler(
        IHorarioDisponibilidadeRepository horarioRepository,
        IHorarioConsultorDisponibilidadeRepository slotRepository,
        ILogger<ListarHorariosPorDiaQueryHandler> logger)
    {
        _horarioRepository = horarioRepository;
        _slotRepository = slotRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<HorarioListaDto>> HandleAsync(
        ListarHorariosPorDiaQuery query,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Listando horários. DiaId: {DiaId}", query.DiaDisponibilidadeId);

            var horarios = await _horarioRepository.GetAllAsync(cancellationToken);
            var slots = await _slotRepository.GetAllAsync(cancellationToken);

            var resultado = horarios
                .OrderBy(h => h.Hora)
                .Select(h => new HorarioListaDto
                {
                    Id = h.Id,
                    Hora = h.Hora.ToString("HH:mm"),
                    Disponivel = slots.Any(s => s.DiaDisponibilidadeId == query.DiaDisponibilidadeId && s.HorarioDisponibilidadeId == h.Id)
                })
                .ToList();

            _logger.LogInformation("Encontrados {Count} horários para dia {DiaId}", resultado.Count, query.DiaDisponibilidadeId);

            return resultado;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar horários. DiaId: {DiaId}", query.DiaDisponibilidadeId);
            return Enumerable.Empty<HorarioListaDto>();
        }
    }
}
