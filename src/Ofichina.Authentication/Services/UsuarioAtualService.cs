using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Ofichina.Application.Abstractions.Authentication;

namespace Ofichina.Authentication.Services;

public sealed class UsuarioAtualService : IUsuarioAtualService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UsuarioAtualService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? ObterUsuarioId()
    {
        var usuario = _httpContextAccessor.HttpContext?.User;
        if (usuario is null)
            return null;

        var value = usuario.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? usuario.FindFirstValue(JwtRegisteredClaimNames.Sub);

        return Guid.TryParse(value, out var usuarioId)
            ? usuarioId
            : null;
    }
}
