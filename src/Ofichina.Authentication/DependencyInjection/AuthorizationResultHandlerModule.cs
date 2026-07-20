using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Ofichina.Authentication.Security;

namespace Ofichina.Authentication;

public static class AuthorizationResultHandlerModule
{
    public static IServiceCollection AddAuthorizationResultHandlerModule(
        this IServiceCollection services)
    {
        services.AddSingleton<IAuthorizationMiddlewareResultHandler, ApiAuthorizationMiddlewareResultHandler>();
        return services;
    }
}