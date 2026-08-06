using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.Abstractions.Interfaces;
using Ofichina.Application.UseCases.Agendamentos.Queries;

namespace Ofichina.Application.UseCases.Agendamentos.Handlers;

/// <summary>
/// Handler para listar consultores disponíveis para dia + horário.
/// </summary>
public sealed class ListarConsultoresPorDiaHorarioQueryHandler : IQueryHandler<ListarConsultoresPorDiaHorarioQuery, IEnumerable<ConsultorListaDto>>
{
    private readonly IHorarioConsultorDisponibilidadeRepository _slotRepository;
    private readonly IPessoaRepository _pessoaRepository;
    private readonly ILogger<ListarConsultoresPorDiaHorarioQueryHandler> _logger;

    public ListarConsultoresPorDiaHorarioQueryHandler(
        IHorarioConsultorDisponibilidadeRepository slotRepository,
        IPessoaRepository pessoaRepository,
        ILogger<ListarConsultoresPorDiaHorarioQueryHandler> logger)
    {
        _slotRepository = slotRepository;
        _pessoaRepository = pessoaRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<ConsultorListaDto>> HandleAsync(
        ListarConsultoresPorDiaHorarioQuery query,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Listando consultores. DiaId: {DiaId}, HorarioId: {HorarioId}", 
                query.DiaDisponibilidadeId, query.HorarioDisponibilidadeId);

            var slots = await _slotRepository.GetAllAsync(cancellationToken);

            var consultorIds = slots
                .Where(s => s.DiaDisponibilidadeId == query.DiaDisponibilidadeId && 
                           s.HorarioDisponibilidadeId == query.HorarioDisponibilidadeId)
                .Select(s => s.ConsultorPessoaId)
                .Distinct()
                .ToList();

            var consultores = await _pessoaRepository.GetAllAsync(cancellationToken);

            var resultado = consultores
                .Where(c => consultorIds.Contains(c.Id))
                .OrderBy(c => c.Nome)
                .Select(c => new ConsultorListaDto
                {
                    PessoaId = c.Id,
                    Nome = c.Nome,
                    Documento = c.Documento?.Numero ?? string.Empty
                })
                .ToList();

            _logger.LogInformation("Encontrados {Count} consultores", resultado.Count);

            return resultado;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar consultores. DiaId: {DiaId}, HorarioId: {HorarioId}", 
                query.DiaDisponibilidadeId, query.HorarioDisponibilidadeId);
            return Enumerable.Empty<ConsultorListaDto>();
        }
    }
}
