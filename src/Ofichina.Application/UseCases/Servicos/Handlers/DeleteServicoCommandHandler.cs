using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Servicos.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Interfaces;

namespace Ofichina.Application.UseCases.Servicos.Handlers;

/// <summary>
/// Handler para exclusÃ£o lÃ³gica de serviÃ§o.
/// </summary>
public sealed class DeleteServicoCommandHandler : ICommandHandler<DeleteServicoCommand, Result>
{
    private readonly IRepository<Servico> _servicoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteServicoCommandHandler> _logger;

    public DeleteServicoCommandHandler(
        IRepository<Servico> servicoRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteServicoCommandHandler> logger)
    {
        _servicoRepository = servicoRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(DeleteServicoCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var servico = await _servicoRepository.GetByIdAsync(command.Id, cancellationToken);

            if (servico is null || servico.EstaExcluida())
                return Result.Failure("ServiÃ§o nÃ£o encontrado.");

            servico.Desativar();
            servico.Excluir();

            await _servicoRepository.UpdateAsync(servico, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao remover serviÃ§o. ServicoId: {ServicoId}", command.Id);
            return Result.Failure("NÃ£o foi possÃ­vel remover o serviÃ§o.");
        }
    }
}
