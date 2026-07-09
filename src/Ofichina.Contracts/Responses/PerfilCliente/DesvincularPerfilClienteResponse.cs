namespace Ofichina.Contracts.Responses.PerfilCliente;

public sealed class DesvincularPerfilClienteResponse
{
    public Guid UsuarioId { get; set; }

    public Guid PerfilId { get; set; }

    public string Mensagem { get; set; } = "Perfil desvinculado com sucesso.";
}