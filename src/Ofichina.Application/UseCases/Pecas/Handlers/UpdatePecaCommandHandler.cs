using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Pecas.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Exceptions;

namespace Ofichina.Application.UseCases.Pecas.Handlers;

/// <summary>
/// Handler para atualização de peça.
/// </summary>
public sealed class UpdatePecaCommandHandler : ICommandHandler<UpdatePecaCommand, Result>
{
    private readonly IRepository<Peca> _pecaRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdatePecaCommandHandler> _logger;

    /// <summary>
    /// Inicializa uma nova instância do handler de atualização de peça.
    /// </summary>
    public UpdatePecaCommandHandler(
        IRepository<Peca> pecaRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdatePecaCommandHandler> logger)
    {
        _pecaRepository = pecaRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Manipula o comando de atualização de peça.
    /// </summary>
    /// <param name="command">Comando de atualização de peça.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Resultado da operação.</returns>
    public async Task<Result> HandleAsync(UpdatePecaCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Iniciando atualização da peça com ID: {Id}", command.PecaId);
            var peca = await _pecaRepository.GetByIdAsync(command.PecaId, cancellationToken);

            if (peca is null || peca.EstaExcluida())
            {
                _logger.LogWarning("Peça com ID: {Id} não encontrada ou está excluída.", command.PecaId);
                return Result.Failure("Peça não encontrada.");
            }

            peca.AtualizarDados(
                command.Nome,
                command.Descricao,
                command.Codigo,
                command.Valor,
                command.QuantidadeEstoque);

            await _pecaRepository.UpdateAsync(peca, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Peça com ID: {Id} atualizada com sucesso.", command.PecaId);
            return Result.Success();
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domínio ao atualizar peça.");
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar peça.");
            return Result.Failure("Não foi possível atualizar a peça.");
        }
    }
}


