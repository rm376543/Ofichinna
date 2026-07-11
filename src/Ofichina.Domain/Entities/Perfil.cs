namespace Ofichina.Domain.Entities;

/// <summary>
/// Representa um perfil de autorização da aplicação.
/// </summary>
public class Perfil : Entity
{
    /// <summary>
    /// Nome descritivo do perfil.
    /// </summary>
    public string NomePerfil { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;

    /// <summary>
    /// Usuários vinculados ao perfil.
    /// </summary>
    public ICollection<UsuarioPerfil> Usuarios { get; set; } = [];

    private Perfil()
    {
    }

    public Perfil(string nomePerfil, string descricao)
    {
        NomePerfil = nomePerfil;
        Descricao = descricao;
    }

    public bool PerfilEstaAtivo ()
    {
        return DeletedAt == null;
    }
}