using Microsoft.Extensions.DependencyInjection;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Autenticacao.Commands;
using Ofichina.Application.UseCases.Autenticacao.Handlers;
using Ofichina.Application.UseCases.PerfilUsuario.Commands;
using Ofichina.Application.UseCases.PerfilUsuario.Handlers;
using Ofichina.Application.UseCases.Perfis.Commands;
using Ofichina.Application.UseCases.Perfis.Handlers;
using Ofichina.Application.UseCases.Perfis.Queries;
using Ofichina.Application.UseCases.PerfilUsuario.Queries;
using Ofichina.Contracts.Responses;
using Ofichina.Contracts.Responses.PerfilUsuario;
using Ofichina.Contracts.Responses.Perfil;
using Ofichina.Contracts.Common;

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

        // Autenticacao
        services.AddScoped<ICommandHandler<AutenticarCommand, Result<AutenticacaoResponse>>, AutenticarCommandHandler>();
        services.AddScoped<ICommandHandler<CadastrarClienteCommand, Result<AutenticacaoResponse>>, CadastrarClienteCommandHandler>();

        //UsuarioPerfil
        services.AddScoped<ICommandHandler<VincularPerfilUsuarioCommand, Result<VincularPerfilUsuarioResponse>>, VincularPerfilUsuarioCommandHandler>();
        services.AddScoped<ICommandHandler<DesvincularPerfilUsuarioCommand, Result<DesvincularPerfilUsuarioResponse>>, DesvincularPerfilUsuarioCommandHandler>();
        services.AddScoped<IQueryHandler<ObterPerfisDoUsuarioQuery, IReadOnlyCollection<string>>, ObterPerfisDoUsuarioQueryHandler>();

        return services;
    }
}
