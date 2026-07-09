using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Cliente.Commands;
using Ofichina.Contracts.Responses.Cliente;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Interfaces;
using Ofichina.Domain.ValueObjects;

namespace Ofichina.Application.UseCases.Cliente.Handlers;

public sealed class VincularPerfilClienteCommandHandler : ICommandHandler<VincularPerfilClienteCommand, Result<VincularPerfilClienteResponse>>
{
    private readonly IRepository<Usuario> _usuarioRepository;
    private readonly IPerfilRepository _perfilRepository;
    private readonly IClientePerfilRepository _usuarioPerfilRepository;
    private readonly IUnitOfWork _unitOfWork;

    public VincularPerfilClienteCommandHandler(
        IRepository<Usuario> usuarioRepository,
        IPerfilRepository perfilRepository,
        IClientePerfilRepository usuarioPerfilRepository,
        IUnitOfWork unitOfWork)
    {
        _usuarioRepository = usuarioRepository;
        _perfilRepository = perfilRepository;
        _usuarioPerfilRepository = usuarioPerfilRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<VincularPerfilClienteResponse>> HandleAsync(VincularPerfilClienteCommand command)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(command.UsuarioId);
        if (usuario is null)
            return Result.Failure<VincularPerfilClienteResponse>("Usuário não encontrado.");

        var perfil = await _perfilRepository.GetByIdAsync(command.PerfilId);
        if (perfil is null)
            return Result.Failure<VincularPerfilClienteResponse>("Perfil não encontrado.");

        if (!perfil.Ativo)
            return Result.Failure<VincularPerfilClienteResponse>("Perfil inativo.");

        var vinculoExistente = await _usuarioPerfilRepository.GetByUsuarioIdPerfilIdAsync(
            command.UsuarioId,
            command.PerfilId);

        if (vinculoExistente is not null)
            return Result.Failure<VincularPerfilClienteResponse>("O vínculo entre usuário e perfil já existe.");

        var vinculo = new UsuarioPerfil(command.UsuarioId, command.PerfilId);

        await _usuarioPerfilRepository.AddAsync(vinculo);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success(new VincularPerfilClienteResponse
        {
            UsuarioId = command.UsuarioId,
            PerfilId = command.PerfilId,
            Mensagem = "Perfil vinculado com sucesso."
        });
    }
}