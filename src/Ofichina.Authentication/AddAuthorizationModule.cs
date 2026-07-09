using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Ofichina.Contracts.Enums;

namespace Ofichina.Authentication
{
    /// <summary>
    /// Módulo de registro de Autenticacao do Sistema
    /// Registra toda logica de autenticacao do sistema
    /// </summary>
    public static class AuthorizationModule
    {
        public static IServiceCollection AddAuthorizationModule(
            this IServiceCollection services)
        {
            services.AddAuthorizationBuilder()
                .SetFallbackPolicy(new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build())
                .AddPolicy(UserPolicyEnum.Ler, policy =>
                    policy.RequireRole(UserRolesEnum.Usuario, UserRolesEnum.Admin))
                .AddPolicy(UserPolicyEnum.Escrever, policy =>
                    policy.RequireRole(UserRolesEnum.Admin))
                .AddPolicy(UserPolicyEnum.Apagar, policy =>
                    policy.RequireRole(UserRolesEnum.Admin));

            return services;
        }
    }
}
