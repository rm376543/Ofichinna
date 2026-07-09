using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Cliente.Commands;
using Ofichina.Contracts.Responses.Cliente;
using Ofichina.Domain.Interfaces;
using Ofichina.Domain.ValueObjects;

namespace Ofichina.Application.UseCases.Cliente.Handlers;

public sealed class DesvincularPerfilClienteCommandHandler : ICommandHandler<DesvincularPerfilClienteCommand, Result<DesvincularPerfilClienteResponse>>
{
    private readonly IClientePerfilRepository _clientePerfilRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DesvincularPerfilClienteCommandHandler(
        IClientePerfilRepository clientePerfilRepository,
        IUnitOfWork unitOfWork)
    {
        _clientePerfilRepository = clientePerfilRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<DesvincularPerfilClienteResponse>> HandleAsync(DesvincularPerfilClienteCommand command)
    {
        var vinculo = await _clientePerfilRepository.GetByUsuarioIdPerfilIdAsync(
            command.UsuarioId,
            command.PerfilId);

        if (vinculo is null)
            return Result.Failure<DesvincularPerfilClienteResponse>("Vínculo entre usuário e perfil não encontrado.");

        await _clientePerfilRepository.DeleteAsync(vinculo);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success(new DesvincularPerfilClienteResponse
        {
            UsuarioId = command.UsuarioId,
            PerfilId = command.PerfilId,
            Mensagem = "Perfil desvinculado com sucesso."
        });
    }
}