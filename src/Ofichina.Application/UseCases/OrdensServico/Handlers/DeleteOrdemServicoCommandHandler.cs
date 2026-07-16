using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.OrdensServico.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Interfaces;

namespace Ofichina.Application.UseCases.OrdensServico.Handlers;

/// <summary>
/// Handler para remoção lógica de ordem de serviço.
/// </summary>
public sealed class DeleteOrdemServicoCommandHandler : ICommandHandler<DeleteOrdemServicoCommand, Result>
{
    private readonly IRepository<OrdemServico> _ordemServicoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteOrdemServicoCommandHandler> _logger;

    public DeleteOrdemServicoCommandHandler(
        IRepository<OrdemServico> ordemServicoRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteOrdemServicoCommandHandler> logger)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(DeleteOrdemServicoCommand command)
    {
        try
        {
            _logger.LogInformation("Iniciando remoção da ordem de serviço. OrdemServicoId: {OrdemServicoId}.", command.Id);

            var ordemServico = await _ordemServicoRepository.GetByIdAsync(command.Id);
            if (ordemServico is null || ordemServico.EstaExcluida())
                return Result.Failure("Ordem de serviço não encontrada.");

            ordemServico.Excluir();

            await _ordemServicoRepository.UpdateAsync(ordemServico);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Ordem de serviço removida com sucesso. OrdemServicoId: {OrdemServicoId}", command.Id);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao remover ordem de serviço. OrdemServicoId: {OrdemServicoId}", command.Id);
            return Result.Failure("Não foi possível remover a ordem de serviço.");
        }
    }
}
