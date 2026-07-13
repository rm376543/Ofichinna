using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.PerfilUsuario.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.PerfilUsuario;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Interfaces;

namespace Ofichina.Application.UseCases.PerfilUsuario.Handlers;

public sealed class VincularPerfilUsuarioCommandHandler : ICommandHandler<VincularPerfilUsuarioCommand, Result<VincularPerfilUsuarioResponse>>
{
    private readonly IRepository<Usuario> _usuarioRepository;
    private readonly IPerfilRepository _perfilRepository;
    private readonly IPerfilUsuarioRepository _usuarioPerfilRepository;
    private readonly IUnitOfWork _unitOfWork;

    public VincularPerfilUsuarioCommandHandler(
        IRepository<Usuario> usuarioRepository,
        IPerfilRepository perfilRepository,
        IPerfilUsuarioRepository usuarioPerfilRepository,
        IUnitOfWork unitOfWork)
    {
        _usuarioRepository = usuarioRepository;
        _perfilRepository = perfilRepository;
        _usuarioPerfilRepository = usuarioPerfilRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<VincularPerfilUsuarioResponse>> HandleAsync(VincularPerfilUsuarioCommand command)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(command.UsuarioId);
        if (usuario is null)
            return Result.Failure<VincularPerfilUsuarioResponse>("Usuário não encontrado.");

        var perfil = await _perfilRepository.GetByIdAsync(command.PerfilId);
        if (perfil is null)
            return Result.Failure<VincularPerfilUsuarioResponse>("Perfil não encontrado.");

        if (!perfil.PerfilEstaAtivo())
            return Result.Failure<VincularPerfilUsuarioResponse>("Perfil inativo.");

        var vinculoExistente = await _usuarioPerfilRepository.GetByUsuarioIdPerfilIdAsync(
            command.UsuarioId,
            command.PerfilId);

        if (vinculoExistente is not null)
            return Result.Failure<VincularPerfilUsuarioResponse>("O vínculo entre usuário e perfil já existe.");

        var vinculo = new UsuarioPerfil(command.UsuarioId, command.PerfilId);

        await _usuarioPerfilRepository.AddAsync(vinculo);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success(new VincularPerfilUsuarioResponse
        {
            UsuarioId = command.UsuarioId,
            PerfilId = command.PerfilId,
            Mensagem = "Perfil vinculado com sucesso."
        });
    }
}