namespace Ofichina.Contracts.Responses.Cliente;

public sealed class DesvincularPerfilClienteResponse
{
    public Guid UsuarioId { get; set; }

    public Guid PerfilId { get; set; }

    public string Mensagem { get; set; } = "Perfil desvinculado com sucesso.";
}