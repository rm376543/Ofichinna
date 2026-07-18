

using Microsoft.Extensions.DependencyInjection;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Autenticacao.Commands;
using Ofichina.Application.UseCases.Autenticacao.Handlers;
using Ofichina.Application.UseCases.Agendamentos.Commands;
using Ofichina.Application.UseCases.Agendamentos.Handlers;
using Ofichina.Application.UseCases.Agendamentos.Queries;
using Ofichina.Application.UseCases.PerfilUsuario.Commands;
using Ofichina.Application.UseCases.PerfilUsuario.Handlers;
using Ofichina.Application.UseCases.PerfilUsuario.Queries;
using Ofichina.Application.UseCases.Perfis.Commands;
using Ofichina.Application.UseCases.Perfis.Handlers;
using Ofichina.Application.UseCases.Perfis.Queries;
using Ofichina.Application.UseCases.OrdensServico.Commands;
using Ofichina.Application.UseCases.OrdensServico.Handlers;
using Ofichina.Application.UseCases.OrdensServico.Queries;
using Ofichina.Application.UseCases.OrdensServico.ItemServico.Commands;
using Ofichina.Application.UseCases.OrdensServico.ItemServico.Handlers;
using Ofichina.Application.UseCases.OrdensServico.ItemServico.Queries;
using Ofichina.Application.UseCases.OrdensServico.ItemPeca.Commands;
using Ofichina.Application.UseCases.OrdensServico.ItemPeca.Handlers;
using Ofichina.Application.UseCases.Pessoas.Commands;
using Ofichina.Application.UseCases.Pessoas.Handlers;
using Ofichina.Application.UseCases.Pessoas.Queries;
using Ofichina.Application.UseCases.Pecas.Commands;
using Ofichina.Application.UseCases.Pecas.Handlers;
using Ofichina.Application.UseCases.Pecas.Queries;
using Ofichina.Application.UseCases.Servicos.Commands;
using Ofichina.Application.UseCases.Servicos.Handlers;
using Ofichina.Application.UseCases.Servicos.Queries;
using Ofichina.Application.UseCases.Veiculos.Commands;
using Ofichina.Application.UseCases.Veiculos.Handlers;
using Ofichina.Application.UseCases.Veiculos.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses;
using Ofichina.Contracts.Responses.Agendamento;
using Ofichina.Contracts.Responses.Perfil;
using Ofichina.Contracts.Responses.PerfilUsuario;
using Ofichina.Contracts.Responses.OrdemServico;
using Ofichina.Contracts.Responses.Pessoa;
using Ofichina.Contracts.Responses.Pecas;
using Ofichina.Contracts.Responses.Servicos;
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

        // Agendamentos
        services.AddScoped<ICommandHandler<CreateAgendamentoCommand, Result<AgendamentoResponse>>, CreateAgendamentoCommandHandler>();
        services.AddScoped<IQueryHandler<GetAgendamentosQuery, Result<IReadOnlyCollection<AgendamentoResponse>>>, GetAgendamentosQueryHandler>();
        services.AddScoped<IQueryHandler<GetAgendamentoByIdQuery, Result<AgendamentoResponse>>, GetAgendamentoByIdQueryHandler>();

        // Ordens de Serviço
        services.AddScoped<ICommandHandler<CreateOrdemServicoCommand, Result<Guid>>, CreateOrdemServicoCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateOrdemServicoCommand, Result>, UpdateOrdemServicoCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteOrdemServicoCommand, Result>, DeleteOrdemServicoCommandHandler>();
        services.AddScoped<ICommandHandler<AlterarStatusOrdemServicoCommand, Result>, AlterarStatusOrdemServicoCommandHandler>();
        services.AddScoped<IQueryHandler<GetOrdensServicoQuery, Result<IReadOnlyCollection<OrdemServicoResponse>>>, GetOrdensServicoQueryHandler>();
        services.AddScoped<IQueryHandler<GetOrdemServicoByIdQuery, Result<OrdemServicoResponse>>, GetOrdemServicoByIdQueryHandler>();

        services.AddScoped<ICommandHandler<CreateItemServicoCommand, Result<Guid>>, CreateItemServicoCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateItemServicoCommand, Result>, UpdateItemServicoCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteItemServicoCommand, Result>, DeleteItemServicoCommandHandler>();
        services.AddScoped<IQueryHandler<GetItemServicosByOrdemServicoQuery, Result<IReadOnlyCollection<ItemServicoResponse>>>, GetItemServicosByOrdemServicoQueryHandler>();
        services.AddScoped<IQueryHandler<GetItemServicoByIdQuery, Result<ItemServicoResponse>>, GetItemServicoByIdQueryHandler>();
        services.AddScoped<ICommandHandler<UtilizarItemPecaCommand, Result>, UtilizarItemPecaCommandHandler>();

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

        // Pecas
        services.AddScoped<ICommandHandler<CreatePecaCommand, Result<Guid>>, CreatePecaCommandHandler>();
        services.AddScoped<ICommandHandler<UpdatePecaCommand, Result>, UpdatePecaCommandHandler>();
        services.AddScoped<ICommandHandler<DeletePecaCommand, Result>, DeletePecaCommandHandler>();
        services.AddScoped<IQueryHandler<GetPecaByIdQuery, Result<PecaResponse>>, GetPecaByIdQueryHandler>();
        services.AddScoped<IQueryHandler<GetPecasQuery, Result<IReadOnlyCollection<PecaResponse>>>, GetPecasQueryHandler>();

        // Servicos
        services.AddScoped<ICommandHandler<CreateServicoCommand, Result<Guid>>, CreateServicoCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateServicoCommand, Result>, UpdateServicoCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteServicoCommand, Result>, DeleteServicoCommandHandler>();
        services.AddScoped<IQueryHandler<GetServicosQuery, Result<IReadOnlyCollection<ServicoResponse>>>, GetServicosQueryHandler>();
        services.AddScoped<IQueryHandler<GetServicoByIdQuery, Result<ServicoResponse>>, GetServicoByIdQueryHandler>();

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
