using Ofichina.Application.Abstractions;
using Ofichina.Application.Abstractions.Authentication.Service;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Application.UseCases.OrdensServico.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Common;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Exceptions;
using Ofichina.Domain.Enums;

namespace Ofichina.Application.UseCases.OrdensServico.Handlers;

/// <summary>
/// Handler para alteração de status da ordem de serviço.
/// </summary>
public sealed class AlterarStatusOrdemServicoCommandHandler : ICommandHandler<AlterarStatusOrdemServicoCommand, Result>
{
    private readonly IOrdemServicoRepository _ordemServicoRepository;
    private readonly IRepository<HistoricoStatus> _historicoStatusRepository;
    private readonly IUserService _usuarioAtualService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AlterarStatusOrdemServicoCommandHandler> _logger;

    public AlterarStatusOrdemServicoCommandHandler(
        IOrdemServicoRepository ordemServicoRepository,
        IRepository<HistoricoStatus> historicoStatusRepository,
        IUserService usuarioAtualService,
        IUnitOfWork unitOfWork,
        ILogger<AlterarStatusOrdemServicoCommandHandler> logger)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _historicoStatusRepository = historicoStatusRepository;
        _usuarioAtualService = usuarioAtualService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(AlterarStatusOrdemServicoCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Iniciando alteração de status da ordem de serviço. OrdemServicoId: {OrdemServicoId}, StatusDestino: {StatusDestino}.", command.Id, command.StatusDestino);

            var statusDestino = MapearStatus(command.StatusDestino);
            var incluirItens = statusDestino == StatusOrdemServico.Finalizada;
            var ordemServico = await _ordemServicoRepository.GetByIdAsync(command.Id, incluirItens, true, cancellationToken);
            if (ordemServico is null || ordemServico.EstaExcluida())
                return Result.Failure("Ordem de serviço não encontrada.");

            var statusAnterior = ordemServico.Status;
            AlterarStatus(ordemServico, statusDestino);

            await _ordemServicoRepository.UpdateAsync(ordemServico, cancellationToken);
            await _historicoStatusRepository.AddAsync(
                HistoricoStatus.ParaOrdemServico(
                    ordemServico.Id,
                    statusAnterior.ToUpperSnakeCase(),
                    ordemServico.Status.ToUpperSnakeCase(),
                    _usuarioAtualService.ObterUsuarioId()),
                cancellationToken);
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
            case StatusOrdemServico.EmExecucao:
                ordemServico.IniciarExecucao();
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

    private static StatusOrdemServico MapearStatus(string statusDestino)
    {
        return Enum.TryParse<StatusOrdemServico>(statusDestino, true, out var status)
            ? status
            : throw new DomainException("Status de destino inválido.");
    }
}

