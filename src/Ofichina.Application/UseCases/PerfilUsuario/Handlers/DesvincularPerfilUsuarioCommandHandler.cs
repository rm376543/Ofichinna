using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.PerfilUsuario.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.PerfilUsuario;
using Ofichina.Domain.Common;
using Ofichina.Application.Abstractions.Interfaces.Repository;

namespace Ofichina.Application.UseCases.PerfilUsuario.Handlers;

public sealed class DesvincularPerfilUsuarioCommandHandler : ICommandHandler<DesvincularPerfilUsuarioCommand, Result<DesvincularPerfilUsuarioResponse>>
{
    private readonly IPerfilUsuarioRepository _clientePerfilRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DesvincularPerfilUsuarioCommandHandler> _logger;

    public DesvincularPerfilUsuarioCommandHandler(
        IPerfilUsuarioRepository clientePerfilRepository,
        IUnitOfWork unitOfWork,
        ILogger<DesvincularPerfilUsuarioCommandHandler> logger)
    {
        _clientePerfilRepository = clientePerfilRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<DesvincularPerfilUsuarioResponse>> HandleAsync(DesvincularPerfilUsuarioCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Iniciando desvinculação de perfil do usuário: [UsuarioId] {UsuarioId}, [PerfilId] {PerfilId}", command.UsuarioId, command.PerfilId);
            var vinculo = await _clientePerfilRepository.GetByUsuarioIdPerfilIdAsync(
                command.UsuarioId, command.PerfilId, cancellationToken);

            if (vinculo is null)
            {
                _logger.LogWarning("Vínculo entre usuário e perfil não encontrado: [UsuarioId] {UsuarioId}, [PerfilId] {PerfilId}", command.UsuarioId, command.PerfilId);
                return Result.Failure<DesvincularPerfilUsuarioResponse>("Vínculo entre usuário e perfil não encontrado.");
            }

            await _clientePerfilRepository.DeleteAsync(vinculo, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Perfil desvinculado com sucesso do usuário: [UsuarioId] {UsuarioId}, [PerfilId] {PerfilId}", command.UsuarioId, command.PerfilId);
            return Result.Success(new DesvincularPerfilUsuarioResponse
            {
                UsuarioId = command.UsuarioId,
                PerfilId = command.PerfilId,
                Mensagem = "Perfil desvinculado com sucesso."
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao desvincular perfil do usuário.");
            return Result.Failure<DesvincularPerfilUsuarioResponse>("Erro ao desvincular perfil do usuário.");
        }
    }
}
