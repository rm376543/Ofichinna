namespace Ofichina.Contracts.Responses.PerfilUsuario;

public sealed class VincularPerfilUsuarioResponse
{
    public Guid UsuarioId { get; set; }

    public Guid PerfilId { get; set; }

    public string Mensagem { get; set; } = "Perfil vinculado com sucesso.";
}