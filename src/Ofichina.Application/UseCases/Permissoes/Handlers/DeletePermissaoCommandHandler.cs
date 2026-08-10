using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Application.UseCases.Permissoes.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Common;

namespace Ofichina.Application.UseCases.Permissoes.Handlers;

public sealed class DeletePermissaoCommandHandler : ICommandHandler<DeletePermissaoCommand, Result>
{
    private readonly IPermissaoRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeletePermissaoCommandHandler> _logger;

    public DeletePermissaoCommandHandler(
        IPermissaoRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<DeletePermissaoCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(DeletePermissaoCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var permissao = await _repository.GetByIdAsync(command.Id, cancellationToken);

            if (permissao is null)
                return Result.Failure("Permissão não encontrada.");

            await _repository.DeleteAsync(permissao, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao desativar permissão. PermissaoId: {PermissaoId}", command.Id);
            return Result.Failure("Não foi possível remover a permissão.");
        }
    }
}
