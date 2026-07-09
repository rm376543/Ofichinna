namespace Ofichina.Contracts.Responses.PerfilCliente;

public sealed class VincularPerfilClienteResponse
{
    public Guid UsuarioId { get; set; }

    public Guid PerfilId { get; set; }

    public string Mensagem { get; set; } = "Perfil vinculado com sucesso.";
}