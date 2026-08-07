using Ofichina.Contracts.Common;

namespace Ofichina.Contracts.Responses.PerfilUsuario;

public sealed class VincularPerfilUsuarioResponse : BaseResponse
{
    public Guid VincularPerfilUsuarioId { get; set; }

    public Guid UsuarioId { get; set; }

    public Guid PerfilId { get; set; }

    public string Mensagem { get; set; } = "Perfil vinculado com sucesso.";
}