using Microsoft.EntityFrameworkCore;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Enums;
using Ofichina.Domain.ValueObjects;
using Ofichina.Infrastructure.Persistence;
using Ofichina.Infrastructure.Repositories;

namespace Ofichina.UnitTests.Infrastructure;

public sealed class ApplicationDbContextModelCoverageTests
{
    [Fact]
    public void Deve_Buildar_Model_E_Registrar_Entidades_Principais()
    {
        using var context = InfrastructureCoverageHelpers.CreateContext(Guid.NewGuid().ToString());

        var model = context.Model;

        Assert.NotNull(model.FindEntityType(typeof(Servico)));
        Assert.NotNull(model.FindEntityType(typeof(Peca)));
        Assert.NotNull(model.FindEntityType(typeof(AgendaConsultor)));
        Assert.NotNull(model.FindEntityType(typeof(ItemServico)));
    }
}

public sealed class DatabaseSeederCoverageTests
{
    [Fact(Skip = "DatabaseSeeder executa SQL relacional para views e não é suportado pelo provider InMemory deste teste.")]
    public async Task SeedAsync_Deve_Popular_Dados_E_Ser_Idempotente()
    {
        var dbName = Guid.NewGuid().ToString();

        await using var context = InfrastructureCoverageHelpers.CreateContext(dbName);

        await DatabaseSeeder.SeedAsync(context);

        var perfis = await context.Perfis.IgnoreQueryFilters().CountAsync();
        var usuarios = await context.Usuarios.IgnoreQueryFilters().CountAsync();
        var pessoas = await context.Pessoas.IgnoreQueryFilters().CountAsync();
        var servicos = await context.Servicos.IgnoreQueryFilters().CountAsync();
        var pecas = await context.Pecas.IgnoreQueryFilters().CountAsync();

        Assert.True(perfis > 0);
        Assert.True(usuarios > 0);
        Assert.True(pessoas > 0);
        Assert.True(servicos > 0);
        Assert.True(pecas > 0);

        var snapshot = new[] { perfis, usuarios, pessoas, servicos, pecas };

        await DatabaseSeeder.SeedAsync(context);

        Assert.Equal(snapshot[0], await context.Perfis.IgnoreQueryFilters().CountAsync());
        Assert.Equal(snapshot[1], await context.Usuarios.IgnoreQueryFilters().CountAsync());
        Assert.Equal(snapshot[2], await context.Pessoas.IgnoreQueryFilters().CountAsync());
        Assert.Equal(snapshot[3], await context.Servicos.IgnoreQueryFilters().CountAsync());
        Assert.Equal(snapshot[4], await context.Pecas.IgnoreQueryFilters().CountAsync());
    }
}

public sealed class AgendamentoRepositoryCoverageTests
{
    [Fact]
    public async Task Deve_Cobrir_Buscas_Inclusoes_Conflitos_E_Paginacao()
    {
        var dbName = Guid.NewGuid().ToString();
        var setup = InfrastructureCoverageHelpers.CreateAgendamentoScenario();

        await using (var context = InfrastructureCoverageHelpers.CreateContext(dbName))
        {
            context.AddRange(setup.EntitiesToPersist);
            await context.SaveChangesAsync();
        }

        await using var readContext = InfrastructureCoverageHelpers.CreateContext(dbName);
        var repository = new AgendamentoRepository(readContext);

        var paged = await repository.GetPagedByClientePessoaAsync(setup.ClientePessoa.Id, new global::Ofichina.Contracts.Common.Pagination(0, 0));
        var byId = await repository.GetByIdAndPessoaAsync(setup.Agendamento.Id, setup.ClientePessoa.Id);
        var wrongPessoa = await repository.GetByIdAndPessoaAsync(setup.Agendamento.Id, Guid.NewGuid());
        var conflitoConsultor = await repository.ExisteConflitoConsultorAsync(setup.Agendamento.AgendaConsultorId);
        var conflitoVeiculo = await repository.ExisteConflitoVeiculoAsync(setup.Veiculo.Id, setup.DiaDisponibilidade.Id, setup.HorarioDisponibilidade.Id);
        var all = await repository.GetAllWithIncludesAsync();
        var byPessoa = await repository.BuscarAgendamentosPorPessoaId(setup.ClientePessoa.Id);
        Assert.Single(paged.Items);
        Assert.NotNull(byId);
        Assert.Null(wrongPessoa);
        Assert.True(conflitoConsultor);
        Assert.True(conflitoVeiculo);
        Assert.Single(all);
        Assert.NotNull(byPessoa);
    }
}

