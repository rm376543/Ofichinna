using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.OrdensServico.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Exceptions;
using Ofichina.Domain.Interfaces;

namespace Ofichina.Application.UseCases.OrdensServico.Handlers;

/// <summary>
/// Handler para atualização de ordem de serviço.
/// </summary>
public sealed class UpdateOrdemServicoCommandHandler : ICommandHandler<UpdateOrdemServicoCommand, Result>
{
    private readonly IRepository<OrdemServico> _ordemServicoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateOrdemServicoCommandHandler> _logger;

    public UpdateOrdemServicoCommandHandler(
        IRepository<OrdemServico> ordemServicoRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateOrdemServicoCommandHandler> logger)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(UpdateOrdemServicoCommand command)
    {
        try
        {
            _logger.LogInformation("Iniciando atualização da ordem de serviço. OrdemServicoId: {OrdemServicoId}.", command.Id);

            var ordemServico = await _ordemServicoRepository.GetByIdAsync(command.Id);
            if (ordemServico is null || ordemServico.EstaExcluida())
                return Result.Failure("Ordem de serviço não encontrada.");

            ordemServico.AtualizarAtendimento(command.FuncionarioId, command.Observacoes);

            await _ordemServicoRepository.UpdateAsync(ordemServico);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Ordem de serviço atualizada com sucesso. OrdemServicoId: {OrdemServicoId}", command.Id);
            return Result.Success();
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domínio ao atualizar ordem de serviço. OrdemServicoId: {OrdemServicoId}", command.Id);
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao atualizar ordem de serviço. OrdemServicoId: {OrdemServicoId}", command.Id);
            return Result.Failure("Não foi possível atualizar a ordem de serviço.");
        }
    }
}
