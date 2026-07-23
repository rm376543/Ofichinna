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
/// Handler para desativar uma peça de serviço.
/// </summary>
public sealed class DeleteServicoPecaCommandHandler : ICommandHandler<DeleteServicoPecaCommand, Result>
{
    private readonly IServicoRepository _servicoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteServicoPecaCommandHandler> _logger;

    public DeleteServicoPecaCommandHandler(
        IServicoRepository servicoRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteServicoPecaCommandHandler> logger)
    {
        _servicoRepository = servicoRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(DeleteServicoPecaCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Iniciando desativação de peça de serviço. ServicoId: {ServicoId}, ServicoPecaId: {ServicoPecaId}.", command.ServicoId, command.ServicoPecaId);

            var servico = await _servicoRepository.GetByIdAsync(command.ServicoId, includePecas: true, cancellationToken, tracking: true);
            if (servico is null || servico.EstaExcluida())
                return Result.Failure("Serviço não encontrado.");

            var peca = servico.ObterPeca(command.ServicoPecaId);
            if (peca is null || peca.EstaExcluida())
                return Result.Failure("Peça não encontrada.");

            servico.RemoverPeca(command.ServicoPecaId);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Peça de serviço desativada com sucesso. ServicoId: {ServicoId}, ServicoPecaId: {ServicoPecaId}.", command.ServicoId, command.ServicoPecaId);
            return Result.Success();
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domínio ao desativar peça de serviço. ServicoId: {ServicoId}, ServicoPecaId: {ServicoPecaId}.", command.ServicoId, command.ServicoPecaId);
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao desativar peça de serviço. ServicoId: {ServicoId}, ServicoPecaId: {ServicoPecaId}.", command.ServicoId, command.ServicoPecaId);
            return Result.Failure("Não foi possível desativar a peça do serviço.");
        }
    }
}