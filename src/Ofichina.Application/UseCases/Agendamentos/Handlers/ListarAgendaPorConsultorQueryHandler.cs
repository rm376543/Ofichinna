using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Agendamentos.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Agendamento;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;

namespace Ofichina.Application.UseCases.Agendamentos.Handlers;

/// <summary>
/// Handler para listar agenda de um consultor em uma data.
/// </summary>
public sealed class ListarAgendaPorConsultorQueryHandler : IQueryHandler<ListarAgendaPorConsultorQuery, Result<IEnumerable<AgendaConsultorResponse>>>
{
    private readonly IAgendaConsultorRepository _slotRepository;
    private readonly IAgendamentoRepository _agendamentoRepository;
    private readonly ILogger<ListarAgendaPorConsultorQueryHandler> _logger;

    public ListarAgendaPorConsultorQueryHandler(
        IAgendaConsultorRepository slotRepository,
        IAgendamentoRepository agendamentoRepository,
        ILogger<ListarAgendaPorConsultorQueryHandler> logger)
    {
        _slotRepository = slotRepository;
        _agendamentoRepository = agendamentoRepository;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<AgendaConsultorResponse>>> HandleAsync(
        ListarAgendaPorConsultorQuery query,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Listando agenda. ConsultorId: {ConsultorId}, Data: {Data}", query.ConsultorPessoaId, query.Data);

            var slots = await _slotRepository.GetAllWithIncludesAsync(cancellationToken);
            var agendamentos = await _agendamentoRepository.GetAllWithIncludesAsync(cancellationToken);
            var agendamentosPorSlot = agendamentos.ToDictionary(a => a.AgendaConsultorId);

            var slotsDoConsultor = slots
                .Where(s => s.ConsultorPessoaId == query.ConsultorPessoaId
                    && s.DiaDisponibilidade.Data == query.Data)
                .ToList();

            var resultado = slotsDoConsultor
                .OrderBy(s => s.HorarioDisponibilidade.Hora)
                .Select(slot => new AgendaConsultorResponse
                {
                    AgendaId = slot.Id,
                    Hora = slot.HorarioDisponibilidade.Hora.ToString("HH:mm"),
                    Status = DeterminarStatus(slot, agendamentosPorSlot),
                    ClienteNome = ObterClienteNome(slot, agendamentosPorSlot),
                    Veiculo = ObterVeiculo(slot, agendamentosPorSlot)
                })
                .ToList();

            _logger.LogInformation("Encontrados {Count} slots na agenda", resultado.Count);

            return Result.Success(resultado.AsEnumerable());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar agenda. ConsultorId: {ConsultorId}, Data: {Data}",
                query.ConsultorPessoaId, query.Data);
            return Result.Failure<IEnumerable<AgendaConsultorResponse>>(ex.Message);
        }
    }

    private static string DeterminarStatus(AgendaConsultor slot, IReadOnlyDictionary<Guid, Agendamento> agendamentosPorSlot)
    {
        agendamentosPorSlot.TryGetValue(slot.Id, out var agendamento);

        if (agendamento is null)
            return "VAGO";

        return agendamento.Status.ToString();
    }

    private static string? ObterClienteNome(AgendaConsultor slot, IReadOnlyDictionary<Guid, Agendamento> agendamentosPorSlot)
    {
        agendamentosPorSlot.TryGetValue(slot.Id, out var agendamento);
        return agendamento?.Cliente?.Nome;
    }

    private static string? ObterVeiculo(AgendaConsultor slot, IReadOnlyDictionary<Guid, Agendamento> agendamentosPorSlot)
    {
        agendamentosPorSlot.TryGetValue(slot.Id, out var agendamento);
        return agendamento?.Veiculo?.Placa?.Numero;
    }
}
