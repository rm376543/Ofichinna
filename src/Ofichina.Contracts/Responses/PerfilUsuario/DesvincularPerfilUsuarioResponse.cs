using Ofichina.Contracts.Common;

namespace Ofichina.Contracts.Responses.PerfilUsuario;

public sealed class DesvincularPerfilUsuarioResponse : BaseResponse
{
    public Guid DesvincularPerfilUsuarioId { get; set; }

    public Guid UsuarioId { get; set; }

    public Guid PerfilId { get; set; }

    public string Mensagem { get; set; } = "Perfil desvinculado com sucesso.";
}