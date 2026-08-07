using Ofichina.Contracts.Common;

namespace Ofichina.Contracts.Requests.PerfilUsuario;

public sealed class VincularPerfilUsuarioRequest : CreateRequest
{
    public Guid UsuarioId { get; set; }

    public Guid PerfilId { get; set; }
}