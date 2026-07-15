using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Ofichina.Authentication.Security;

namespace Ofichina.Authentication
{
    /// <summary>
    /// Módulo de registro de Autorização do sistema.
    /// </summary>
    public static class AuthorizationModule
    {
        public static IServiceCollection AddAuthorizationModule(
            this IServiceCollection services)
        {
            services.AddAuthorizationBuilder()
                .SetFallbackPolicy(new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build());

            services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
            services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

            return services;
        }
    }
}