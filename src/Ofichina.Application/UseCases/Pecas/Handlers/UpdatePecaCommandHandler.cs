using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Pecas.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Exceptions;
using Ofichina.Domain.Interfaces;

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

    /// <inheritdoc />
    public async Task<Result> HandleAsync(UpdatePecaCommand command)
    {
        try
        {
            var peca = await _pecaRepository.GetByIdAsync(command.Id);

            if (peca is null || peca.EstaExcluida())
                return Result.Failure("Peça não encontrada.");

            peca.AtualizarDados(
                command.Nome,
                command.Descricao,
                command.Codigo,
                command.Valor,
                command.QuantidadeEstoque);

            if (command.Ativo)
                peca.Ativar();
            else
                peca.Desativar();

            await _pecaRepository.UpdateAsync(peca);
            await _unitOfWork.SaveChangesAsync();

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