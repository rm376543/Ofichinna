using Ofichina.Domain.Exceptions;

namespace Ofichina.Domain.Entities;

/// <summary>
/// Representa o vínculo entre um usuário e um perfil de autorização.
/// Essa entidade é responsável por manter a associação entre usuários
/// e os perfis que determinam seu acesso na aplicação.
/// </summary>
public class UsuarioPerfil : Entity
{
    /// <summary>
    /// Identificador do usuário associado ao perfil.
    /// </summary>
    public Guid UsuarioId { get; private set; } = Guid.Empty;


    /// <summary>
    /// Identificador do perfil associado ao usuário.
    /// </summary>
    public Guid PerfilId { get; private set; } = Guid.Empty;


    /// <summary>
    /// Usuário relacionado ao vínculo.
    /// </summary>
    public Usuario Usuario { get; private set; } = default!;


    /// <summary>
    /// Perfil relacionado ao vínculo.
    /// </summary>
    public Perfil Perfil { get; private set; } = default!;


    /// <summary>
    /// Construtor utilizado pelo Entity Framework Core.
    /// </summary>
    private UsuarioPerfil()
    {
        // EF Core
    }


    /// <summary>
    /// Cria um novo vínculo entre usuário e perfil.
    /// </summary>
    /// <param name="usuarioId">
    /// Identificador do usuário.
    /// </param>
    /// <param name="perfilId">
    /// Identificador do perfil.
    /// </param>
    public UsuarioPerfil(
        Guid usuarioId,
        Guid perfilId)
    {
        if (usuarioId == Guid.Empty)
            throw new DomainException(
                "O usuário deve ser informado.");


        if (perfilId == Guid.Empty)
            throw new DomainException(
                "O perfil deve ser informado.");


        UsuarioId = usuarioId;

        PerfilId = perfilId;
    }
}