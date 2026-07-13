using Ofichina.Application.Abstractions;

namespace Ofichina.Application.UseCases.PerfilUsuario.Queries;

public sealed class ObterPerfisDoUsuarioQuery : IQuery<IReadOnlyCollection<string>>
{
    public Guid UsuarioId { get; }

    public ObterPerfisDoUsuarioQuery(Guid usuarioId)
    {
        UsuarioId = usuarioId;
    }
}