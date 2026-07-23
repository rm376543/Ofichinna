using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.Abstractions.Interfaces;
using Ofichina.Application.UseCases.ItensServico.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Exceptions;
using Ofichina.Domain.Common;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Enums;

namespace Ofichina.Application.UseCases.ItensServico.Handlers;

/// <summary>
/// Handler para atualizacao de item de servico.
/// </summary>
public sealed class UpdateItemServicoCommandHandler : ICommandHandler<UpdateItemServicoCommand, Result>
{
    private readonly IOrdemServicoRepository _ordemServicoRepository;
    private readonly IItemServicoRepository _itemServicoRepository;
    private readonly IRepository<ServicoPeca> _pecaServicoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateItemServicoCommandHandler> _logger;

    public UpdateItemServicoCommandHandler(
        IOrdemServicoRepository ordemServicoRepository,
        IItemServicoRepository itemServicoRepository,
        IRepository<ServicoPeca> pecaServicoRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateItemServicoCommandHandler> logger)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _itemServicoRepository = itemServicoRepository;
        _pecaServicoRepository = pecaServicoRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(UpdateItemServicoCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Iniciando atualização de item de serviço. OrdemServicoId: {OrdemServicoId}, ItemServicoId: {ItemServicoId}.", command.OrdemServicoId, command.Id);

            var ordemServico = await _ordemServicoRepository.GetByIdAsync(command.OrdemServicoId, cancellationToken);
            if (ordemServico is null || ordemServico.EstaExcluida())
                return Result.Failure("Ordem de serviço não encontrada.");

            if (ordemServico.Status != StatusOrdemServico.Recebida && ordemServico.Status != StatusOrdemServico.EmDiagnostico)
                return Result.Failure("Não é possível alterar itens nesta etapa da OS.");

            var item = await _itemServicoRepository.GetByOrdemServicoIdAndIdAsync(command.OrdemServicoId, command.Id, cancellationToken, tracking: true, includeRelacionados: true);
            if (item is null || item.EstaExcluida())
                return Result.Failure("Item de serviço não encontrado.");

            List<ServicoPeca> novasPecas = new();
            foreach (var pecaCommand in command.Pecas)
            {
                ServicoPeca? pecaServico = await _pecaServicoRepository.GetByIdAsync(pecaCommand.ServicoPecaId, cancellationToken, tracking: true);
                if (pecaServico is null || pecaServico.EstaExcluida())
                    return Result.Failure("Peça de serviço não encontrada.");

                novasPecas.Add(pecaServico);
            }

            item.SubstituirPecas(novasPecas);

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

