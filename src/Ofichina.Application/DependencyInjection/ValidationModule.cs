using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Ofichina.Application.DependencyInjection;

/// <summary>
/// Módulo de registro de validações da aplicação.
/// Utiliza FluentValidation para validar requisições.
/// </summary>
public static class ValidationModule
{
    public static IServiceCollection AddValidations(this IServiceCollection services)
    {
        // Registra todos os validadores do assembly da aplicação
        var assembly = typeof(ValidationModule).Assembly;
        services.AddValidatorsFromAssembly(assembly, includeInternalTypes: true);

        return services;
    }
}
