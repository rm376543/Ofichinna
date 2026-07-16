using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.OrdensServico.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Exceptions;
using Ofichina.Domain.Interfaces;

namespace Ofichina.Application.UseCases.OrdensServico.Handlers;

/// <summary>
/// Handler para alteração de status da ordem de serviço.
/// </summary>
public sealed class AlterarStatusOrdemServicoCommandHandler : ICommandHandler<AlterarStatusOrdemServicoCommand, Result>
{
    private readonly IRepository<OrdemServico> _ordemServicoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AlterarStatusOrdemServicoCommandHandler> _logger;

    public AlterarStatusOrdemServicoCommandHandler(
        IRepository<OrdemServico> ordemServicoRepository,
        IUnitOfWork unitOfWork,
        ILogger<AlterarStatusOrdemServicoCommandHandler> logger)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(AlterarStatusOrdemServicoCommand command)
    {
        try
        {
            _logger.LogInformation("Iniciando alteração de status da ordem de serviço. OrdemServicoId: {OrdemServicoId}, StatusDestino: {StatusDestino}.", command.Id, command.StatusDestino);

            var ordemServico = await _ordemServicoRepository.GetByIdAsync(command.Id);
            if (ordemServico is null || ordemServico.EstaExcluida())
                return Result.Failure("Ordem de serviço não encontrada.");

            AlterarStatus(ordemServico, command.StatusDestino);

            await _ordemServicoRepository.UpdateAsync(ordemServico);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Status da ordem de serviço alterado com sucesso. OrdemServicoId: {OrdemServicoId}, StatusDestino: {StatusDestino}.", command.Id, command.StatusDestino);
            return Result.Success();
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domínio ao alterar status da ordem de serviço. OrdemServicoId: {OrdemServicoId}, StatusDestino: {StatusDestino}.", command.Id, command.StatusDestino);
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao alterar status da ordem de serviço. OrdemServicoId: {OrdemServicoId}, StatusDestino: {StatusDestino}.", command.Id, command.StatusDestino);
            return Result.Failure("Não foi possível alterar o status da ordem de serviço.");
        }
    }

    private static void AlterarStatus(OrdemServico ordemServico, StatusOrdemServico statusDestino)
    {
        switch (statusDestino)
        {
            case StatusOrdemServico.EmDiagnostico:
                ordemServico.IniciarDiagnostico();
                break;
            case StatusOrdemServico.AguardandoAprovacao:
                ordemServico.SolicitarAprovacao();
                break;
            case StatusOrdemServico.EmExecucao:
                ordemServico.Aprovar();
                break;
            case StatusOrdemServico.Finalizada:
                ordemServico.Finalizar();
                break;
            case StatusOrdemServico.Entregue:
                ordemServico.Entregar();
                break;
            case StatusOrdemServico.Cancelada:
                ordemServico.Cancelar();
                break;
            default:
                throw new DomainException("Status de destino inválido.");
        }
    }
}
