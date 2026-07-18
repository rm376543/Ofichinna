using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.OrdensServico.ItemServico.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Exceptions;
using Ofichina.Domain.Interfaces;

namespace Ofichina.Application.UseCases.OrdensServico.ItemServico.Handlers;

/// <summary>
/// Handler para atualizaÃ§Ã£o de item de serviÃ§o.
/// </summary>
public sealed class UpdateItemServicoCommandHandler : ICommandHandler<UpdateItemServicoCommand, Result>
{
    private readonly IOrdemServicoRepository _ordemServicoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateItemServicoCommandHandler> _logger;

    public UpdateItemServicoCommandHandler(
        IOrdemServicoRepository ordemServicoRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateItemServicoCommandHandler> logger)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(UpdateItemServicoCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Iniciando atualizaÃ§Ã£o de item de serviÃ§o. OrdemServicoId: {OrdemServicoId}, ItemServicoId: {ItemServicoId}.", command.OrdemServicoId, command.Id);

            var ordemServico = await _ordemServicoRepository.GetByIdAsync(command.OrdemServicoId, includeItens: true, cancellationToken);
            if (ordemServico is null || ordemServico.EstaExcluida())
                return Result.Failure("Ordem de serviÃ§o nÃ£o encontrada.");

            ordemServico.AtualizarServico(command.Id, command.Descricao, command.Valor);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Item de serviÃ§o atualizado com sucesso. OrdemServicoId: {OrdemServicoId}, ItemServicoId: {ItemServicoId}.", command.OrdemServicoId, command.Id);
            return Result.Success();
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domÃ­nio ao atualizar item de serviÃ§o. OrdemServicoId: {OrdemServicoId}, ItemServicoId: {ItemServicoId}.", command.OrdemServicoId, command.Id);
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao atualizar item de serviÃ§o. OrdemServicoId: {OrdemServicoId}, ItemServicoId: {ItemServicoId}.", command.OrdemServicoId, command.Id);
            return Result.Failure("NÃ£o foi possÃ­vel atualizar o item de serviÃ§o.");
        }
    }
}

