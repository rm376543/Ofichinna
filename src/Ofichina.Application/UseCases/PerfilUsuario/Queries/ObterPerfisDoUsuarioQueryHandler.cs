using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.PerfilUsuario.Queries;
using Ofichina.Authentication.Abstractions;

namespace Ofichina.Application.UseCases.PerfilUsuario.Handlers;

public sealed class ObterPerfisDoUsuarioQueryHandler
    : IQueryHandler<ObterPerfisDoUsuarioQuery, IReadOnlyCollection<string>>
{
    private readonly IPerfilAutorizacaoService _perfilService;

    public ObterPerfisDoUsuarioQueryHandler(IPerfilAutorizacaoService perfilService)
    {
        _perfilService = perfilService;
    }

    public Task<IReadOnlyCollection<string>> HandleAsync(ObterPerfisDoUsuarioQuery query, CancellationToken cancellationToken = default)
        => _perfilService.ObterPerfisAsync(query.UsuarioId);
}
