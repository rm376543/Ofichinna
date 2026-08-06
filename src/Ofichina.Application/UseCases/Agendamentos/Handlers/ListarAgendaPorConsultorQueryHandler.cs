using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.Abstractions.Interfaces;
using Ofichina.Application.UseCases.Agendamentos.Queries;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;

namespace Ofichina.Application.UseCases.Agendamentos.Handlers;

/// <summary>
/// Handler para listar agenda de um consultor em uma data.
/// </summary>
public sealed class ListarAgendaPorConsultorQueryHandler : IQueryHandler<ListarAgendaPorConsultorQuery, IEnumerable<AgendaSlotDto>>
{
    private readonly IHorarioConsultorDisponibilidadeRepository _slotRepository;
    private readonly IAgendamentoRepository _agendamentoRepository;
    private readonly ILogger<ListarAgendaPorConsultorQueryHandler> _logger;

    public ListarAgendaPorConsultorQueryHandler(
        IHorarioConsultorDisponibilidadeRepository slotRepository,
        IAgendamentoRepository agendamentoRepository,
        ILogger<ListarAgendaPorConsultorQueryHandler> logger)
    {
        _slotRepository = slotRepository;
        _agendamentoRepository = agendamentoRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<AgendaSlotDto>> HandleAsync(
        ListarAgendaPorConsultorQuery query,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Listando agenda. ConsultorId: {ConsultorId}, Data: {Data}", 
                query.ConsultorPessoaId, query.Data);

            var slots = await _slotRepository.GetAllAsync(cancellationToken);
            var agendamentos = await _agendamentoRepository.GetAllAsync(cancellationToken);

            var slotsDoConsultor = slots
                .Where(s => s.ConsultorPessoaId == query.ConsultorPessoaId)
                .ToList();

            var resultado = slotsDoConsultor
                .OrderBy(s => s.HorarioDisponibilidade.Hora)
                .Select(slot => new AgendaSlotDto
                {
                    SlotId = slot.Id,
                    Hora = slot.HorarioDisponibilidade.Hora.ToString("HH:mm"),
                    Status = DeterminarStatus(slot, agendamentos),
                    ClienteNome = ObterClienteNome(slot, agendamentos),
                    Veiculo = ObterVeiculo(slot, agendamentos)
                })
                .ToList();

            _logger.LogInformation("Encontrados {Count} slots na agenda", resultado.Count);

            return resultado;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar agenda. ConsultorId: {ConsultorId}, Data: {Data}", 
                query.ConsultorPessoaId, query.Data);
            return Enumerable.Empty<AgendaSlotDto>();
        }
    }

    private static string DeterminarStatus(HorarioConsultorDisponibilidade slot, IEnumerable<Agendamento> agendamentos)
    {
        var agendamento = agendamentos.FirstOrDefault(a => a.HorarioConsultorDisponibilidadeId == slot.Id);
        if (agendamento is null)
            return "VAGO";

        return agendamento.Status.ToString();
    }

    private static string? ObterClienteNome(HorarioConsultorDisponibilidade slot, IEnumerable<Agendamento> agendamentos)
    {
        var agendamento = agendamentos.FirstOrDefault(a => a.HorarioConsultorDisponibilidadeId == slot.Id);
        return agendamento?.Cliente?.Nome;
    }

    private static string? ObterVeiculo(HorarioConsultorDisponibilidade slot, IEnumerable<Agendamento> agendamentos)
    {
        var agendamento = agendamentos.FirstOrDefault(a => a.HorarioConsultorDisponibilidadeId == slot.Id);
        return agendamento?.Veiculo?.Placa?.Numero;
    }
}
