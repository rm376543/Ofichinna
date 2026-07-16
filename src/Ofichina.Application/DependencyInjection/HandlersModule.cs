

using Microsoft.Extensions.DependencyInjection;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Autenticacao.Commands;
using Ofichina.Application.UseCases.Autenticacao.Handlers;
using Ofichina.Application.UseCases.PerfilUsuario.Commands;
using Ofichina.Application.UseCases.PerfilUsuario.Handlers;
using Ofichina.Application.UseCases.PerfilUsuario.Queries;
using Ofichina.Application.UseCases.Perfis.Commands;
using Ofichina.Application.UseCases.Perfis.Handlers;
using Ofichina.Application.UseCases.Perfis.Queries;
using Ofichina.Application.UseCases.Pessoas.Commands;
using Ofichina.Application.UseCases.Pessoas.Handlers;
using Ofichina.Application.UseCases.Pessoas.Queries;
using Ofichina.Application.UseCases.Veiculos.Commands;
using Ofichina.Application.UseCases.Veiculos.Handlers;
using Ofichina.Application.UseCases.Veiculos.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses;
using Ofichina.Contracts.Responses.Perfil;
using Ofichina.Contracts.Responses.PerfilUsuario;
using Ofichina.Contracts.Responses.Pessoa;
using Ofichina.Contracts.Responses.Veiculo;

namespace Ofichina.Application.DependencyInjection;

/// <summary>
/// Módulo de registro de handlers (CQRS) da aplicação.
/// Registra todos os handlers de commands e queries.
/// </summary>
public static class HandlersModule
{
    public static IServiceCollection AddHandlers(this IServiceCollection services)
    {
        // Autenticacao
        services.AddScoped<ICommandHandler<AutenticarCommand, Result<AutenticacaoResponse>>, AutenticarCommandHandler>();
        services.AddScoped<ICommandHandler<CadastrarUsuarioCommand, Result<AutenticacaoResponse>>, CadastrarUsuarioCommandHandler>();

        // Perfis
        services.AddScoped<ICommandHandler<CreatePerfilCommand, Result>, CreatePerfilCommandHandler>();
        services.AddScoped<ICommandHandler<UpdatePerfilCommand, Result>, UpdatePerfilCommandHandler>();
        services.AddScoped<ICommandHandler<DeletePerfilCommand, Result>, DeletePerfilCommandHandler>();

        services.AddScoped<IQueryHandler<GetPerfilByIdQuery, Result<PerfilResponse>>, GetPerfilByIdQueryHandler>();
        services.AddScoped<IQueryHandler<GetPerfisQuery, Result<IReadOnlyCollection<PerfilResponse>>>, GetPerfisQueryHandler>();

        // Pessoa
        services.AddScoped<ICommandHandler<CreatePessoaCommand, Result<Guid>>, CreatePessoaCommandHandler>();
        services.AddScoped<ICommandHandler<UpdatePessoaCommand, Result>, UpdatePessoaCommandHandler>();
        services.AddScoped<ICommandHandler<DeletePessoaCommand, Result>, DeletePessoaCommandHandler>();
        services.AddScoped<IQueryHandler<GetPessoaByIdQuery, Result<PessoaResponse>>, GetPessoaByIdQueryHandler>();
        services.AddScoped<IQueryHandler<GetPessoasQuery, Result<IReadOnlyCollection<PessoaResponse>>>, GetPessoaQueryHandler>();

        // Veiculos
        services.AddScoped<ICommandHandler<CreateVeiculoCommand, Result<Guid>>, CreateVeiculoCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateVeiculoCommand, Result>, UpdateVeiculoCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteVeiculoCommand, Result>, DeleteVeiculoCommandHandler>();
        services.AddScoped<IQueryHandler<GetVeiculosQuery, Result<IReadOnlyCollection<VeiculoResponse>>>, GetVeiculosQueryHandler>();
        services.AddScoped<IQueryHandler<GetVeiculoByIdQuery, Result<VeiculoResponse>>, GetVeiculoByIdQueryHandler>();

        //UsuarioPerfil
        services.AddScoped<ICommandHandler<VincularPerfilUsuarioCommand, Result<VincularPerfilUsuarioResponse>>, VincularPerfilUsuarioCommandHandler>();
        services.AddScoped<ICommandHandler<DesvincularPerfilUsuarioCommand, Result<DesvincularPerfilUsuarioResponse>>, DesvincularPerfilUsuarioCommandHandler>();
        services.AddScoped<IQueryHandler<ObterPerfisDoUsuarioQuery, IReadOnlyCollection<string>>, ObterPerfisDoUsuarioQueryHandler>();

        return services;
    }
}
