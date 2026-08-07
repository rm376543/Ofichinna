using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.PerfilUsuario;
using Ofichina.Contracts.Responses.PerfilUsuario;

namespace Ofichina.Application.UseCases.PerfilUsuario.Commands;

public sealed class VincularPerfilUsuarioCommand : ICommand<Result<VincularPerfilUsuarioResponse>>
{
    public Guid UsuarioId { get; }

    public Guid PerfilId { get; }

    public VincularPerfilUsuarioCommand(VincularPerfilUsuarioRequest request)
    {
        UsuarioId = request.UsuarioId;
        PerfilId = request.PerfilId;
    }
}