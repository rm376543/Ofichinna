using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Responses.Cliente;
using Ofichina.Domain.ValueObjects;

namespace Ofichina.Application.UseCases.Cliente.Commands;

public sealed class DesvincularPerfilClienteCommand : ICommand<Result<DesvincularPerfilClienteResponse>>
{
    public Guid UsuarioId { get; }

    public Guid PerfilId { get; }

    public DesvincularPerfilClienteCommand(Guid usuarioId, Guid perfilId)
    {
        UsuarioId = usuarioId;
        PerfilId = perfilId;
    }
}