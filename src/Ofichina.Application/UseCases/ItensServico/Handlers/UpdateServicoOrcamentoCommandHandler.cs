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
/// Handler para atualização de item de serviço somente-serviço de um orçamento.
/// </summary>
public sealed class UpdateServicoOrcamentoCommandHandler : ICommandHandler<UpdateServicoOrcamentoCommand, Result>
{
    private readonly IOrcamentoRepository _orcamentoRepository;
    private readonly IItemServicoRepository _itemServicoRepository;
    private readonly IRepository<Servico> _servicoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateServicoOrcamentoCommandHandler> _logger;

    public UpdateServicoOrcamentoCommandHandler(
        IOrcamentoRepository orcamentoRepository,
        IItemServicoRepository itemServicoRepository,
        IRepository<Servico> servicoRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateServicoOrcamentoCommandHandler> logger)
    {
        _orcamentoRepository = orcamentoRepository;
        _itemServicoRepository = itemServicoRepository;
        _servicoRepository = servicoRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(UpdateServicoOrcamentoCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Iniciando atualização de item de serviço somente-serviço do orçamento. OrcamentoId: {OrcamentoId}, ItemServicoId: {ItemServicoId}.", command.OrcamentoId, command.ItemServicoId);

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

            var duplicado = await _itemServicoRepository.GetByOrcamentoSemPecaAsync(
                command.OrcamentoId,
                command.ServicoId,
                cancellationToken,
                tracking: true);

            if (duplicado is not null && duplicado.Id != command.ItemServicoId && !duplicado.EstaExcluida())
                return Result.Failure("Já existe um item de serviço com este serviço vinculado ao orçamento.");

            var quantidade = item.Quantidade > 0 ? item.Quantidade : 1;
            item.AtualizarDados(command.ServicoId, pecaId: null, quantidade);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Item de serviço somente-serviço do orçamento atualizado com sucesso. OrcamentoId: {OrcamentoId}, ItemServicoId: {ItemServicoId}.", command.OrcamentoId, command.ItemServicoId);
            return Result.Success();
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domínio ao atualizar item de serviço somente-serviço do orçamento. OrcamentoId: {OrcamentoId}, ItemServicoId: {ItemServicoId}.", command.OrcamentoId, command.ItemServicoId);
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao atualizar item de serviço somente-serviço do orçamento. OrcamentoId: {OrcamentoId}, ItemServicoId: {ItemServicoId}.", command.OrcamentoId, command.ItemServicoId);
            return Result.Failure("Não foi possível atualizar o item de serviço do orçamento.");
        }
    }
}