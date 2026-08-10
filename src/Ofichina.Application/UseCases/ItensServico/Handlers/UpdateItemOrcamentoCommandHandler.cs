using Ofichina.Application.Abstractions;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Application.UseCases.ItensServico.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Enums;
using Ofichina.Domain.Exceptions;

namespace Ofichina.Application.UseCases.ItensServico.Handlers;

/// <summary>
/// Handler para atualização de item de serviço do orçamento.
/// </summary>
public sealed class UpdateItemOrcamentoCommandHandler : ICommandHandler<UpdateItemOrcamentoCommand, Result>
{
    private readonly IOrcamentoRepository _orcamentoRepository;
    private readonly IItemServicoRepository _itemServicoRepository;
    private readonly IRepository<Servico> _servicoRepository;
    private readonly IRepository<Peca> _pecaRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateItemOrcamentoCommandHandler> _logger;

    public UpdateItemOrcamentoCommandHandler(
        IOrcamentoRepository orcamentoRepository,
        IItemServicoRepository itemServicoRepository,
        IRepository<Servico> servicoRepository,
        IRepository<Peca> pecaRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateItemOrcamentoCommandHandler> logger)
    {
        _orcamentoRepository = orcamentoRepository;
        _itemServicoRepository = itemServicoRepository;
        _servicoRepository = servicoRepository;
        _pecaRepository = pecaRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(UpdateItemOrcamentoCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Iniciando atualização de item de serviço do orçamento. OrcamentoId: {OrcamentoId}, ItemServicoId: {ItemServicoId}.", command.OrcamentoId, command.ItemServicoId);

            var item = await _itemServicoRepository.GetByOrcamentoIdAndItemServicoIdAsync(command.OrcamentoId, command.ItemServicoId, cancellationToken, includeRelacionados: true, tracking: true);
            if (item is null || item.EstaExcluida())
                return Result.Failure("Item de serviço não encontrado.");

            var orcamento = await _orcamentoRepository.GetByIdAsync(command.OrcamentoId, includeItens: true, cancellationToken: cancellationToken, tracking: true);
            if (orcamento is null || orcamento.EstaExcluida())
                return Result.Failure("Orçamento não encontrado.");

            if (orcamento.Status != StatusOrcamento.EmDiagnostico)
                return Result.Failure("Não é possível alterar itens nesta etapa do orçamento.");

            var servico = await _servicoRepository.GetByIdAsync(command.ServicoId, cancellationToken, tracking: true);
            if (servico is null || servico.EstaExcluida())
                return Result.Failure("Serviço não encontrado.");

            if (command.PecaId.HasValue)
            {
                var peca = await _pecaRepository.GetByIdAsync(command.PecaId.Value, cancellationToken, tracking: true);
                if (peca is null || peca.EstaExcluida())
                    return Result.Failure("Peça não encontrada.");
            }

            var duplicado = await _itemServicoRepository.GetByOrcamentoServicoPecaIdAsync(
                command.OrcamentoId,
                command.ServicoId,
                command.PecaId,
                cancellationToken,
                tracking: true);

            if (duplicado is not null && duplicado.Id != command.ItemServicoId && !duplicado.EstaExcluida())
                return Result.Failure("Já existe um item de serviço com este serviço e esta peça no orçamento.");

            item.AtualizarDados(command.ServicoId, command.PecaId, command.Quantidade);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Item de serviço do orçamento atualizado com sucesso. OrcamentoId: {OrcamentoId}, ItemServicoId: {ItemServicoId}.", command.OrcamentoId, command.ItemServicoId);
            return Result.Success();
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domínio ao atualizar item de serviço do orçamento. OrcamentoId: {OrcamentoId}, ItemServicoId: {ItemServicoId}.", command.OrcamentoId, command.ItemServicoId);
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao atualizar item de serviço do orçamento. OrcamentoId: {OrcamentoId}, ItemServicoId: {ItemServicoId}.", command.OrcamentoId, command.ItemServicoId);
            return Result.Failure("Não foi possível atualizar o item de serviço do orçamento.");
        }
    }
}
