using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.PerfilUsuario.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.PerfilUsuario;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Common;
using Ofichina.Application.Abstractions.Common;
using Ofichina.Application.Abstractions.Interfaces.Repository;

namespace Ofichina.Application.UseCases.PerfilUsuario.Handlers;

public sealed class VincularPerfilUsuarioCommandHandler : ICommandHandler<VincularPerfilUsuarioCommand, Result<VincularPerfilUsuarioResponse>>
{
    private readonly IRepository<Usuario> _usuarioRepository;
    private readonly IPerfilRepository _perfilRepository;
    private readonly IPerfilUsuarioRepository _usuarioPerfilRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<VincularPerfilUsuarioCommandHandler> _logger;

    public VincularPerfilUsuarioCommandHandler(
        IRepository<Usuario> usuarioRepository,
        IPerfilRepository perfilRepository,
        IPerfilUsuarioRepository usuarioPerfilRepository,
        IUnitOfWork unitOfWork,
        ILogger<VincularPerfilUsuarioCommandHandler> logger)
    {
        _usuarioRepository = usuarioRepository;
        _perfilRepository = perfilRepository;
        _usuarioPerfilRepository = usuarioPerfilRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<VincularPerfilUsuarioResponse>> HandleAsync(VincularPerfilUsuarioCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Iniciando o processo de vinculação do perfil ao usuário. UsuarioId: {UsuarioId}, PerfilId: {PerfilId}", command.UsuarioId, command.PerfilId);

            var usuario = await _usuarioRepository.GetByIdAsync(command.UsuarioId, cancellationToken);
            if (usuario is null)
            {
                _logger.LogWarning("Usuário não encontrado. UsuarioId: {UsuarioId}", command.UsuarioId);
                return Result.Failure<VincularPerfilUsuarioResponse>("Usuário não encontrado.");
            }

            var perfil = await _perfilRepository.GetByIdAsync(command.PerfilId, cancellationToken);
            if (perfil is null)
            {
                _logger.LogWarning("Perfil não encontrado. PerfilId: {PerfilId}", command.PerfilId);
                return Result.Failure<VincularPerfilUsuarioResponse>("Perfil não encontrado.");
            }

            if (!perfil.EstaAtivo)
            {
                _logger.LogWarning("Perfil inativo. PerfilId: {PerfilId}", command.PerfilId);
                return Result.Failure<VincularPerfilUsuarioResponse>("Perfil inativo.");
            }

            var vinculoExistente = await _usuarioPerfilRepository.GetByUsuarioIdPerfilIdAsync(
                command.UsuarioId, command.PerfilId, cancellationToken);

            if (vinculoExistente is not null)
            {
                _logger.LogWarning("O vínculo entre usuário e perfil já existe. UsuarioId: {UsuarioId}, PerfilId: {PerfilId}", command.UsuarioId, command.PerfilId);
                return Result.Failure<VincularPerfilUsuarioResponse>("O vínculo entre usuário e perfil já existe.");
            }

            var vinculo = new UsuarioPerfil(command.UsuarioId, command.PerfilId);

            await _usuarioPerfilRepository.AddAsync(vinculo, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Perfil vinculado com sucesso ao usuário. UsuarioId: {UsuarioId}, PerfilId: {PerfilId}", command.UsuarioId, command.PerfilId);

            return Result.Success(new VincularPerfilUsuarioResponse
            {
                UsuarioId = command.UsuarioId,
                PerfilId = command.PerfilId,
                Mensagem = "Perfil vinculado com sucesso."
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao vincular perfil ao usuário.");
            return Result.Failure<VincularPerfilUsuarioResponse>("Erro ao vincular perfil ao usuário.");
        }
    }
}
