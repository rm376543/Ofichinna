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
/// Handler para criaÃ§Ã£o de item de serviÃ§o.
/// </summary>
public sealed class CreateItemServicoCommandHandler : ICommandHandler<CreateItemServicoCommand, Result<Guid>>
{
    private readonly IOrdemServicoRepository _ordemServicoRepository;
    private readonly IRepository<Servico> _servicoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateItemServicoCommandHandler> _logger;

    public CreateItemServicoCommandHandler(
        IOrdemServicoRepository ordemServicoRepository,
        IRepository<Servico> servicoRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateItemServicoCommandHandler> logger)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _servicoRepository = servicoRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> HandleAsync(CreateItemServicoCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Iniciando criaÃ§Ã£o de item de serviÃ§o. OrdemServicoId: {OrdemServicoId}, ServicoId: {ServicoId}.", command.OrdemServicoId, command.ServicoId);

            var ordemServico = await _ordemServicoRepository.GetByIdAsync(command.OrdemServicoId, includeItens: true, cancellationToken);
            if (ordemServico is null || ordemServico.EstaExcluida())
                return Result.Failure<Guid>("Ordem de serviÃ§o nÃ£o encontrada.");

            var servico = await _servicoRepository.GetByIdAsync(command.ServicoId, cancellationToken);
            if (servico is null)
                return Result.Failure<Guid>("ServiÃ§o nÃ£o encontrado.");

            if (servico.EstaExcluida())
                return Result.Failure<Guid>("ServiÃ§o inativo.");

            var item = ordemServico.AdicionarServico(command.ServicoId, servico.Nome, servico.Valor);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Item de serviÃ§o criado com sucesso. OrdemServicoId: {OrdemServicoId}, ServicoId: {ServicoId}, ItemServicoId: {ItemServicoId}.", command.OrdemServicoId, command.ServicoId, item.Id);
            return Result.Success(item.Id);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domÃ­nio ao criar item de serviÃ§o. OrdemServicoId: {OrdemServicoId}, ServicoId: {ServicoId}.", command.OrdemServicoId, command.ServicoId);
            return Result.Failure<Guid>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao criar item de serviÃ§o. OrdemServicoId: {OrdemServicoId}, ServicoId: {ServicoId}.", command.OrdemServicoId, command.ServicoId);
            return Result.Failure<Guid>("NÃ£o foi possÃ­vel criar o item de serviÃ§o.");
        }
    }
}

