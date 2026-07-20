using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;

namespace Ofichina.Application.UseCases.PerfilPermissoes.Commands;

public sealed class VincularPermissaoPerfilCommand : ICommand<Result>
{
    public Guid PerfilId { get; }

    public Guid PermissaoId { get; }

    public VincularPermissaoPerfilCommand(Guid perfilId, Guid permissaoId)
    {
        PerfilId = perfilId;
        PermissaoId = permissaoId;
    }
}
