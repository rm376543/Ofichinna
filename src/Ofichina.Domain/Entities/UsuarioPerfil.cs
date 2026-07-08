namespace Ofichina.Domain.Entities;

/// <summary>
/// Representa o vínculo entre usuário e perfil.
/// </summary>
public class UsuarioPerfil : Entity
{
    public Guid UsuarioId { get; set; }

    public Guid PerfilId { get; set; }

    public Usuario Usuario { get; set; } = default!;

    public Perfil Perfil { get; set; } = default!;

    private UsuarioPerfil()
    {
    }

    public UsuarioPerfil(Guid usuarioId, Guid perfilId)
    {
        UsuarioId = usuarioId;
        PerfilId = perfilId;
    }
}