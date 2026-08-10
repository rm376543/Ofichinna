using Microsoft.AspNetCore.Authorization;
using Ofichina.Application.Abstractions.Authentication.Service;
using System.Security.Claims;

namespace Ofichina.Authentication.Security;

public sealed class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IProfileAuthService _perfilAutorizacaoService;

    public PermissionAuthorizationHandler(IProfileAuthService perfilAutorizacaoService)
    {
        _perfilAutorizacaoService = perfilAutorizacaoService;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var usuarioIdValue =
            context.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(usuarioIdValue, out var usuarioId))
            return;

        var possuiPermissao = await _perfilAutorizacaoService
            .PossuiPermissaoAsync(usuarioId, requirement.Permission);

        if (possuiPermissao)
            context.Succeed(requirement);
    }
}