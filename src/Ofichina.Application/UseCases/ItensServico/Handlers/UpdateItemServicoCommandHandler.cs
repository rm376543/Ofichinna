using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.Abstractions.Interfaces;
using Ofichina.Application.UseCases.ItemServico.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Exceptions;
using Ofichina.Domain.Common;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Enums;

namespace Ofichina.Application.UseCases.ItemServico.Handlers;

/// <summary>
/// Handler para atualizacao de item de servico.
/// </summary>
public sealed class UpdateItemServicoCommandHandler : ICommandHandler<UpdateItemServicoCommand, Result>
{
    private readonly IOrdemServicoRepository _ordemServicoRepository;
    private readonly IItemServicoRepository _itemServicoRepository;
    private readonly IRepository<PecaServico> _pecaServicoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateItemServicoCommandHandler> _logger;

    public UpdateItemServicoCommandHandler(
        IOrdemServicoRepository ordemServicoRepository,
        IItemServicoRepository itemServicoRepository,
        IRepository<PecaServico> pecaServicoRepository,
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
            _logger.LogInformation("Iniciando atualização de item de serviço. OrdemServicoId: {OrdemServicoId}, ItemServicoId: {ItemServicoId}, PecaServicoId: {PecaServicoId}.", command.OrdemServicoId, command.Id, command.PecaServicoId);

            var ordemServico = await _ordemServicoRepository.GetByIdAsync(command.OrdemServicoId, cancellationToken);
            if (ordemServico is null || ordemServico.EstaExcluida())
                return Result.Failure("Ordem de serviço não encontrada.");

            if (ordemServico.Status != StatusOrdemServico.Recebida && ordemServico.Status != StatusOrdemServico.EmDiagnostico)
                return Result.Failure("Não é possível alterar itens nesta etapa da OS.");

            var item = await _itemServicoRepository.GetByOrdemServicoIdAndIdAsync(command.OrdemServicoId, command.Id, cancellationToken, tracking: true, includeRelacionados: true);
            if (item is null || item.EstaExcluida())
                return Result.Failure("Item de serviço não encontrado.");

            var pecaServicoAtual = item.PecaServico;
            if (pecaServicoAtual is null || pecaServicoAtual.EstaExcluida())
                return Result.Failure("Peça de serviço não encontrada.");

            if (command.PecaServicoId == item.PecaServicoId)
                return Result.Success();

            var novaPecaServico = await _pecaServicoRepository.GetByIdAsync(command.PecaServicoId, cancellationToken, tracking: true);
            if (novaPecaServico is null || novaPecaServico.EstaExcluida())
                return Result.Failure("Peça de serviço não encontrada.");

            item.AtualizarServico(novaPecaServico.Id);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Item de serviço atualizado com sucesso. OrdemServicoId: {OrdemServicoId}, ItemServicoId: {ItemServicoId}, PecaServicoId: {PecaServicoId}.", command.OrdemServicoId, command.Id, command.PecaServicoId);
            return Result.Success();
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domínio ao atualizar item de serviço. OrdemServicoId: {OrdemServicoId}, ItemServicoId: {ItemServicoId}, PecaServicoId: {PecaServicoId}.", command.OrdemServicoId, command.Id, command.PecaServicoId);
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao atualizar item de serviço. OrdemServicoId: {OrdemServicoId}, ItemServicoId: {ItemServicoId}, PecaServicoId: {PecaServicoId}.", command.OrdemServicoId, command.Id, command.PecaServicoId);
            return Result.Failure("Não foi possível atualizar o item de serviço.");
        }
    }
}

