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

            return services;
        }
    }
}
