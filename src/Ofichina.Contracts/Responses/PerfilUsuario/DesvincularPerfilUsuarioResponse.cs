using Ofichina.Contracts.Common;

namespace Ofichina.Contracts.Responses.PerfilUsuario;

public sealed class DesvincularPerfilUsuarioResponse : BaseEntity
{
    public Guid UsuarioId { get; set; }

    public Guid PerfilId { get; set; }

    public string Mensagem { get; set; } = "Perfil desvinculado com sucesso.";
}