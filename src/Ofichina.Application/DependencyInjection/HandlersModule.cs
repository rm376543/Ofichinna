using Microsoft.Extensions.DependencyInjection;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Perfis.Commands;
using Ofichina.Application.UseCases.Perfis.Handlers;
using Ofichina.Application.UseCases.Perfis.Queries;
using Ofichina.Contracts.Responses.Perfil;

namespace Ofichina.Application.DependencyInjection;

/// <summary>
/// Módulo de registro de handlers (CQRS) da aplicação.
/// Registra todos os handlers de commands e queries.
/// </summary>
public static class HandlersModule
{
    public static IServiceCollection AddHandlers(this IServiceCollection services)
    {
        // Perfis
        services.AddScoped<ICommandHandler<CreatePerfilCommand, Guid>, CreatePerfilCommandHandler>();
        services.AddScoped<ICommandHandler<UpdatePerfilCommand>, UpdatePerfilCommandHandler>();
        services.AddScoped<ICommandHandler<DeletePerfilCommand>, DeletePerfilCommandHandler>();

        services.AddScoped<IQueryHandler<GetPerfilByIdQuery, PerfilResponse?>, GetPerfilByIdQueryHandler>();
        services.AddScoped<IQueryHandler<GetPerfisQuery, IReadOnlyCollection<PerfilResponse>>, GetPerfisQueryHandler>();

        return services;
    }
}
