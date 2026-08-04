using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.Abstractions.Authentication;
using Ofichina.Application.Abstractions.Common;
using Ofichina.Application.Abstractions.Interfaces;
using Ofichina.Application.UseCases.Orcamentos.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Common;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Exceptions;

namespace Ofichina.Application.UseCases.Orcamentos.Handlers;

/// <summary>
/// Handler para reprovar orçamento.
/// </summary>
public sealed class ReprovarOrcamentoCommandHandler : ICommandHandler<ReprovarOrcamentoCommand, Result>
{
    private readonly IOrcamentoRepository _orcamentoRepository;
    private readonly IRepository<MotivoRecusaOrcamento> _motivoRecusaRepository;
    private readonly IRepository<HistoricoStatus> _historicoStatusRepository;
    private readonly IUsuarioAtualService _usuarioAtualService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ReprovarOrcamentoCommandHandler> _logger;

    public ReprovarOrcamentoCommandHandler(
        IOrcamentoRepository orcamentoRepository,
        IRepository<MotivoRecusaOrcamento> motivoRecusaRepository,
        IRepository<HistoricoStatus> historicoStatusRepository,
        IUsuarioAtualService usuarioAtualService,
        IUnitOfWork unitOfWork,
        ILogger<ReprovarOrcamentoCommandHandler> logger)
    {
        _orcamentoRepository = orcamentoRepository;
        _motivoRecusaRepository = motivoRecusaRepository;
        _historicoStatusRepository = historicoStatusRepository;
        _usuarioAtualService = usuarioAtualService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(ReprovarOrcamentoCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var orcamento = await _orcamentoRepository.GetByIdAsync(command.Id, cancellationToken, tracking: true);
            if (orcamento is null || orcamento.EstaExcluida())
                return Result.Failure("Orçamento não encontrado.");

            var statusAnterior = orcamento.Status;
            orcamento.Reprovar();

            await _orcamentoRepository.UpdateAsync(orcamento, cancellationToken);

            if (!string.IsNullOrWhiteSpace(command.Motivo))
            {
                await _motivoRecusaRepository.AddAsync(
                    new MotivoRecusaOrcamento(orcamento.Id, command.Motivo),
                    cancellationToken);
            }

            await _historicoStatusRepository.AddAsync(
                HistoricoStatus.ParaOrcamento(
                    orcamento.Id,
                    statusAnterior.ToUpperSnakeCase(),
                    orcamento.Status.ToUpperSnakeCase(),
                    _usuarioAtualService.ObterUsuarioId()),
                cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domínio ao reprovar orçamento. OrcamentoId: {OrcamentoId}", command.Id);
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao reprovar orçamento. OrcamentoId: {OrcamentoId}", command.Id);
            return Result.Failure("Não foi possível reprovar o orçamento.");
        }
    }
}
