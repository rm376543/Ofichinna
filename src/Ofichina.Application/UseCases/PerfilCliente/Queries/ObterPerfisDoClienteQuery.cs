using Ofichina.Application.Abstractions;

namespace Ofichina.Application.UseCases.PerfilCliente.Queries;

public sealed class ObterPerfisDoClienteQuery : IQuery<IReadOnlyCollection<string>>
{
    public Guid UsuarioId { get; }

    public ObterPerfisDoClienteQuery(Guid usuarioId)
    {
        UsuarioId = usuarioId;
    }
}