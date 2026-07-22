using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.Abstractions.Interfaces;
using Ofichina.Application.UseCases.OrdensServico.ItemServico.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Common;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Exceptions;
using DomainPeca = Ofichina.Domain.Entities.Peca;

namespace Ofichina.Application.UseCases.OrdensServico.ItemServico.Handlers;

/// <summary>
/// Handler para adicionar peça a um item de serviço.
/// </summary>
public sealed class CreateItemServicoPecaCommandHandler : ICommandHandler<CreateItemServicoPecaCommand, Result<Guid>>
{
    private readonly IOrdemServicoRepository _ordemServicoRepository;
    private readonly IRepository<DomainPeca> _pecaRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateItemServicoPecaCommandHandler> _logger;

    public CreateItemServicoPecaCommandHandler(
        IOrdemServicoRepository ordemServicoRepository,
        IRepository<DomainPeca> pecaRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateItemServicoPecaCommandHandler> logger)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _pecaRepository = pecaRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> HandleAsync(CreateItemServicoPecaCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Iniciando inclusão de peça no item de serviço. OrdemServicoId: {OrdemServicoId}, ItemServicoId: {ItemServicoId}, PecaId: {PecaId}.", command.OrdemServicoId, command.ItemServicoId, command.PecaId);

            var ordemServico = await _ordemServicoRepository.GetByIdAsync(command.OrdemServicoId, includeItens: true, cancellationToken, tracking: true);
            if (ordemServico is null || ordemServico.EstaExcluida())
                return Result.Failure<Guid>("Ordem de serviço não encontrada.");

            var itemServico = ordemServico.ObterServico(command.ItemServicoId);
            if (itemServico is null || itemServico.EstaExcluida())
                return Result.Failure<Guid>("Item de serviço não encontrado.");

            var peca = await _pecaRepository.GetByIdAsync(command.PecaId, cancellationToken);
            if (peca is null || peca.EstaExcluida())
                return Result.Failure<Guid>("Peça não encontrada.");

            ordemServico.AdicionarPeca(
                command.ItemServicoId,
                peca.Id,
                peca.Nome,
                command.Quantidade,
                peca.Valor);

            await _unitOfWork.SaveChangesAsync();

            var pecaAdicionada = ordemServico.ObterServico(command.ItemServicoId)?.Pecas
                .FirstOrDefault(x => x.PecaId == command.PecaId && !x.EstaExcluida());

            _logger.LogInformation("Peça adicionada com sucesso ao item de serviço. OrdemServicoId: {OrdemServicoId}, ItemServicoId: {ItemServicoId}, PecaId: {PecaId}, ItemPecaId: {ItemPecaId}.", command.OrdemServicoId, command.ItemServicoId, command.PecaId, pecaAdicionada?.Id);

            return Result.Success(pecaAdicionada?.Id ?? Guid.Empty);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domínio ao adicionar peça ao item de serviço. OrdemServicoId: {OrdemServicoId}, ItemServicoId: {ItemServicoId}, PecaId: {PecaId}.", command.OrdemServicoId, command.ItemServicoId, command.PecaId);
            return Result.Failure<Guid>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao adicionar peça ao item de serviço. OrdemServicoId: {OrdemServicoId}, ItemServicoId: {ItemServicoId}, PecaId: {PecaId}.", command.OrdemServicoId, command.ItemServicoId, command.PecaId);
            return Result.Failure<Guid>("Não foi possível adicionar a peça ao item de serviço.");
        }
    }
}