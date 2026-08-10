using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ofichina.Application.UseCases.Orcamentos.Commands;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Enums;
using Ofichina.Domain.ValueObjects;
using Ofichina.Infrastructure.Persistence;

namespace Ofichina.IntegrationTests.Application.Orcamentos;

public sealed class AprovarOrcamentoFlowTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public AprovarOrcamentoFlowTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _ = _factory.CreateClient();
    }

    [Fact]
    public async Task AprovarOrcamento_Deve_Criar_Ordem_De_Servico_Com_ProblemaRelatado_E_Vincular_Items()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var dados = CriarCenario(context);

        var result = await mediator.Send(new AprovarOrcamentoCommand(dados.Orcamento.Id, 78123));

        Assert.True(result.IsSuccess);

        var ordemServico = await context.Set<OrdemServico>()
            .Include(x => x.Servicos)
                .ThenInclude(x => x.Servico)
            .SingleAsync(x => x.PessoaId == dados.Orcamento.PessoaId && x.VeiculoId == dados.Orcamento.VeiculoId);

        Assert.Equal(StatusOrdemServico.Criado, ordemServico.Status);
        Assert.Equal(78123, ordemServico.Hodometro);
        Assert.Equal(dados.Orcamento.Observacoes, ordemServico.ProblemaRelatado);
        Assert.Equal(dados.Orcamento.PessoaId, ordemServico.PessoaId);
        Assert.Equal(dados.Orcamento.VeiculoId, ordemServico.VeiculoId);
        Assert.Equal(dados.Orcamento.ConsultorId, ordemServico.ConsultorId);

        var itensVinculados = await context.Set<ItemServico>()
            .AsNoTracking()
            .Where(x => x.OrdemServicoId == ordemServico.Id)
            .ToListAsync();

        Assert.Single(itensVinculados);
        Assert.Equal(dados.Servico.Id, itensVinculados[0].ServicoId);
        Assert.Equal(dados.Orcamento.Id, itensVinculados[0].OrcamentoId);
    }

    private static Cenario CriarCenario(ApplicationDbContext context)
    {
        var cliente = new Pessoa(
            "Cliente Integração",
            new Cpf("39053344705"),
            new Telefone("11999999999"),
            new Endereco("Rua Integração", "100", null, "Centro", "São Paulo", "SP", new Cep("01001000")),
            Guid.NewGuid());

        var consultor = new Pessoa(
            "Consultor Integração",
            new Cpf("39053344706"),
            new Telefone("11999999998"),
            new Endereco("Rua Integração", "200", null, "Centro", "São Paulo", "SP", new Cep("01001000")),
            Guid.NewGuid());

        var mecanico = new Pessoa(
            "Mecânico Integração",
            new Cpf("39053344707"),
            new Telefone("11999999997"),
            new Endereco("Rua Integração", "300", null, "Centro", "São Paulo", "SP", new Cep("01001000")),
            Guid.NewGuid());

        var veiculo = new Veiculo(
            cliente.Id,
            new Placa("ABC1234"),
            "Volkswagen",
            "Gol",
            2020,
            "Prata",
            new Hodometro(100000));

        var diaDisponibilidade = new DiaDisponibilidade(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)));
        var horarioDisponibilidade = new HorarioDisponibilidade(new TimeOnly(9, 0));
        var agendaConsultor = new AgendaConsultor(diaDisponibilidade.Id, horarioDisponibilidade.Id, consultor.Id);
        var agendamento = new Agendamento(cliente.Id, agendaConsultor.Id, veiculo.Id, 78123, "Visita técnica");

        var servico = new Servico("Troca de óleo", null, 120m);
        var orcamento = new Orcamento(
            cliente.Id,
            veiculo.Id,
            agendamento.Id,
            mecanico.Id,
            consultor.Id,
            DateTime.UtcNow.AddDays(10),
            0m,
            "Barulhos durante a aceleração");

        orcamento.AdicionarServico(servico.Id, null, 1, StatusOrcamento.Criado);
        orcamento.IniciarDiagnostico();
        orcamento.FinalizarDiagnostico();
        orcamento.EnviarParaCliente();
        orcamento.Aprovar();

        context.AddRange(cliente, consultor, mecanico, veiculo, diaDisponibilidade, horarioDisponibilidade, agendaConsultor, agendamento, servico, orcamento);
        context.SaveChanges();

        return new Cenario(orcamento, servico);
    }

    private sealed record Cenario(Orcamento Orcamento, Servico Servico);
}
