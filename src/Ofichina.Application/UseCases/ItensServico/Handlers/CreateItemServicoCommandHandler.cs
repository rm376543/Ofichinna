using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Exceptions;
using Ofichina.Domain.Common;
using Ofichina.Application.Abstractions.Interfaces;
using Ofichina.Application.UseCases.ItensServico.Commands;
using Ofichina.Domain.Enums;

namespace Ofichina.Application.UseCases.ItensServico.Handlers;

/// <summary>
/// Handler para criacao de item de servico.
/// </summary>
public sealed class CreateItemServicoCommandHandler : ICommandHandler<CreateItemServicoCommand, Result<Guid>>
{
    private readonly IOrdemServicoRepository _ordemServicoRepository;
    private readonly IItemServicoRepository _itemServicoRepository;
    private readonly IRepository<ServicoPeca> _pecaServicoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateItemServicoCommandHandler> _logger;

    public CreateItemServicoCommandHandler(
        IOrdemServicoRepository ordemServicoRepository,
        IItemServicoRepository itemServicoRepository,
        IRepository<ServicoPeca> pecaServicoRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateItemServicoCommandHandler> logger)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _itemServicoRepository = itemServicoRepository;
        _pecaServicoRepository = pecaServicoRepository;
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
                var pecaServico = await _pecaServicoRepository.GetByIdAsync(pecaCommand.ServicoPecaId, cancellationToken, tracking: true);
                if (pecaServico is null || pecaServico.EstaExcluida())
                    return Result.Failure<Guid>("Peça de serviço não encontrada.");

                pecas.Add(pecaServico);
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

