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
            var authorizationBuilder = services.AddAuthorizationBuilder();

            authorizationBuilder.SetFallbackPolicy(new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build());

            authorizationBuilder.AddPolicy(UserPolicyEnum.Ler, policy =>
                policy.RequireRole(UserRolesEnum.Usuario, UserRolesEnum.Admin));

            authorizationBuilder.AddPolicy(UserPolicyEnum.Escrever, policy =>
                policy.RequireRole(UserRolesEnum.Admin));

            authorizationBuilder.AddPolicy(UserPolicyEnum.Atualizar, policy =>
                policy.RequireRole(UserRolesEnum.Admin));

            authorizationBuilder.AddPolicy(UserPolicyEnum.Deletar, policy =>
                policy.RequireRole(UserRolesEnum.Admin));
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