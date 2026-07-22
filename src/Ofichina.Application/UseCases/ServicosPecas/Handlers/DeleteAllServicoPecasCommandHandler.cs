using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.Abstractions.Interfaces;
using Ofichina.Application.UseCases.ServicosPecas.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Common;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Exceptions;

namespace Ofichina.Application.UseCases.ServicosPecas.Handlers;

/// <summary>
/// Handler para desativar todas as peças de um serviço.
/// </summary>
public sealed class DeleteAllServicoPecasCommandHandler : ICommandHandler<DeleteAllServicoPecasCommand, Result>
{
    private readonly IServicoRepository _servicoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteAllServicoPecasCommandHandler> _logger;

    public DeleteAllServicoPecasCommandHandler(
        IServicoRepository servicoRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteAllServicoPecasCommandHandler> logger)
    {
        _servicoRepository = servicoRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(DeleteAllServicoPecasCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Iniciando desativação de todas as peças do serviço. ServicoId: {ServicoId}.", command.ServicoId);

            var servico = await _servicoRepository.GetByIdAsync(command.ServicoId, includePecas: true, cancellationToken, tracking: true);
            if (servico is null || servico.EstaExcluida())
                return Result.Failure("Serviço não encontrado.");

            servico.RemoverTodasAsPecas();

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Todas as peças do serviço foram desativadas com sucesso. ServicoId: {ServicoId}.", command.ServicoId);
            return Result.Success();
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domínio ao desativar peças do serviço. ServicoId: {ServicoId}.", command.ServicoId);
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao desativar peças do serviço. ServicoId: {ServicoId}.", command.ServicoId);
            return Result.Failure("Não foi possível desativar as peças do serviço.");
        }
    }
}