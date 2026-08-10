using Ofichina.Application.Abstractions;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Application.UseCases.Agendamentos.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Agendamento;

namespace Ofichina.Application.UseCases.Agendamentos.Handlers;

/// <summary>
/// Handler para listar dias disponíveis em um período (mês/ano).
/// </summary>
public sealed class ListarDiasDisponiveisQueryHandler : IQueryHandler<ListarDiasDisponiveisQuery, Result<IEnumerable<DiaDisponibilidadeResponse>>>
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

    public async Task<Result<IEnumerable<DiaDisponibilidadeResponse>>> HandleAsync(
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
                .Select(d => new DiaListaResponse
                {
                    DiaListaId = d.Id,
                    Data = d.Data.ToString("yyyy-MM-dd")
                })
                .ToList();

            _logger.LogInformation("Encontrados {Count} dias disponíveis para {Mes}/{Ano}", diasFiltrados.Count, query.Mes, query.Ano);

            var resultado = diasFiltrados.Select(d => new DiaDisponibilidadeResponse
            {
                DiaId = d.DiaListaId,
                Dia = DateOnly.ParseExact(d.Data, "yyyy-MM-dd"),
            });

            return Result.Success(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar dias disponíveis. Mês: {Mes}, Ano: {Ano}", query.Mes, query.Ano);
            return Result.Failure<IEnumerable<DiaDisponibilidadeResponse>>("Erro ao listar dias disponíveis.");

        }
    }
}
