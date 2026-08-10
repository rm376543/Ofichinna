using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Application.UseCases.PerfilPermissoes.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Common;

namespace Ofichina.Application.UseCases.PerfilPermissoes.Handlers;

public sealed class DesvincularPermissaoPerfilCommandHandler : ICommandHandler<DesvincularPermissaoPerfilCommand, Result>
{
    private readonly IPerfilPermissaoRepository _perfilPermissaoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DesvincularPermissaoPerfilCommandHandler> _logger;

    public DesvincularPermissaoPerfilCommandHandler(
        IPerfilPermissaoRepository perfilPermissaoRepository,
        IUnitOfWork unitOfWork,
        ILogger<DesvincularPermissaoPerfilCommandHandler> logger)
    {
        _perfilPermissaoRepository = perfilPermissaoRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(DesvincularPermissaoPerfilCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var vinculo = await _perfilPermissaoRepository.GetByPerfilIdPermissaoIdAsync(command.PerfilId, command.PermissaoId, cancellationToken);

            if (vinculo is null)
                return Result.Failure("Vínculo entre perfil e permissão não encontrado.");

            await _perfilPermissaoRepository.DeleteAsync(vinculo, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao desvincular permissão do perfil. PerfilId: {PerfilId}, PermissaoId: {PermissaoId}", command.PerfilId, command.PermissaoId);
            return Result.Failure("Não foi possível desvincular a permissão do perfil.");
        }
    }
}
