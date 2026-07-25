using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.Abstractions.Interfaces;
using Ofichina.Application.UseCases.ItensServico.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Exceptions;
using Ofichina.Domain.Common;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Enums;
using Ofichina.Application.Abstractions.Common;

namespace Ofichina.Application.UseCases.ItensServico.Handlers;

/// <summary>
/// Handler para atualizacao de item de servico.
/// </summary>
public sealed class UpdateItemServicoCommandHandler : ICommandHandler<UpdateItemServicoCommand, Result>
{
    private readonly IOrdemServicoRepository _ordemServicoRepository;
    private readonly IItemServicoRepository _itemServicoRepository;
    private readonly IRepository<ServicoPeca> _servicoPecaRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateItemServicoCommandHandler> _logger;

    public UpdateItemServicoCommandHandler(
        IOrdemServicoRepository ordemServicoRepository,
        IItemServicoRepository itemServicoRepository,
        IRepository<ServicoPeca> servicoPecaRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateItemServicoCommandHandler> logger)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _itemServicoRepository = itemServicoRepository;
        _servicoPecaRepository = servicoPecaRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(UpdateItemServicoCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Iniciando atualização de item de serviço. OrdemServicoId: {OrdemServicoId}, ItemServicoId: {ItemServicoId}.", command.OrdemServicoId, command.Id);

            var item = await _itemServicoRepository.GetByOrdemServicoIdAndItemServicoIdAsync(command.OrdemServicoId, command.Id, cancellationToken, tracking: true, includeRelacionados: true);
            if (item is null || item.EstaExcluida())
                return Result.Failure("Item de serviço não encontrado.");

            var ordemServico = await _ordemServicoRepository.GetByIdAsync(command.OrdemServicoId, cancellationToken);
            if (ordemServico is null || ordemServico.EstaExcluida())
                return Result.Failure("Ordem de serviço não encontrada.");

            if (ordemServico.Status != StatusOrdemServico.Recebida && ordemServico.Status != StatusOrdemServico.EmDiagnostico)
                return Result.Failure("Não é possível alterar itens nesta etapa da OS.");

            var servicoPeca = await _servicoPecaRepository.GetByIdAsync(command.ServicoPecaId, cancellationToken);
            if (servicoPeca is null || servicoPeca.EstaExcluida())
                return Result.Failure("Serviço/Peça não encontrada.");

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Item de serviço atualizado com sucesso. OrdemServicoId: {OrdemServicoId}, ItemServicoId: {ItemServicoId}.", command.OrdemServicoId, command.Id);
            return Result.Success();
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domínio ao atualizar item de serviço. OrdemServicoId: {OrdemServicoId}, ItemServicoId: {ItemServicoId}.", command.OrdemServicoId, command.Id);
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao atualizar item de serviço. OrdemServicoId: {OrdemServicoId}, ItemServicoId: {ItemServicoId}.", command.OrdemServicoId, command.Id);
            return Result.Failure("Não foi possível atualizar o item de serviço.");
        }
    }
}

