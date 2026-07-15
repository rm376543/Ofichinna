using Ofichina.Domain.Exceptions;

namespace Ofichina.Domain.Entities;

public class PerfilPermissao : Entity
{
    public Guid PerfilId { get; private set; } = Guid.Empty;

    public Guid PermissaoId { get; private set; } = Guid.Empty;

    public Perfil Perfil { get; private set; } = default!;

    public Permissao Permissao { get; private set; } = default!;

    private PerfilPermissao()
    {
    }

    public PerfilPermissao(Guid perfilId, Guid permissaoId)
    {
        if (perfilId == Guid.Empty)
            throw new DomainException("O perfil deve ser informado.");

        if (permissaoId == Guid.Empty)
            throw new DomainException("A permissão deve ser informada.");

        PerfilId = perfilId;
        PermissaoId = permissaoId;
    }
}