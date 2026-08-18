using Ofichina.Application.Abstractions;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Application.UseCases.OrdensServico.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Exceptions;

namespace Ofichina.Application.UseCases.OrdensServico.Handlers;

/// <summary>
/// Handler para utilização de peça vinculada a um serviço da ordem de serviço.
/// </summary>
public sealed class UtilizarPecaCommandHandler : ICommandHandler<UtilizarPecaCommand, Result>
{
    private readonly IOrdemServicoRepository _ordemServicoRepository;
    private readonly IItemServicoRepository _itemServicoRepository;
    private readonly IRepository<Peca> _pecaRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UtilizarPecaCommandHandler> _logger;

    public UtilizarPecaCommandHandler(
        IOrdemServicoRepository ordemServicoRepository,
        IItemServicoRepository itemServicoRepository,
        IRepository<Peca> pecaRepository,
        IUnitOfWork unitOfWork,
        ILogger<UtilizarPecaCommandHandler> logger)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _itemServicoRepository = itemServicoRepository;
        _pecaRepository = pecaRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(UtilizarPecaCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Iniciando utilização de peça na ordem de serviço. OrdemServicoId: {OrdemServicoId}, PecaId: {PecaId}.", command.OrdemServicoId, command.PecaId);

            var ordemServico = await _ordemServicoRepository.GetByIdAsync(command.OrdemServicoId, true, true, cancellationToken);
            if (ordemServico is null || ordemServico.EstaExcluida())
                return Result.Failure("Ordem de serviço não encontrada.");

            var itemServico = await _itemServicoRepository.GetByOrdemServicoIdAndItemServicoIdAsync(
                command.OrdemServicoId,
                command.ItemServicoId,
                cancellationToken,
                tracking: true,
                includeRelacionados: true);

            if (itemServico is null || itemServico.EstaExcluida())
                return Result.Failure("Item de serviço não encontrado.");

            if (!itemServico.PecaId.HasValue)
                return Result.Failure("O item de serviço não possui peça vinculada.");

            var peca = await _pecaRepository.GetByIdAsync(itemServico.PecaId.Value, cancellationToken, tracking: true);
            if (peca is null || peca.EstaExcluida())
                return Result.Failure("Peça de catálogo não encontrada.");

            if (itemServico.Quantidade > peca.QuantidadeEstoque)
                return Result.Failure("Quantidade insuficiente em estoque.");

            peca.SaidaEstoque(itemServico.Quantidade);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Peça utilizada com sucesso. OrdemServicoId: {OrdemServicoId}, PecaId: {PecaId}.", command.OrdemServicoId, command.PecaId);
            return Result.Success();
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domínio ao utilizar peça. OrdemServicoId: {OrdemServicoId}, PecaId: {PecaId}.", command.OrdemServicoId, command.PecaId);
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao utilizar peça. OrdemServicoId: {OrdemServicoId}, PecaId: {PecaId}.", command.OrdemServicoId, command.PecaId);
            return Result.Failure("Não foi possível utilizar a peça.");
        }
    }
}
