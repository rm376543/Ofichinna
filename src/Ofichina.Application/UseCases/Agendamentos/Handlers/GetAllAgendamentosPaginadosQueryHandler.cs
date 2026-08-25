using Ofichina.Application.Abstractions;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Application.UseCases.Agendamentos.Mappings;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Agendamento;

namespace Ofichina.Application.UseCases.Entidades.Handlers;

/// <summary>
/// Handler para listar todos agendamentos do sistema.
/// </summary>
public sealed class GetAllAgendamentosPaginadosQueryHandler
    : IQueryHandler<
        GetAllAgendamentosPaginadosQuery,
        Result<PagedResponse<AgendamentoUsuarioResponse>>>
{
    private readonly IAgendamentoRepository _repository;
    private readonly ILogger<GetAllAgendamentosPaginadosQueryHandler> _logger;

    public GetAllAgendamentosPaginadosQueryHandler(
        IAgendamentoRepository repository,
        ILogger<GetAllAgendamentosPaginadosQueryHandler> logger)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<Result<PagedResponse<AgendamentoUsuarioResponse>>> HandleAsync(
        GetAllAgendamentosPaginadosQuery query,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var agendamentos = await _repository.GetAllAgendamentosPaginadosAsync(
                query.Pagination,
                cancellationToken);

            if (agendamentos.Items.Count == 0)
            {
                return Result.Failure<PagedResponse<AgendamentoUsuarioResponse>>(
                    "Nenhum registro encontrado.");
            }

            var result = agendamentos.ToPagedResponse(
                x => x.ToUsuarioResponse());

            return Result.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar registros.");

            return Result.Failure<PagedResponse<AgendamentoUsuarioResponse>>(
                "Não foi possível obter os registros.");
        }
    }
}