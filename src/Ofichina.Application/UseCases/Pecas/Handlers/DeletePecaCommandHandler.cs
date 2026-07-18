using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Pecas.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Interfaces;

namespace Ofichina.Application.UseCases.Pecas.Handlers;

/// <summary>
/// Handler para exclusÃ£o lÃ³gica de peÃ§a.
/// </summary>
public sealed class DeletePecaCommandHandler : ICommandHandler<DeletePecaCommand, Result>
{
    private readonly IRepository<Peca> _pecaRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeletePecaCommandHandler> _logger;

    /// <summary>
    /// Inicializa uma nova instÃ¢ncia do handler de exclusÃ£o de peÃ§a.
    /// </summary>
    public DeletePecaCommandHandler(
        IRepository<Peca> pecaRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeletePecaCommandHandler> logger)
    {
        _pecaRepository = pecaRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Result> HandleAsync(DeletePecaCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var peca = await _pecaRepository.GetByIdAsync(command.Id, cancellationToken);

            if (peca is null || peca.EstaExcluida())
                return Result.Failure("PeÃ§a nÃ£o encontrada.");

            peca.Desativar();

            await _pecaRepository.UpdateAsync(peca, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao remover peÃ§a. PecaId: {PecaId}", command.Id);
            return Result.Failure("NÃ£o foi possÃ­vel remover a peÃ§a.");
        }
    }
}
