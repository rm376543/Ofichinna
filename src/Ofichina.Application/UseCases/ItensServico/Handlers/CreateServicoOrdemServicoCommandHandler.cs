using Ofichina.Application.Abstractions;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Application.UseCases.ItensServico.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Enums;
using Ofichina.Domain.Exceptions;

namespace Ofichina.Application.UseCases.ItensServico.Handlers;

/// <summary>
/// Handler para criação de item de serviço somente-serviço em uma ordem de serviço.
/// </summary>
public sealed class CreateServicoOrdemServicoCommandHandler : ICommandHandler<CreateServicoOrdemServicoCommand, Result>
{
    private readonly IOrdemServicoRepository _ordemServicoRepository;
    private readonly IItemServicoRepository _itemServicoRepository;
    private readonly IRepository<Servico> _servicoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateServicoOrdemServicoCommandHandler> _logger;

    public CreateServicoOrdemServicoCommandHandler(
        IOrdemServicoRepository ordemServicoRepository,
        IItemServicoRepository itemServicoRepository,
        IRepository<Servico> servicoRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateServicoOrdemServicoCommandHandler> logger)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _itemServicoRepository = itemServicoRepository;
        _servicoRepository = servicoRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(CreateServicoOrdemServicoCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Iniciando criação de item de serviço somente-serviço. OrdemServicoId: {OrdemServicoId}.", command.OrdemServicoId);

            var ordemServico = await _ordemServicoRepository.GetByIdAsync(command.OrdemServicoId, cancellationToken);
            if (ordemServico is null || ordemServico.EstaExcluida())
                return Result.Failure("Ordem de serviço não encontrada.");

            if (ordemServico.Status != StatusOrdemServico.Recebida)
                return Result.Failure("Não é possível alterar itens nesta etapa da OS.");

            var servico = await _servicoRepository.GetByIdAsync(command.ServicoId, cancellationToken, tracking: true);
            if (servico is null || servico.EstaExcluida())
                return Result.Failure("Serviço não encontrado.");

            var existente = await _itemServicoRepository.GetByOrdemServicoSemPecaAsync(
                command.OrdemServicoId,
                command.ServicoId,
                cancellationToken,
                tracking: true);

            if (existente is not null && !existente.EstaExcluida())
                return Result.Failure("Já existe um item de serviço com este serviço vinculado à ordem.");

            var item = ItemServico.ParaOrdemServico(
                command.OrdemServicoId,
                command.ServicoId,
                pecaId: null,
                quantidade: 1);

            await _itemServicoRepository.AddAsync(item, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Item de serviço somente-serviço criado com sucesso. OrdemServicoId: {OrdemServicoId}, ItemServicoId: {ItemServicoId}.", command.OrdemServicoId, item.Id);
            return Result.Success();
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domínio ao criar item de serviço somente-serviço. OrdemServicoId: {OrdemServicoId}.", command.OrdemServicoId);
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao criar item de serviço somente-serviço. OrdemServicoId: {OrdemServicoId}.", command.OrdemServicoId);
            return Result.Failure("Não foi possível criar o item de serviço.");
        }
    }
}