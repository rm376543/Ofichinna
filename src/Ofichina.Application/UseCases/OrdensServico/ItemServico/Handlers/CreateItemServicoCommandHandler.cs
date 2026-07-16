using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.OrdensServico.ItemServico.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Exceptions;
using Ofichina.Domain.Interfaces;

namespace Ofichina.Application.UseCases.OrdensServico.ItemServico.Handlers;

/// <summary>
/// Handler para criação de item de serviço.
/// </summary>
public sealed class CreateItemServicoCommandHandler : ICommandHandler<CreateItemServicoCommand, Result<Guid>>
{
    private readonly IOrdemServicoRepository _ordemServicoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateItemServicoCommandHandler> _logger;

    public CreateItemServicoCommandHandler(
        IOrdemServicoRepository ordemServicoRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateItemServicoCommandHandler> logger)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> HandleAsync(CreateItemServicoCommand command)
    {
        try
        {
            _logger.LogInformation("Iniciando criação de item de serviço. OrdemServicoId: {OrdemServicoId}.", command.OrdemServicoId);

            var ordemServico = await _ordemServicoRepository.GetByIdAsync(command.OrdemServicoId, includeItens: true);
            if (ordemServico is null || ordemServico.EstaExcluida())
                return Result.Failure<Guid>("Ordem de serviço não encontrada.");

            var item = ordemServico.AdicionarServico(command.Descricao, command.Valor);

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
