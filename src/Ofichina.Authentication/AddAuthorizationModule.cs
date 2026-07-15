using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Ofichina.Authentication.Security;
using Ofichina.Contracts.Enums;

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
            var authorizationBuilder = services.AddAuthorizationBuilder();

            authorizationBuilder.SetFallbackPolicy(new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build());

            services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
            services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

            return services;
        }
    }
}