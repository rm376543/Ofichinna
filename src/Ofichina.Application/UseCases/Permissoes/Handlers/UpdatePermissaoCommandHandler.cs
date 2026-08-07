using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Permissoes.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Exceptions;

namespace Ofichina.Application.UseCases.Permissoes.Handlers;

public sealed class UpdatePermissaoCommandHandler : ICommandHandler<UpdatePermissaoCommand, Result>
{
    private readonly IPermissaoRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdatePermissaoCommandHandler> _logger;

    public UpdatePermissaoCommandHandler(
        IPermissaoRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<UpdatePermissaoCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(UpdatePermissaoCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var permissao = await _repository.GetByIdAsync(command.PermissaoId, cancellationToken);

            if (permissao is null)
                return Result.Failure("Permissão não encontrada.");

            var codigoExistente = await _repository.GetByCodigoAsync(command.Codigo, cancellationToken);

            if (codigoExistente is not null && codigoExistente.Id != command.PermissaoId)
                return Result.Failure("Já existe uma permissão com este código.");

            permissao.Atualizar(command.Codigo, command.Descricao);

            await _repository.UpdateAsync(permissao, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domínio ao atualizar permissão. PermissaoId: {PermissaoId}", command.PermissaoId);
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar permissão. PermissaoId: {PermissaoId}", command.PermissaoId);
            return Result.Failure("Não foi possível atualizar a permissão.");
        }
    }
}
