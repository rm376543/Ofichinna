using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.OrdensServico.ItemServico.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Exceptions;
using Ofichina.Application.Abstractions.Interfaces;
using Ofichina.Domain.Common;

namespace Ofichina.Application.UseCases.OrdensServico.ItemServico.Handlers;

/// <summary>
/// Handler para remocao logica de item de servico.
/// </summary>
public sealed class DeleteItemServicoCommandHandler : ICommandHandler<DeleteItemServicoCommand, Result>
{
    private readonly IOrdemServicoRepository _ordemServicoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteItemServicoCommandHandler> _logger;

    public DeleteItemServicoCommandHandler(
        IOrdemServicoRepository ordemServicoRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteItemServicoCommandHandler> logger)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(DeleteItemServicoCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Iniciando remoção de item de serviço. OrdemServicoId: {OrdemServicoId}, ItemServicoId: {ItemServicoId}.", command.OrdemServicoId, command.Id);

            var ordemServico = await _ordemServicoRepository.GetByIdAsync(command.OrdemServicoId, includeItens: true, cancellationToken);
            if (ordemServico is null || ordemServico.EstaExcluida())
                return Result.Failure("Ordem de serviço não encontrada.");

            ordemServico.RemoverServico(command.Id);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Item de serviço removido com sucesso. OrdemServicoId: {OrdemServicoId}, ItemServicoId: {ItemServicoId}.", command.OrdemServicoId, command.Id);
            return Result.Success();
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domínio ao remover item de serviço. OrdemServicoId: {OrdemServicoId}, ItemServicoId: {ItemServicoId}.", command.OrdemServicoId, command.Id);
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao remover item de serviço. OrdemServicoId: {OrdemServicoId}, ItemServicoId: {ItemServicoId}.", command.OrdemServicoId, command.Id);
            return Result.Failure("Não foi possível remover o item de serviço.");
        }
    }
}