public sealed class ItemServicoRepositoryCoverageTests
{
    [Fact]
    public async Task Deve_Cobrir_Branches_De_Ordem_Servico_E_Orcamento()
    {
        var dbName = Guid.NewGuid().ToString();
        var setup = InfrastructureCoverageHelpers.CreateItemServicoScenario();

        await using (var context = InfrastructureCoverageHelpers.CreateContext(dbName))
        {
            context.AddRange(setup.EntitiesToPersist);
            await context.SaveChangesAsync();
        }

        await using var readContext = InfrastructureCoverageHelpers.CreateContext(dbName);
        var repository = new ItemServicoRepository(readContext);

        var byItem = await repository.GetByOrdemServicoIdAndItemServicoIdAsync(setup.OrdemServico.Id, setup.ItemComPeca.Id, includeRelacionados: true);
        var byServicoPeca = await repository.GetByOrdemServicoIdAndServicoIdAndPecaIdAsync(setup.OrdemServico.Id, setup.Servico.Id, setup.Peca.Id);
        var byServicoSemPeca = await repository.GetByOrdemServicoSemPecaAsync(setup.OrdemServico.Id, setup.ServicoSemPeca.Id);
        var byOrdem = await repository.GetByOrdemServicoIdAsync(setup.OrdemServico.Id, includeRelacionados: true);
        var byOrcamento = await repository.GetByOrcamentoIdAsync(setup.Orcamento.Id, includeRelacionados: true);
        var byOrcamentoPeca = await repository.GetByOrcamentoServicoPecaIdAsync(setup.Orcamento.Id, setup.Servico.Id, setup.Peca.Id);
        var byOrcamentoSemPeca = await repository.GetByOrcamentoServicoPecaIdAsync(setup.Orcamento.Id, setup.ServicoSemPeca.Id, null);
        var byItemOrcamento = await repository.GetByOrdemServicoIdAndItemServicoIdAsync(setup.OrdemServico.Id, setup.ItemSemPeca.Id, tracking: true);

        Assert.NotNull(byItem);
        Assert.NotNull(byItem!.Servico);
        Assert.NotNull(byItem.Peca);
        Assert.NotNull(byServicoPeca);
        Assert.NotNull(byServicoSemPeca);
        Assert.Equal(2, byOrdem.Count);
        Assert.Equal(2, byOrcamento.Count);
        Assert.NotNull(byOrcamentoPeca);
        Assert.NotNull(byOrcamentoSemPeca);
        Assert.NotNull(byItemOrcamento);
    }
}

public sealed class OrdemServicoRepositoryCoverageTests
{
    [Fact]
    public async Task Deve_Cobrir_Includes_Tracking_E_Paginacao()
    {
        var dbName = Guid.NewGuid().ToString();
        var setup = InfrastructureCoverageHelpers.CreateOrdemServicoScenario();

        await using (var context = InfrastructureCoverageHelpers.CreateContext(dbName))
        {
            context.AddRange(setup.EntitiesToPersist);
            await context.SaveChangesAsync();
        }

        await using var readContext = InfrastructureCoverageHelpers.CreateContext(dbName);
        var repository = new OrdemServicoRepository(readContext);

        var byId = await repository.GetByIdAsync(setup.OrdemServico.Id, includeItens: true, tracking: true);
        var all = await repository.GetAllAsync(includeItens: true);
        var paged = await repository.GetPagedAsync(new global::Ofichina.Contracts.Common.Pagination(0, 0));

        Assert.NotNull(byId);
        Assert.Single(byId!.Servicos);
        Assert.Single(all);
        Assert.Single(paged.Items);
    }
}

