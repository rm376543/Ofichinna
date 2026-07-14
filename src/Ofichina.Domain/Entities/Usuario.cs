using Ofichina.Domain.Exceptions;
using Ofichina.Domain.ValueObjects;

namespace Ofichina.Domain.Entities;

/// <summary>
/// Representa um usuário autenticável da aplicação.
/// </summary>
public class Usuario : Entity
{
    private readonly List<UsuarioPerfil> _perfis = [];

    /// <summary>
    /// E-mail utilizado para autenticação.
    /// </summary>
    public Email Email { get; private set; } = null!;

    /// <summary>
    /// Hash da senha do usuário.
    /// </summary>
    public string SenhaHash { get; private set; } = string.Empty;

    /// <summary>
    /// Perfis associados ao usuário.
    /// </summary>
    public IReadOnlyCollection<UsuarioPerfil> Perfis => _perfis.AsReadOnly();

    private Usuario()
    {
        // Necessário para o Entity Framework
    }

    public Usuario(Email email, string senhaHash)
    {
        if (email is null)
            throw new DomainException("O e-mail deve ser informado.");

        if (string.IsNullOrWhiteSpace(senhaHash))
            throw new DomainException("A senha deve ser informada.");

        Email = email;
        SenhaHash = senhaHash;
    }

    /// <summary>
    /// Altera o e-mail do usuário.
    /// </summary>
    public void AlterarEmail(Email email)
    {
        if (email is null)
            throw new DomainException("O e-mail deve ser informado.");

        Email = email;
    }

    /// <summary>
    /// Altera o hash da senha do usuário.
    /// </summary>
    public void AlterarSenha(string senhaHash)
    {
        if (string.IsNullOrWhiteSpace(senhaHash))
            throw new DomainException("A senha deve ser informada.");

        SenhaHash = senhaHash;
    }

    /// <summary>
    /// Adiciona um perfil ao usuário.
    /// </summary>
    public void AdicionarPerfil(UsuarioPerfil usuarioPerfil)
    {
        if (usuarioPerfil is null)
            throw new DomainException("O perfil deve ser informado.");

        if (_perfis.Any(p => p.PerfilId == usuarioPerfil.PerfilId))
            return;

        _perfis.Add(usuarioPerfil);
    }

    /// <summary>
    /// Remove um perfil do usuário.
    /// </summary>
    public void RemoverPerfil(Guid perfilId)
    {
        var usuarioPerfil = _perfis.FirstOrDefault(p => p.PerfilId == perfilId);

        if (usuarioPerfil is null)
            return;

        _perfis.Remove(usuarioPerfil);
    }

    /// <summary>
    /// Indica se o usuário está ativo.
    /// </summary>
    public bool EstaAtivo()
    {
        return DeletedAt is null;
    }
}