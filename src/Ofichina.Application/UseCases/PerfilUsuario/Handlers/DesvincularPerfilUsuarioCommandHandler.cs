using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.PerfilUsuario.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.PerfilUsuario;
using Ofichina.Domain.Interfaces;

namespace Ofichina.Application.UseCases.PerfilUsuario.Handlers;

public sealed class DesvincularPerfilUsuarioCommandHandler : ICommandHandler<DesvincularPerfilUsuarioCommand, Result<DesvincularPerfilUsuarioResponse>>
{
    private readonly IPerfilUsuarioRepository _clientePerfilRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DesvincularPerfilUsuarioCommandHandler(
        IPerfilUsuarioRepository clientePerfilRepository,
        IUnitOfWork unitOfWork)
    {
        _clientePerfilRepository = clientePerfilRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<DesvincularPerfilUsuarioResponse>> HandleAsync(DesvincularPerfilUsuarioCommand command)
    {
        var vinculo = await _clientePerfilRepository.GetByUsuarioIdPerfilIdAsync(
            command.UsuarioId,
            command.PerfilId);

        if (vinculo is null)
            return Result.Failure<DesvincularPerfilUsuarioResponse>("Vínculo entre usuário e perfil não encontrado.");

        await _clientePerfilRepository.DeleteAsync(vinculo);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success(new DesvincularPerfilUsuarioResponse
        {
            UsuarioId = command.UsuarioId,
            PerfilId = command.PerfilId,
            Mensagem = "Perfil desvinculado com sucesso."
        });
    }
}