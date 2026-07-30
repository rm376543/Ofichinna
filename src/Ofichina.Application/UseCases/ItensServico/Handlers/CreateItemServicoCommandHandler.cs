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
public sealed class CreateItemServicoCommandHandler : ICommandHandler<CreateItemServicoCommand, Result<Guid>>
{
    private readonly IOrdemServicoRepository _ordemServicoRepository;
    private readonly IItemServicoRepository _itemServicoRepository;
    private readonly IRepository<ServicoPeca> _servicoPecaRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateItemServicoCommandHandler> _logger;

    public CreateItemServicoCommandHandler(
        IOrdemServicoRepository ordemServicoRepository,
        IItemServicoRepository itemServicoRepository,
        IRepository<ServicoPeca> servicoPecaRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateItemServicoCommandHandler> logger)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _itemServicoRepository = itemServicoRepository;
        _servicoPecaRepository = servicoPecaRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> HandleAsync(CreateItemServicoCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Iniciando criação de item de serviço. OrdemServicoId: {OrdemServicoId}.", command.OrdemServicoId);

            var ordemServico = await _ordemServicoRepository.GetByIdAsync(command.OrdemServicoId, cancellationToken);
            if (ordemServico is null || ordemServico.EstaExcluida())
                return Result.Failure<Guid>("Ordem de serviço não encontrada.");

            if (ordemServico.Status != StatusOrdemServico.Recebida && ordemServico.Status != StatusOrdemServico.EmDiagnostico)
                return Result.Failure<Guid>("Não é possível alterar itens nesta etapa da OS.");

            var pecas = new List<ServicoPeca>();
            foreach (var pecaCommand in command.Pecas)
            {
                var servicoPeca = await _servicoPecaRepository.GetByIdAsync(pecaCommand.ServicoPecaId, cancellationToken, tracking: true);
                if (servicoPeca is null || servicoPeca.EstaExcluida())
                    return Result.Failure<Guid>("Peça de serviço não encontrada.");

                pecas.Add(servicoPeca);
            }

            var item = new ItemServico(command.OrdemServicoId);
            foreach (var pecaCommand in command.Pecas)
            {
                var peca = pecas.First(x => x.Id == pecaCommand.ServicoPecaId);
                var quantidade = pecaCommand.Quantidade;
                item.AdicionarPeca(peca, quantidade);
            }

            await _itemServicoRepository.AddAsync(item, cancellationToken);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Item de serviço criado com sucesso. OrdemServicoId: {OrdemServicoId}, ItemServicoId: {ItemServicoId}.", command.OrdemServicoId, item.Id);
            return Result.Success(item.Id);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domínio ao criar item de serviço. OrdemServicoId: {OrdemServicoId}.", command.OrdemServicoId);
            return Result.Failure<Guid>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao criar item de serviço. OrdemServicoId: {OrdemServicoId}.", command.OrdemServicoId);
            return Result.Failure<Guid>("Não foi possível criar o item de serviço.");
        }
    }
}

