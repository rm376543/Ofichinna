using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Pecas.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Exceptions;
using Ofichina.Domain.Interfaces;

namespace Ofichina.Application.UseCases.Pecas.Handlers;

/// <summary>
/// Handler para criaÃ§Ã£o de peÃ§a.
/// </summary>
public sealed class CreatePecaCommandHandler : ICommandHandler<CreatePecaCommand, Result<Guid>>
{
    private readonly IRepository<Peca> _pecaRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreatePecaCommandHandler> _logger;

    /// <summary>
    /// Inicializa uma nova instÃ¢ncia do handler de criaÃ§Ã£o de peÃ§a.
    /// </summary>
    public CreatePecaCommandHandler(
        IRepository<Peca> pecaRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreatePecaCommandHandler> logger)
    {
        _pecaRepository = pecaRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Result<Guid>> HandleAsync(CreatePecaCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var peca = new Peca(
                command.Nome,
                command.Descricao,
                command.Codigo,
                command.Valor,
                command.QuantidadeEstoque);

            if (!command.Ativo)
                peca.Desativar();

            await _pecaRepository.AddAsync(peca, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success(peca.Id);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domÃ­nio ao criar peÃ§a.");
            return Result.Failure<Guid>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar peÃ§a.");
            return Result.Failure<Guid>("NÃ£o foi possÃ­vel criar a peÃ§a.");
        }
    }
}
