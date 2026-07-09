using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Responses.Cliente;
using Ofichina.Domain.ValueObjects;

namespace Ofichina.Application.UseCases.Cliente.Commands;

public sealed class VincularPerfilClienteCommand : ICommand<Result<VincularPerfilClienteResponse>>
{
    public Guid UsuarioId { get; }

    public Guid PerfilId { get; }

    public VincularPerfilClienteCommand(Guid usuarioId, Guid perfilId)
    {
        UsuarioId = usuarioId;
        PerfilId = perfilId;
    }
}