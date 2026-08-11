using Ofichina.Application.Abstractions;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Application.UseCases.Agendamentos.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Agendamento;

namespace Ofichina.Application.UseCases.Agendamentos.Handlers
{
    public sealed class GetHorariosDisponiveisQueryHandler : IQueryHandler<GetHorariosDisponiveisQuery, Result<PagedResponse<HorarioResponse>>>
    {
        private readonly IHorarioDisponibilidadeRepository _horarioDisponibilidadeRepository;
        private readonly ILogger<GetHorariosDisponiveisQueryHandler> _logger;

        public GetHorariosDisponiveisQueryHandler(
            IHorarioDisponibilidadeRepository horarioDisponibilidadeRepository,
            ILogger<GetHorariosDisponiveisQueryHandler> logger)
        {
            _horarioDisponibilidadeRepository = horarioDisponibilidadeRepository;
            _logger = logger;
        }

        public async Task<Result<PagedResponse<HorarioResponse>>> HandleAsync(GetHorariosDisponiveisQuery query, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Iniciando busca por horarios disponiveis no sistema.");

                var horariosDisponiveis = await _horarioDisponibilidadeRepository.GetPagedAsync(query.Pagination, cancellationToken);

                if (horariosDisponiveis is null)
                {
                    _logger.LogWarning("Nenhum horario disponivel encontrado.");
                    return Result.Failure<PagedResponse<HorarioResponse>>("Nenhum horario disponivel encontrado.");
                }

                var response = horariosDisponiveis.ToPagedResponse(h => new HorarioResponse
                {
                    HorarioId = h.Id,
                    Horario = h.Hora,
                    Disponivel = h.DeletedAt is null
                });

                return Result.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro inesperado ao tentar buscar horarios disponiveis.");
                return Result.Failure<PagedResponse<HorarioResponse>>("Ocorreu um erro inesperado ao tentar buscar horarios disponiveis.");
            }
        }
    }
}