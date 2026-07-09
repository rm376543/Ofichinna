namespace Ofichina.Contracts.Requests.PerfilCliente;

public sealed class VincularPerfilClienteRequest : CreateRequest
{
    public Guid UsuarioId { get; set; }

    public Guid PerfilId { get; set; }
}