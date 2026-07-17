using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Pecas.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Interfaces;

namespace Ofichina.Application.UseCases.Pecas.Handlers;

/// <summary>
/// Handler para exclusão lógica de peça.
/// </summary>
public sealed class DeletePecaCommandHandler : ICommandHandler<DeletePecaCommand, Result>
{
    private readonly IRepository<Peca> _pecaRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeletePecaCommandHandler> _logger;

    /// <summary>
    /// Inicializa uma nova instância do handler de exclusão de peça.
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
    public async Task<Result> HandleAsync(DeletePecaCommand command)
    {
        try
        {
            var peca = await _pecaRepository.GetByIdAsync(command.Id);

            if (peca is null || peca.EstaExcluida())
                return Result.Failure("Peça não encontrada.");

            peca.Desativar();
            peca.ExcluirLogicamente();

            await _pecaRepository.UpdateAsync(peca);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao remover peça. PecaId: {PecaId}", command.Id);
            return Result.Failure("Não foi possível remover a peça.");
        }
    }
}