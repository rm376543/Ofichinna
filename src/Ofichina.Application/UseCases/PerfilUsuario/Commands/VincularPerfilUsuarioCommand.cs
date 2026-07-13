using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.PerfilUsuario;

namespace Ofichina.Application.UseCases.PerfilUsuario.Commands;

public sealed class VincularPerfilUsuarioCommand : ICommand<Result<VincularPerfilUsuarioResponse>>
{
    public Guid UsuarioId { get; }

    public Guid PerfilId { get; }

    public VincularPerfilUsuarioCommand(Guid usuarioId, Guid perfilId)
    {
        UsuarioId = usuarioId;
        PerfilId = perfilId;
    }
}