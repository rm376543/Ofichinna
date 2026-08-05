using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.ItensServico.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Enums;
using Ofichina.Domain.Exceptions;

namespace Ofichina.Application.UseCases.ItensServico.Handlers;

/// <summary>
/// Handler para criacao de item de servico.
/// </summary>
public sealed class CreateItemServicoCommandHandler : ICommandHandler<CreateItemServicoCommand, Result>
{
    private readonly IOrdemServicoRepository _ordemServicoRepository;
    private readonly IItemServicoRepository _itemServicoRepository;
    private readonly IRepository<Servico> _servicoRepository;
    private readonly IRepository<Peca> _pecaRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateItemServicoCommandHandler> _logger;

    public CreateItemServicoCommandHandler(
        IOrdemServicoRepository ordemServicoRepository,
        IItemServicoRepository itemServicoRepository,
        IRepository<Servico> servicoRepository,
        IRepository<Peca> pecaRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateItemServicoCommandHandler> logger)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _itemServicoRepository = itemServicoRepository;
        _servicoRepository = servicoRepository;
        _pecaRepository = pecaRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(CreateItemServicoCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Iniciando criação de item de serviço. OrdemServicoId: {OrdemServicoId}.", command.OrdemServicoId);

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

            var existente = await _itemServicoRepository.GetByOrdemServicoIdAndServicoIdAndPecaIdAsync(
                command.OrdemServicoId,
                command.ServicoId,
                command.PecaId,
                cancellationToken,
                tracking: true);

            if (existente is not null && !existente.EstaExcluida())
                return Result.Failure("Já existe um item de serviço com este serviço e esta peça na ordem.");

            var item = ItemServico.ParaOrdemServico(
                command.OrdemServicoId,
                command.ServicoId,
                command.PecaId,
                command.Quantidade);

            await _itemServicoRepository.AddAsync(item, cancellationToken);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Item de serviço criado com sucesso. OrdemServicoId: {OrdemServicoId}, ItemServicoId: {ItemServicoId}.", command.OrdemServicoId, item.Id);
            return Result.Success();
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domínio ao criar item de serviço. OrdemServicoId: {OrdemServicoId}.", command.OrdemServicoId);
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao criar item de serviço. OrdemServicoId: {OrdemServicoId}.", command.OrdemServicoId);
            return Result.Failure("Não foi possível criar o item de serviço.");
        }
    }
}

