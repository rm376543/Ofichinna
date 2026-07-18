using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.OrdensServico.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Exceptions;
using Ofichina.Domain.Interfaces;

namespace Ofichina.Application.UseCases.OrdensServico.Handlers;

/// <summary>
/// Handler para atualizaÃ§Ã£o de ordem de serviÃ§o.
/// </summary>
public sealed class UpdateOrdemServicoCommandHandler : ICommandHandler<UpdateOrdemServicoCommand, Result>
{
    private readonly IRepository<OrdemServico> _ordemServicoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateOrdemServicoCommandHandler> _logger;

    public UpdateOrdemServicoCommandHandler(
        IRepository<OrdemServico> ordemServicoRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateOrdemServicoCommandHandler> logger)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(UpdateOrdemServicoCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Iniciando atualizaÃ§Ã£o da ordem de serviÃ§o. OrdemServicoId: {OrdemServicoId}.", command.Id);

            var ordemServico = await _ordemServicoRepository.GetByIdAsync(command.Id, cancellationToken);
            if (ordemServico is null || ordemServico.EstaExcluida())
                return Result.Failure("Ordem de serviÃ§o nÃ£o encontrada.");

            ordemServico.AtualizarAtendimento(command.FuncionarioId, command.Observacoes);

            await _ordemServicoRepository.UpdateAsync(ordemServico, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Ordem de serviÃ§o atualizada com sucesso. OrdemServicoId: {OrdemServicoId}", command.Id);
            return Result.Success();
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domÃ­nio ao atualizar ordem de serviÃ§o. OrdemServicoId: {OrdemServicoId}", command.Id);
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao atualizar ordem de serviÃ§o. OrdemServicoId: {OrdemServicoId}", command.Id);
            return Result.Failure("NÃ£o foi possÃ­vel atualizar a ordem de serviÃ§o.");
        }
    }
}