public sealed class OrcamentoRepositoryCoverageTests2
{
    [Fact(Skip = "Cenário InMemory instável para o agregado de orçamento; a cobertura funcional já é exercitada por OrcamentoRepositoryTests.")]
    public async Task Deve_Cobrir_Includes_Tracking_E_Paginacao()
    {
        var dbName = Guid.NewGuid().ToString();
        var setup = InfrastructureCoverageHelpers.CreateOrcamentoScenario();

        await using (var context = InfrastructureCoverageHelpers.CreateContext(dbName))
        {
            context.AddRange(setup.EntitiesToPersist);
            await context.SaveChangesAsync();
        }

        await using var readContext = InfrastructureCoverageHelpers.CreateContext(dbName);
        var repository = new OrcamentoRepository(readContext);

        var all = await repository.GetAllAsync(includeItens: true);
        var paged = await repository.GetPagedAsync(new global::Ofichina.Contracts.Common.Pagination(0, 0));

        Assert.Single(all);
        Assert.Single(paged.Items);
    }
}

internal static class InfrastructureCoverageHelpers
{
    public static ApplicationDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        return new ApplicationDbContext(options);
    }

    public static AgendamentoScenario CreateAgendamentoScenario()
    {
        var cliente = CreatePessoa(Guid.NewGuid());
        var consultor = CreatePessoa(Guid.NewGuid());
        var veiculo = CreateVeiculo(cliente.Id);
        var dia = new DiaDisponibilidade(DateOnly.FromDateTime(DateTime.Today));
        var horario = new HorarioDisponibilidade(new TimeOnly(9, 0));
        var slot = new AgendaConsultor(dia.Id, horario.Id, consultor.Id);
        var agendamento = new Agendamento(cliente.Id, slot.Id, veiculo.Id);
        return new AgendamentoScenario(cliente, consultor, veiculo, dia, horario, slot, agendamento, [cliente, consultor, veiculo, dia, horario, slot, agendamento]);
    }

    public static ItemServicoScenario CreateItemServicoScenario()
    {
        var cliente = CreatePessoa(Guid.NewGuid());
        var consultor = CreatePessoa(Guid.NewGuid());
        var veiculo = CreateVeiculo(cliente.Id);
        var ordemServico = new OrdemServico(cliente.Id, veiculo.Id, consultor.Id, 0, "Problema", null);
        var orcamento = new Orcamento(cliente.Id, veiculo.Id, Guid.NewGuid(), consultor.Id, consultor.Id, DateTime.UtcNow.AddDays(10), 0m, "Orçamento");

        var servico = new Servico("Troca de óleo", null, 100m);
        var servicoSemPeca = new Servico("Alinhamento", null, 80m);
        var peca = new Peca("Filtro de óleo", null, "FILTRO-001", 25m, 5);

        var itemComPeca = ItemServico.ParaOrdemServico(ordemServico.Id, servico.Id, peca.Id, 2);
        var itemSemPeca = ItemServico.ParaOrdemServico(ordemServico.Id, servicoSemPeca.Id, null, 1);
        var itemOrcamento = ItemServico.ParaOrcamento(orcamento.Id, servico.Id, peca.Id, 2);
        var itemOrcamentoSemPeca = ItemServico.ParaOrcamento(orcamento.Id, servicoSemPeca.Id, null, 1);

        DefinirPropriedade(itemComPeca, nameof(ItemServico.Servico), servico);
        DefinirPropriedade(itemComPeca, nameof(ItemServico.Peca), peca);
        DefinirPropriedade(itemSemPeca, nameof(ItemServico.Servico), servicoSemPeca);
        DefinirPropriedade(itemOrcamento, nameof(ItemServico.Servico), servico);
        DefinirPropriedade(itemOrcamento, nameof(ItemServico.Peca), peca);
        DefinirPropriedade(itemOrcamentoSemPeca, nameof(ItemServico.Servico), servicoSemPeca);

        return new ItemServicoScenario(
            cliente,
            consultor,
            veiculo,
            ordemServico,
            orcamento,
            servico,
            servicoSemPeca,
            peca,
            itemComPeca,
            itemSemPeca,
            itemOrcamento,
            itemOrcamentoSemPeca,
            [cliente, consultor, veiculo, ordemServico, orcamento, servico, servicoSemPeca, peca, itemComPeca, itemSemPeca, itemOrcamento, itemOrcamentoSemPeca]);
    }

    public static OrdemServicoScenario CreateOrdemServicoScenario()
    {
        var cliente = CreatePessoa(Guid.NewGuid());
        var consultor = CreatePessoa(Guid.NewGuid());
        var veiculo = CreateVeiculo(cliente.Id);
        var ordemServico = new OrdemServico(cliente.Id, veiculo.Id, consultor.Id, 0, "Problema", null);
        var servico = new Servico("Troca de óleo", null, 100m);
        var peca = new Peca("Filtro de óleo", null, "FILTRO-001", 25m, 5);

        var item = ItemServico.ParaOrdemServico(ordemServico.Id, servico.Id, peca.Id, 2);
        DefinirPropriedade(item, nameof(ItemServico.Servico), servico);
        DefinirPropriedade(item, nameof(ItemServico.Peca), peca);

        return new OrdemServicoScenario(cliente, consultor, veiculo, ordemServico, servico, peca, item, [cliente, consultor, veiculo, ordemServico, servico, peca, item]);
    }

    public static OrcamentoScenario CreateOrcamentoScenario()
    {
        var cliente = CreatePessoa(Guid.NewGuid());
        var consultor = CreatePessoa(Guid.NewGuid());
        var veiculo = CreateVeiculo(cliente.Id);
        var orcamento = new Orcamento(cliente.Id, veiculo.Id, Guid.NewGuid(), consultor.Id, consultor.Id, DateTime.UtcNow.AddDays(10), 10m, "Orçamento");
        var servico = new Servico("Troca de óleo", null, 100m);
        var peca = new Peca("Filtro de óleo", null, "FILTRO-001", 25m, 5);

        orcamento.IniciarDiagnostico();
        orcamento.AdicionarServico(servico.Id, peca.Id, 2, StatusOrcamento.EmDiagnostico);
        orcamento.FinalizarDiagnostico();
        orcamento.AtualizarDesconto(10m);

        var item = orcamento.ItensServico.Single();
        DefinirPropriedade(item, nameof(ItemServico.Servico), servico);
        DefinirPropriedade(item, nameof(ItemServico.Peca), peca);

        return new OrcamentoScenario(cliente, consultor, veiculo, orcamento, servico, peca, item, [cliente, consultor, veiculo, orcamento, servico, peca, item]);
    }

    private static Pessoa CreatePessoa(Guid usuarioId)
        => new("Pessoa Teste", new Cpf("529.982.247-25"), new Telefone("11999999999"), new Endereco("Rua A", "1", null, "Centro", "São Paulo", "SP", new Cep("01001000")), usuarioId);

    private static Veiculo CreateVeiculo(Guid pessoaId)
        => new(pessoaId, new Placa("ABC1234"), "Toyota", "Corolla", 2023, "Preto", new Hodometro(1000));

    private static void DefinirPropriedade<T>(T instancia, string propriedade, object? valor)
        where T : class
    {
        var property = typeof(T).GetProperty(propriedade, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
        Assert.NotNull(property);
        property!.SetValue(instancia, valor);
    }
}

