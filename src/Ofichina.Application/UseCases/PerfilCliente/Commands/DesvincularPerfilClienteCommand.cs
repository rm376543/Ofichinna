using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.PerfilCliente;

namespace Ofichina.Application.UseCases.PerfilCliente.Commands;

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