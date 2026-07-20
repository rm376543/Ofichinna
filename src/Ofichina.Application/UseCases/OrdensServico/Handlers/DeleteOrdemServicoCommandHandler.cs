using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.OrdensServico.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Common;

namespace Ofichina.Application.UseCases.OrdensServico.Handlers;

/// <summary>
/// Handler para remoÃ§Ã£o lÃ³gica de ordem de serviÃ§o.
/// </summary>
public sealed class DeleteOrdemServicoCommandHandler : ICommandHandler<DeleteOrdemServicoCommand, Result>
{
    private readonly IRepository<OrdemServico> _ordemServicoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteOrdemServicoCommandHandler> _logger;

    public DeleteOrdemServicoCommandHandler(
        IRepository<OrdemServico> ordemServicoRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteOrdemServicoCommandHandler> logger)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(DeleteOrdemServicoCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Iniciando remoÃ§Ã£o da ordem de serviÃ§o. OrdemServicoId: {OrdemServicoId}.", command.Id);

            var ordemServico = await _ordemServicoRepository.GetByIdAsync(command.Id, cancellationToken);
            if (ordemServico is null || ordemServico.EstaExcluida())
                return Result.Failure("Ordem de serviÃ§o nÃ£o encontrada.");

            ordemServico.Excluir();

            await _ordemServicoRepository.UpdateAsync(ordemServico, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Ordem de serviÃ§o removida com sucesso. OrdemServicoId: {OrdemServicoId}", command.Id);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao remover ordem de serviÃ§o. OrdemServicoId: {OrdemServicoId}", command.Id);
            return Result.Failure("NÃ£o foi possÃ­vel remover a ordem de serviÃ§o.");
        }
    }
}

