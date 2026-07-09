using Ofichina.Contracts.Requests;

namespace Ofichina.Contracts.Requests.Usuario;

public sealed class VincularPerfilClienteRequest : CreateRequest
{
    public Guid UsuarioId { get; set; }

    public Guid PerfilId { get; set; }
}