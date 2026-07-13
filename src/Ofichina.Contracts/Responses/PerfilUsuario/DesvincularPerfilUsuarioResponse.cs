namespace Ofichina.Contracts.Responses.PerfilUsuario;

public sealed class DesvincularPerfilUsuarioResponse
{
    public Guid UsuarioId { get; set; }

    public Guid PerfilId { get; set; }

    public string Mensagem { get; set; } = "Perfil desvinculado com sucesso.";
}