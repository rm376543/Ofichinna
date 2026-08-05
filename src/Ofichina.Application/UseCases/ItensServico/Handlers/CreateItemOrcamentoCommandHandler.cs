using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.ItensServico.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Enums;
using Ofichina.Domain.Exceptions;

namespace Ofichina.Application.UseCases.ItensServico.Handlers;

/// <summary>
/// Handler para criação de item de serviço em um orçamento.
/// </summary>
public sealed class CreateItemOrcamentoCommandHandler : ICommandHandler<CreateItemOrcamentoCommand, Result>
{
    private readonly IOrcamentoRepository _orcamentoRepository;
    private readonly IItemServicoRepository _itemServicoRepository;
    private readonly IRepository<Servico> _servicoRepository;
    private readonly IRepository<Peca>? _pecaRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateItemOrcamentoCommandHandler> _logger;

    public CreateItemOrcamentoCommandHandler(
        IOrcamentoRepository orcamentoRepository,
        IItemServicoRepository itemServicoRepository,
        IRepository<Servico> servicoRepository,
        IRepository<Peca>? pecaRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateItemOrcamentoCommandHandler> logger)
    {
        _orcamentoRepository = orcamentoRepository;
        _servicoRepository = servicoRepository;
        _pecaRepository = pecaRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _itemServicoRepository = itemServicoRepository;
    }

    public async Task<Result> HandleAsync(CreateItemOrcamentoCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Iniciando criação de item de serviço. OrcamentoId: {OrcamentoId}.", command.OrcamentoId);

            var ordemServico = await _orcamentoRepository.GetByIdAsync(command.OrcamentoId, cancellationToken);
            if (ordemServico is null || ordemServico.EstaExcluida())
                return Result.Failure("Orçamento não encontrado.");

            if (ordemServico.Status != StatusOrcamento.EmDiagnostico)
                return Result.Failure("Não é possível alterar itens nesta etapa do orçamento.");

            var servico = await _servicoRepository.GetByIdAsync(command.ServicoId, cancellationToken, tracking: true);
            if (servico is null || servico.EstaExcluida())
                return Result.Failure("Serviço não encontrado.");

            if (command.PecaId.HasValue)
            {
                if (_pecaRepository is null)
                    return Result.Failure("Repositório de peças não está disponível.");
                var peca = await _pecaRepository.GetByIdAsync(command.PecaId.Value, cancellationToken, tracking: true);
                if (peca is null || peca.EstaExcluida())
                    return Result.Failure("Peça não encontrada.");
            }

            var existente = await _itemServicoRepository.GetByOrcamentoServicoPecaIdAsync(
                    command.OrcamentoId,
                    command.ServicoId,
                    command.PecaId,
                    cancellationToken,
                    tracking: true);

            if (existente is not null && !existente.EstaExcluida())
                return Result.Failure("Já existe um item de serviço com este serviço e esta peça vinculado ao orcamento.");

            var item = ItemServico.ParaOrcamento(
                command.OrcamentoId,
                command.ServicoId,
                command.PecaId,
                command.Quantidade);

            await _itemServicoRepository.AddAsync(item, cancellationToken);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Item de serviço criado com sucesso. OrcamentoId: {OrcamentoId}, ItemServicoId: {ItemServicoId}.", command.OrcamentoId, item.Id);
            return Result.Success();
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domínio ao criar item de serviço no orçamento. OrcamentoId: {OrcamentoId}.", command.OrcamentoId);
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao criar item de serviço no orçamento. OrcamentoId: {OrcamentoId}.", command.OrcamentoId);
            return Result.Failure("Ocorreu um erro ao criar o item de serviço.");
        }
    }
}
