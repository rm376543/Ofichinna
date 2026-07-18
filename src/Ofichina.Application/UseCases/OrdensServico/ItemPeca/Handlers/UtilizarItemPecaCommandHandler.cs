using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.OrdensServico.ItemPeca.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Exceptions;
using Ofichina.Domain.Interfaces;

namespace Ofichina.Application.UseCases.OrdensServico.ItemPeca.Handlers;

/// <summary>
/// Handler para utilizaÃ§Ã£o de item de peÃ§a em ordem de serviÃ§o.
/// </summary>
public sealed class UtilizarItemPecaCommandHandler : ICommandHandler<UtilizarItemPecaCommand, Result>
{
    private readonly IOrdemServicoRepository _ordemServicoRepository;
    private readonly IRepository<Peca> _pecaRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UtilizarItemPecaCommandHandler> _logger;

    public UtilizarItemPecaCommandHandler(
        IOrdemServicoRepository ordemServicoRepository,
        IRepository<Peca> pecaRepository,
        IUnitOfWork unitOfWork,
        ILogger<UtilizarItemPecaCommandHandler> logger)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _pecaRepository = pecaRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(UtilizarItemPecaCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Iniciando utilizaÃ§Ã£o de item de peÃ§a. OrdemServicoId: {OrdemServicoId}, ItemPecaId: {ItemPecaId}.", command.OrdemServicoId, command.Id);

            var ordemServico = await _ordemServicoRepository.GetByIdAsync(command.OrdemServicoId, includeItens: true, cancellationToken);
            if (ordemServico is null || ordemServico.EstaExcluida())
                return Result.Failure("Ordem de serviÃ§o nÃ£o encontrada.");

            var item = ordemServico.Pecas.FirstOrDefault(x => x.Id == command.Id);
            if (item is null || item.EstaExcluida())
                return Result.Failure("PeÃ§a nÃ£o encontrada.");

            var peca = await _pecaRepository.GetByIdAsync(item.PecaId, cancellationToken);
            if (peca is null || peca.EstaExcluida())
                return Result.Failure("PeÃ§a de catÃ¡logo nÃ£o encontrada.");

            if (item.Quantidade > peca.QuantidadeEstoque)
                return Result.Failure("Quantidade insuficiente em estoque.");

            ordemServico.UtilizarPeca(command.Id);
            peca.SaidaEstoque(item.Quantidade);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Item de peÃ§a utilizado com sucesso. OrdemServicoId: {OrdemServicoId}, ItemPecaId: {ItemPecaId}.", command.OrdemServicoId, command.Id);
            return Result.Success();
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domÃ­nio ao utilizar item de peÃ§a. OrdemServicoId: {OrdemServicoId}, ItemPecaId: {ItemPecaId}.", command.OrdemServicoId, command.Id);
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao utilizar item de peÃ§a. OrdemServicoId: {OrdemServicoId}, ItemPecaId: {ItemPecaId}.", command.OrdemServicoId, command.Id);
            return Result.Failure("NÃ£o foi possÃ­vel utilizar a peÃ§a.");
        }
    }
}

