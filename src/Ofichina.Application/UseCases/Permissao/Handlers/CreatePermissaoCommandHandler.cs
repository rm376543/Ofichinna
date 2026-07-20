using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.Abstractions.Interfaces;
using Ofichina.Application.UseCases.Permissoes.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Exceptions;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Common;

namespace Ofichina.Application.UseCases.Permissoes.Handlers;

public sealed class CreatePermissaoCommandHandler : ICommandHandler<CreatePermissaoCommand, Result<Guid>>
{
    private readonly IPermissaoRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreatePermissaoCommandHandler> _logger;

    public CreatePermissaoCommandHandler(
        IPermissaoRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<CreatePermissaoCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> HandleAsync(CreatePermissaoCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var codigoExistente = await _repository.GetByCodigoAsync(command.Codigo, cancellationToken);

            if (codigoExistente is not null)
                return Result.Failure<Guid>("Já existe uma permissão com este código.");

            var permissao = new Permissao(command.Codigo, command.Descricao);

            await _repository.AddAsync(permissao, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success(permissao.Id);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domínio ao criar permissão.");
            return Result.Failure<Guid>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar permissão.");
            return Result.Failure<Guid>("Não foi possível criar a permissão.");
        }
    }
}
