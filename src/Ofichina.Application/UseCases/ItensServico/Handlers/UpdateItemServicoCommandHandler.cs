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
    private readonly IRepository<Servico> _servicoRepository;
    private readonly IRepository<Peca> _pecaRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateItemServicoCommandHandler> _logger;

    public UpdateItemServicoCommandHandler(
        IOrdemServicoRepository ordemServicoRepository,
        IItemServicoRepository itemServicoRepository,
        IRepository<Servico> servicoRepository,
        IRepository<Peca> pecaRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateItemServicoCommandHandler> logger)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _itemServicoRepository = itemServicoRepository;
        _servicoRepository = servicoRepository;
        _pecaRepository = pecaRepository;
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

            if (ordemServico.Status != StatusOrdemServico.Recebida)
                return Result.Failure("Não é possível alterar itens nesta etapa da OS.");

            var servico = await _servicoRepository.GetByIdAsync(command.ServicoId, cancellationToken, tracking: true);
            if (servico is null || servico.EstaExcluida())
                return Result.Failure("Serviço não encontrado.");

            var peca = await _pecaRepository.GetByIdAsync(command.PecaId, cancellationToken, tracking: true);
            if (peca is null || peca.EstaExcluida())
                return Result.Failure("Peça não encontrada.");

            var duplicado = await _itemServicoRepository.GetByOrdemServicoIdAndServicoIdAndPecaIdAsync(
                command.OrdemServicoId,
                command.ServicoId,
                command.PecaId,
                cancellationToken,
                tracking: true);

            if (duplicado is not null && duplicado.Id != command.Id && !duplicado.EstaExcluida())
                return Result.Failure("Já existe um item de serviço com este serviço e esta peça na ordem.");

            item.AtualizarDados(command.ServicoId, command.PecaId, command.Quantidade);

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

