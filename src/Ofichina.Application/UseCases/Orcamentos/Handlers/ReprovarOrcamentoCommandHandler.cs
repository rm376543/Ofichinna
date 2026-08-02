using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.Abstractions.Common;
using Ofichina.Application.Abstractions.Interfaces;
using Ofichina.Application.UseCases.Orcamentos.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Exceptions;

namespace Ofichina.Application.UseCases.Orcamentos.Handlers;

/// <summary>
/// Handler para reprovar orçamento.
/// </summary>
public sealed class ReprovarOrcamentoCommandHandler : ICommandHandler<ReprovarOrcamentoCommand, Result>
{
    private readonly IOrcamentoRepository _orcamentoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ReprovarOrcamentoCommandHandler> _logger;

    public ReprovarOrcamentoCommandHandler(
        IOrcamentoRepository orcamentoRepository,
        IUnitOfWork unitOfWork,
        ILogger<ReprovarOrcamentoCommandHandler> logger)
    {
        _orcamentoRepository = orcamentoRepository;
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

            orcamento.Reprovar();

            await _orcamentoRepository.UpdateAsync(orcamento, cancellationToken);
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
