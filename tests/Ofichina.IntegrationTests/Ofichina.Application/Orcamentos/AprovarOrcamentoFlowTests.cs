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
        await using var scope = _factory.Services.CreateAsyncScope();

        var context = scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();

        var mediator = scope.ServiceProvider
            .GetRequiredService<IMediator>();

        var dados = await CriarCenarioAsync(context);

        var result = await mediator.Send(
            new AprovarOrcamentoCommand(dados.Orcamento.Id));

        Assert.True(
            result.IsSuccess,
            result.Error?.ToString());

        var ordemServico = await context.Set<OrdemServico>()
            .Include(x => x.Servicos)
                .ThenInclude(x => x.Servico)
            .SingleAsync(x =>
                x.PessoaId == dados.Orcamento.PessoaId &&
                x.VeiculoId == dados.Orcamento.VeiculoId);

        Assert.Equal(
            StatusOrdemServico.Criado,
            ordemServico.Status);

        Assert.Equal(
            dados.HodometroAgendamento,
            ordemServico.Hodometro);

        Assert.Equal(
            dados.Orcamento.Observacoes,
            ordemServico.ProblemaRelatado);

        Assert.Equal(
            dados.Orcamento.PessoaId,
            ordemServico.PessoaId);

        Assert.Equal(
            dados.Orcamento.VeiculoId,
            ordemServico.VeiculoId);

        Assert.Equal(
            dados.Orcamento.ConsultorId,
            ordemServico.ConsultorId);

        var itensVinculados = await context.Set<ItemServico>()
            .AsNoTracking()
            .Where(x => x.OrdemServicoId == ordemServico.Id)
            .ToListAsync();

        Assert.Single(itensVinculados);

        Assert.Equal(
            dados.Servico.Id,
            itensVinculados[0].ServicoId);

        Assert.Equal(
            dados.Orcamento.Id,
            itensVinculados[0].OrcamentoId);
    }

    private static async Task<Cenario> CriarCenarioAsync(
        ApplicationDbContext context)
    {
        var sufixo = Guid.NewGuid().ToString("N")[..8];

        var usuarioCliente = new Usuario(
            new Email($"cliente.{sufixo}@ofichina.com"),
            "hash-cliente");

        var usuarioConsultor = new Usuario(
            new Email($"consultor.{sufixo}@ofichina.com"),
            "hash-consultor");

        var usuarioMecanico = new Usuario(
            new Email($"mecanico.{sufixo}@ofichina.com"),
            "hash-mecanico");

        var cliente = new Pessoa(
            $"Cliente Integração {sufixo}",
            new Cpf("12345678909"),
            new Telefone("11999999999"),
            new Endereco(
                "Rua Integração",
                "100",
                null,
                "Centro",
                "São Paulo",
                "SP",
                new Cep("01001000")),
            usuarioCliente.Id);

        var consultor = new Pessoa(
            $"Consultor Integração {sufixo}",
            new Cpf("52998224725"),
            new Telefone("11999999998"),
            new Endereco(
                "Rua Integração",
                "200",
                null,
                "Centro",
                "São Paulo",
                "SP",
                new Cep("01001000")),
            usuarioConsultor.Id);

        var mecanico = new Pessoa(
            $"Mecânico Integração {sufixo}",
            new Cpf("11144477735"),
            new Telefone("11999999997"),
            new Endereco(
                "Rua Integração",
                "300",
                null,
                "Centro",
                "São Paulo",
                "SP",
                new Cep("01001000")),
            usuarioMecanico.Id);

        var veiculo = new Veiculo(
            cliente.Id,
            new Placa(GerarPlaca()),
            "Volkswagen",
            "Gol",
            2020,
            "Prata",
            new Hodometro(100000));

        /*
         * DiasDisponibilidade possui índice UNIQUE sobre Data.
         *
         * Portanto, primeiro procuramos um registro existente.
         * Caso não exista, criamos um novo.
         */
        var dataDisponibilidade =
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(45));

        var diaDisponibilidade =
            await context.Set<DiaDisponibilidade>()
                .SingleOrDefaultAsync(x =>
                    x.Data == dataDisponibilidade);

        if (diaDisponibilidade is null)
        {
            diaDisponibilidade =
                new DiaDisponibilidade(dataDisponibilidade);

            context.Add(diaDisponibilidade);
        }

        /*
         * HorariosDisponibilidade possui índice UNIQUE sobre Hora.
         *
         * Pelo mesmo motivo, reutilizamos o registro existente.
         */
        var horaDisponibilidade = new TimeOnly(13, 30);

        var horarioDisponibilidade =
            await context.Set<HorarioDisponibilidade>()
                .SingleOrDefaultAsync(x =>
                    x.Hora == horaDisponibilidade);

        if (horarioDisponibilidade is null)
        {
            horarioDisponibilidade =
                new HorarioDisponibilidade(horaDisponibilidade);

            context.Add(horarioDisponibilidade);
        }

        /*
         * Precisamos garantir que os registros acima estejam
         * persistidos antes de criar o AgendaConsultor.
         */
        await context.SaveChangesAsync();

        var agendaConsultor = new AgendaConsultor(
            diaDisponibilidade.Id,
            horarioDisponibilidade.Id,
            consultor.Id);

        const int hodometroAgendamento = 78123;

        var agendamento = new Agendamento(
            cliente.Id,
            agendaConsultor.Id,
            veiculo.Id,
            hodometroAgendamento,
            "Visita técnica");

        var servico = new Servico(
            "Troca de óleo",
            null,
            120m);

        var orcamento = new Orcamento(
            cliente.Id,
            veiculo.Id,
            agendamento.Id,
            mecanico.Id,
            consultor.Id,
            DateTime.UtcNow.AddDays(10),
            0m,
            "Barulhos durante a aceleração");

        orcamento.AdicionarServico(
            servico.Id,
            null,
            1,
            StatusOrcamento.Criado);

        orcamento.IniciarDiagnostico();
        orcamento.FinalizarDiagnostico();
        orcamento.EnviarParaCliente();

        context.AddRange(
            usuarioCliente,
            usuarioConsultor,
            usuarioMecanico,
            cliente,
            consultor,
            mecanico,
            veiculo,
            agendaConsultor,
            agendamento,
            servico,
            orcamento);

        await context.SaveChangesAsync();

        return new Cenario(
            orcamento,
            servico,
            hodometroAgendamento);
    }

    private sealed record Cenario(
        Orcamento Orcamento,
        Servico Servico,
        int HodometroAgendamento);

    private static string GerarPlaca()
    {
        var guid = Guid.NewGuid().ToString("N").ToUpperInvariant();

        var letras = guid
            .Where(char.IsLetter)
            .Take(3)
            .ToArray();

        var numeros = guid
            .Where(char.IsDigit)
            .Take(3)
            .ToArray();

        if (letras.Length < 3 || numeros.Length < 3)
            return "ABC1D23";

        return $"{letras[0]}{letras[1]}{letras[2]}1{letras[0]}{numeros[0]}{numeros[1]}";
    }
}