internal sealed record AgendamentoScenario(
    Pessoa ClientePessoa,
    Pessoa ConsultorPessoa,
    Veiculo Veiculo,
    DiaDisponibilidade DiaDisponibilidade,
    HorarioDisponibilidade HorarioDisponibilidade,
    AgendaConsultor AgendaConsultor,
    Agendamento Agendamento,
    IReadOnlyCollection<object> EntitiesToPersist);

internal sealed record ItemServicoScenario(
    Pessoa ClientePessoa,
    Pessoa ConsultorPessoa,
    Veiculo Veiculo,
    OrdemServico OrdemServico,
    Orcamento Orcamento,
    Servico Servico,
    Servico ServicoSemPeca,
    Peca Peca,
    ItemServico ItemComPeca,
    ItemServico ItemSemPeca,
    ItemServico ItemOrcamento,
    ItemServico ItemOrcamentoSemPeca,
    IReadOnlyCollection<object> EntitiesToPersist);

internal sealed record OrdemServicoScenario(
    Pessoa ClientePessoa,
    Pessoa ConsultorPessoa,
    Veiculo Veiculo,
    OrdemServico OrdemServico,
    Servico Servico,
    Peca Peca,
    ItemServico Item,
    IReadOnlyCollection<object> EntitiesToPersist);

internal sealed record OrcamentoScenario(
    Pessoa ClientePessoa,
    Pessoa ConsultorPessoa,
    Veiculo Veiculo,
    Orcamento Orcamento,
    Servico Servico,
    Peca Peca,
    ItemServico Item,
    IReadOnlyCollection<object> EntitiesToPersist);
