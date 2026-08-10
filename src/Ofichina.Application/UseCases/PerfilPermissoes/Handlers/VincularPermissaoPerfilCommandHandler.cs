using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.PerfilPermissoes.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Common;
using Ofichina.Application.Abstractions.Interfaces.Repository;

namespace Ofichina.Application.UseCases.PerfilPermissoes.Handlers;

public sealed class VincularPermissaoPerfilCommandHandler : ICommandHandler<VincularPermissaoPerfilCommand, Result>
{
    private readonly IPerfilRepository _perfilRepository;
    private readonly IPermissaoRepository _permissaoRepository;
    private readonly IPerfilPermissaoRepository _perfilPermissaoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<VincularPermissaoPerfilCommandHandler> _logger;

    public VincularPermissaoPerfilCommandHandler(
        IPerfilRepository perfilRepository,
        IPermissaoRepository permissaoRepository,
        IPerfilPermissaoRepository perfilPermissaoRepository,
        IUnitOfWork unitOfWork,
        ILogger<VincularPermissaoPerfilCommandHandler> logger)
    {
        _perfilRepository = perfilRepository;
        _permissaoRepository = permissaoRepository;
        _perfilPermissaoRepository = perfilPermissaoRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(VincularPermissaoPerfilCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var perfil = await _perfilRepository.GetByIdAsync(command.PerfilId, cancellationToken);

            if (perfil is null)
                return Result.Failure("Perfil não encontrado.");

            var permissao = await _permissaoRepository.GetByIdAsync(command.PermissaoId, cancellationToken);

            if (permissao is null)
                return Result.Failure("Permissão não encontrada.");

            var vinculoExistente = await _perfilPermissaoRepository.GetByPerfilIdPermissaoIdAsync(command.PerfilId, command.PermissaoId, cancellationToken);

            if (vinculoExistente is not null)
                return Result.Failure("O vínculo entre perfil e permissão já existe.");

            var vinculo = new PerfilPermissao(command.PerfilId, command.PermissaoId);

            await _perfilPermissaoRepository.AddAsync(vinculo, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao vincular permissão ao perfil. PerfilId: {PerfilId}, PermissaoId: {PermissaoId}", command.PerfilId, command.PermissaoId);
            return Result.Failure("Não foi possível vincular a permissão ao perfil.");
        }
    }
}
