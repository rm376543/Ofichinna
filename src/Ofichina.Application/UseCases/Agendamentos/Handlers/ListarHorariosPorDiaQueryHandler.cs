using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Agendamentos.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Agendamento;

namespace Ofichina.Application.UseCases.Agendamentos.Handlers;

/// <summary>
/// Handler para listar horários disponíveis de um dia.
/// </summary>
public sealed class ListarHorariosPorDiaQueryHandler : IQueryHandler<ListarHorariosPorDiaQuery, Result<IEnumerable<HorarioDisponivelResponse>>>
{
    private readonly IHorarioDisponibilidadeRepository _horarioRepository;
    private readonly IAgendaConsultorRepository _slotRepository;
    private readonly ILogger<ListarHorariosPorDiaQueryHandler> _logger;

    public ListarHorariosPorDiaQueryHandler(
        IHorarioDisponibilidadeRepository horarioRepository,
        IAgendaConsultorRepository slotRepository,
        ILogger<ListarHorariosPorDiaQueryHandler> logger)
    {
        _horarioRepository = horarioRepository;
        _slotRepository = slotRepository;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<HorarioDisponivelResponse>>> HandleAsync(
        ListarHorariosPorDiaQuery query,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Listando horários. DiaId: {DiaId}", query.DiaDisponibilidadeId);

            var buscarHorarios = await _horarioRepository.GetAllAsync(cancellationToken);
            var slots = await _slotRepository.GetAllAsync(cancellationToken);

            var horarios = buscarHorarios
                .OrderBy(h => h.Hora)
                .Select(h => new HorarioListaResponse
                {
                    HorarioListaId = h.Id,
                    Hora = h.Hora.ToString("HH:mm"),
                    Disponivel = slots.Any(s => s.DiaDisponibilidadeId == query.DiaDisponibilidadeId && s.HorarioDisponibilidadeId == h.Id)
                })
                .ToList();

            _logger.LogInformation("Encontrados {Count} horários para dia {DiaId}", horarios.Count, query.DiaDisponibilidadeId);

            var resultado = horarios.Select(h => new HorarioDisponivelResponse
            {
                HorarioDisponivelId = h.HorarioListaId,
                Horario = TimeOnly.ParseExact(h.Hora, "HH:mm"),
                Disponivel = h.Disponivel
            });

            return Result.Success(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar horários. DiaId: {DiaId}", query.DiaDisponibilidadeId);
            return Result.Failure<IEnumerable<HorarioDisponivelResponse>>(ex.Message);
        }
    }
}
