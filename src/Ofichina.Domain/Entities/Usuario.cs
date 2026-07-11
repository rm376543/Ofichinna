using Ofichina.Domain.ValueObjects;

namespace Ofichina.Domain.Entities;

/// <summary>
/// Representa um usuário autenticável da aplicação.
/// </summary>
public class Usuario : Entity
{
    /// <summary>
    /// E-mail utilizado para autenticação.
    /// </summary>
    public Email Email { get; set; } = default!;

    /// <summary>
    /// Hash da senha do usuário.
    /// </summary>
    public string SenhaHash { get; set; } = string.Empty;

    /// <summary>
    /// Perfis associados ao usuário.
    /// </summary>
    public ICollection<UsuarioPerfil> Perfis { get; set; } = [];

    private Usuario()
    {
    }

    public Usuario(Email email, string senhaHash)
    {
        Email = email;
        SenhaHash = senhaHash;
    }

    public bool UsuarioEstaAtivo ()
    {
        return DeletedAt == null;
    }
}