using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Cliente.Queries;
using Ofichina.Authentication.Abstractions;

namespace Ofichina.Application.UseCases.Cliente.Handlers;

public sealed class ObterPerfisDoClienteQueryHandler
    : IQueryHandler<ObterPerfisDoClienteQuery, IReadOnlyCollection<string>>
{
    private readonly IPerfilAutorizacaoService _perfilService;

    public ObterPerfisDoClienteQueryHandler(IPerfilAutorizacaoService perfilService)
    {
        _perfilService = perfilService;
    }

    public Task<IReadOnlyCollection<string>> HandleAsync(ObterPerfisDoClienteQuery query)
        => _perfilService.ObterPerfisAsync(query.UsuarioId);
}