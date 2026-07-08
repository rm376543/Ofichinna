using Ofichina.Domain.Shared;

namespace Ofichina.Domain.Entities;

/// <summary>
/// Representa um usuário autenticável da aplicação.
/// </summary>
public class Usuario : Entity
{
    /// <summary>
    /// Nome de exibição do usuário.
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// E-mail utilizado para autenticação.
    /// </summary>
    public Email Email { get; set; } = default!;

    /// <summary>
    /// Hash da senha do usuário.
    /// </summary>
    public string SenhaHash { get; set; } = string.Empty;

    /// <summary>
    /// Indica se o usuário pode autenticar.
    /// </summary>
    public bool Ativo { get; set; } = true;

    /// <summary>
    /// Perfis associados ao usuário.
    /// </summary>
    public ICollection<UsuarioPerfil> Perfis { get; set; } = [];

    private Usuario()
    {
    }

    public Usuario(string nome, Email email, string senhaHash)
    {
        Nome = nome;
        Email = email;
        SenhaHash = senhaHash;
    }
}