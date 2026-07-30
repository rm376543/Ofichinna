using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Common;
using Ofichina.Domain.Exceptions;
using Ofichina.Application.Abstractions.Interfaces;
using Ofichina.Application.UseCases.OrdensServico.Commands;
using Ofichina.Domain.Entities;
using Ofichina.Application.Abstractions.Common;

namespace Ofichina.Application.UseCases.OrdensServico.Handlers;

/// <summary>
/// Handler para utilização de peça vinculada a um serviço da ordem de serviço.
/// </summary>
public sealed class UtilizarPecaCommandHandler : ICommandHandler<UtilizarPecaCommand, Result>
{
    private readonly IOrdemServicoRepository _ordemServicoRepository;
    private readonly IRepository<Peca> _pecaRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UtilizarPecaCommandHandler> _logger;

    public UtilizarPecaCommandHandler(
        IOrdemServicoRepository ordemServicoRepository,
        IRepository<Peca> pecaRepository,
        IUnitOfWork unitOfWork,
        ILogger<UtilizarPecaCommandHandler> logger)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _pecaRepository = pecaRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(UtilizarPecaCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Iniciando utilização de peça na ordem de serviço. OrdemServicoId: {OrdemServicoId}, PecaId: {PecaId}.", command.OrdemServicoId, command.Id);

            var ordemServico = await _ordemServicoRepository.GetByIdAsync(command.OrdemServicoId, includeItens: true, cancellationToken, tracking: true);
            if (ordemServico is null || ordemServico.EstaExcluida())
                return Result.Failure("Ordem de serviço não encontrada.");

            var servico = ordemServico.ObterServico(command.ItemServicoId);
            if (servico is null || servico.EstaExcluida())
                return Result.Failure("Serviço não encontrado.");

            var servicoPeca = servico.ObterPeca(command.Id);
            if (servicoPeca is null || servicoPeca.EstaExcluida())
                return Result.Failure("Peça não encontrada.");

            var peca = await _pecaRepository.GetByIdAsync(servicoPeca.PecaId, cancellationToken, tracking: true);
            if (peca is null || peca.EstaExcluida())
                return Result.Failure("Peça de catálogo não encontrada.");

            if (servicoPeca.Quantidade > peca.QuantidadeEstoque)
                return Result.Failure("Quantidade insuficiente em estoque.");

            ordemServico.UtilizarPeca(command.ItemServicoId, command.Id);
            peca.SaidaEstoque(servicoPeca.Quantidade);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Peça utilizada com sucesso. OrdemServicoId: {OrdemServicoId}, PecaId: {PecaId}.", command.OrdemServicoId, command.Id);
            return Result.Success();
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domínio ao utilizar peça. OrdemServicoId: {OrdemServicoId}, PecaId: {PecaId}.", command.OrdemServicoId, command.Id);
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao utilizar peça. OrdemServicoId: {OrdemServicoId}, PecaId: {PecaId}.", command.OrdemServicoId, command.Id);
            return Result.Failure("Não foi possível utilizar a peça.");
        }
    }
}
