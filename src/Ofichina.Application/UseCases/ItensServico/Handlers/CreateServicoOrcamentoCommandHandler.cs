using Ofichina.Application.Abstractions;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Application.UseCases.ItensServico.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Enums;
using Ofichina.Domain.Exceptions;

namespace Ofichina.Application.UseCases.ItensServico.Handlers;

/// <summary>
/// Handler para criação de item de serviço somente-serviço em um orçamento.
/// </summary>
public sealed class CreateServicoOrcamentoCommandHandler : ICommandHandler<CreateServicoOrcamentoCommand, Result>
{
    private readonly IOrcamentoRepository _orcamentoRepository;
    private readonly IItemServicoRepository _itemServicoRepository;
    private readonly IRepository<Servico> _servicoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateServicoOrcamentoCommandHandler> _logger;

    public CreateServicoOrcamentoCommandHandler(
        IOrcamentoRepository orcamentoRepository,
        IItemServicoRepository itemServicoRepository,
        IRepository<Servico> servicoRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateServicoOrcamentoCommandHandler> logger)
    {
        _orcamentoRepository = orcamentoRepository;
        _itemServicoRepository = itemServicoRepository;
        _servicoRepository = servicoRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(CreateServicoOrcamentoCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Iniciando criação de item de serviço somente-serviço. OrcamentoId: {OrcamentoId}.", command.OrcamentoId);

            var orcamento = await _orcamentoRepository.GetByIdAsync(command.OrcamentoId, cancellationToken);
            if (orcamento is null || orcamento.EstaExcluida())
                return Result.Failure("Orçamento não encontrado.");

            if (orcamento.Status != StatusOrcamento.EmDiagnostico)
                return Result.Failure("Não é possível alterar itens nesta etapa do orçamento.");

            var servico = await _servicoRepository.GetByIdAsync(command.ServicoId, cancellationToken, tracking: true);
            if (servico is null || servico.EstaExcluida())
                return Result.Failure("Serviço não encontrado.");

            var existente = await _itemServicoRepository.GetByOrcamentoSemPecaAsync(
                command.OrcamentoId,
                command.ServicoId,
                cancellationToken,
                tracking: true);

            if (existente is not null && !existente.EstaExcluida())
                return Result.Failure("Já existe um item de serviço com este serviço vinculado ao orcamento.");

            var item = ItemServico.ParaOrcamento(
                command.OrcamentoId,
                command.ServicoId,
                pecaId: null,
                quantidade: 1);

            await _itemServicoRepository.AddAsync(item, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Item de serviço somente-serviço criado com sucesso. OrcamentoId: {OrcamentoId}, ItemServicoId: {ItemServicoId}.", command.OrcamentoId, item.Id);
            return Result.Success();
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domínio ao criar item de serviço somente-serviço no orçamento. OrcamentoId: {OrcamentoId}.", command.OrcamentoId);
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao criar item de serviço somente-serviço no orçamento. OrcamentoId: {OrcamentoId}.", command.OrcamentoId);
            return Result.Failure("Ocorreu um erro ao criar o item de serviço.");
        }
    }
}