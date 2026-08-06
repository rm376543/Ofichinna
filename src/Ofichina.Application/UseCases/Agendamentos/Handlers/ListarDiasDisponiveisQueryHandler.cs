using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.Abstractions.Interfaces;
using Ofichina.Application.UseCases.Agendamentos.Queries;

namespace Ofichina.Application.UseCases.Agendamentos.Handlers;

/// <summary>
/// Handler para listar dias disponíveis em um período (mês/ano).
/// </summary>
public sealed class ListarDiasDisponiveisQueryHandler : IQueryHandler<ListarDiasDisponiveisQuery, IEnumerable<DiaListaDto>>
{
    private readonly IDiaDisponibilidadeRepository _diaDisponibilidadeRepository;
    private readonly ILogger<ListarDiasDisponiveisQueryHandler> _logger;

    public ListarDiasDisponiveisQueryHandler(
        IDiaDisponibilidadeRepository diaDisponibilidadeRepository,
        ILogger<ListarDiasDisponiveisQueryHandler> logger)
    {
        _diaDisponibilidadeRepository = diaDisponibilidadeRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<DiaListaDto>> HandleAsync(
        ListarDiasDisponiveisQuery query,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Listando dias disponíveis. Mês: {Mes}, Ano: {Ano}", query.Mes, query.Ano);

            var todas = await _diaDisponibilidadeRepository.GetAllAsync(cancellationToken);

            var diasFiltrados = todas
                .Where(d => d.Data.Year == query.Ano && d.Data.Month == query.Mes && d.Data >= DateOnly.FromDateTime(DateTime.UtcNow))
                .OrderBy(d => d.Data)
                .Select(d => new DiaListaDto
                {
                    Id = d.Id,
                    Data = d.Data.ToString("yyyy-MM-dd")
                })
                .ToList();

            _logger.LogInformation("Encontrados {Count} dias disponíveis para {Mes}/{Ano}", diasFiltrados.Count, query.Mes, query.Ano);

            return diasFiltrados;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar dias disponíveis. Mês: {Mes}, Ano: {Ano}", query.Mes, query.Ano);
            return Enumerable.Empty<DiaListaDto>();
        }
    }
}
