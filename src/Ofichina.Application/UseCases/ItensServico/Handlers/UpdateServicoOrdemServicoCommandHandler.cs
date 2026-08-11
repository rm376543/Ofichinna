using Ofichina.Application.Abstractions;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Application.UseCases.ItensServico.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Enums;
using Ofichina.Domain.Exceptions;

namespace Ofichina.Application.UseCases.ItensServico.Handlers;

/// <summary>
/// Handler para atualização de item de serviço somente-serviço em uma ordem de serviço.
/// </summary>
public sealed class UpdateServicoOrdemServicoCommandHandler : ICommandHandler<UpdateServicoOrdemServicoCommand, Result>
{
    private readonly IOrdemServicoRepository _ordemServicoRepository;
    private readonly IItemServicoRepository _itemServicoRepository;
    private readonly IRepository<Servico> _servicoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateServicoOrdemServicoCommandHandler> _logger;

    public UpdateServicoOrdemServicoCommandHandler(
        IOrdemServicoRepository ordemServicoRepository,
        IItemServicoRepository itemServicoRepository,
        IRepository<Servico> servicoRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateServicoOrdemServicoCommandHandler> logger)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _itemServicoRepository = itemServicoRepository;
        _servicoRepository = servicoRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(UpdateServicoOrdemServicoCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Iniciando atualização de item de serviço somente-serviço. OrdemServicoId: {OrdemServicoId}, ItemServicoId: {ItemServicoId}.", command.OrdemServicoId, command.ItemServicoId);

            var item = await _itemServicoRepository.GetByOrdemServicoIdAndItemServicoIdAsync(command.OrdemServicoId, command.ItemServicoId, cancellationToken, tracking: true, includeRelacionados: true);
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

            var duplicado = await _itemServicoRepository.GetByOrdemServicoSemPecaAsync(
                command.OrdemServicoId,
                command.ServicoId,
                cancellationToken,
                tracking: true);

            if (duplicado is not null && duplicado.Id != command.ItemServicoId && !duplicado.EstaExcluida())
                return Result.Failure("Já existe um item de serviço com este serviço vinculado à ordem.");

            var quantidade = item.Quantidade > 0 ? item.Quantidade : 1;
            item.AtualizarDados(command.ServicoId, pecaId: null, quantidade);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Item de serviço somente-serviço atualizado com sucesso. OrdemServicoId: {OrdemServicoId}, ItemServicoId: {ItemServicoId}.", command.OrdemServicoId, command.ItemServicoId);
            return Result.Success();
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domínio ao atualizar item de serviço somente-serviço. OrdemServicoId: {OrdemServicoId}, ItemServicoId: {ItemServicoId}.", command.OrdemServicoId, command.ItemServicoId);
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao atualizar item de serviço somente-serviço. OrdemServicoId: {OrdemServicoId}, ItemServicoId: {ItemServicoId}.", command.OrdemServicoId, command.ItemServicoId);
            return Result.Failure("Não foi possível atualizar o item de serviço.");
        }
    }
}