using Microsoft.EntityFrameworkCore;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Enums;
using Ofichina.Infrastructure.Persistence;
using Ofichina.Infrastructure.Repositories;
using System.Reflection;

namespace Ofichina.UnitTests.Infrastructure.Repositories;

public sealed class OrcamentoRepositoryTests
{
    [Fact]
    public async Task GetPagedAsync_Deve_Carregar_Itens_Servicos_E_Pecas_Para_Calcular_Totais()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new ApplicationDbContext(options);

        var servico = new Servico("Troca de óleo", null, 100m);
        var peca = new Peca("Filtro de óleo", null, "FILTRO-001", 25m, 10);
        var orcamento = new Orcamento(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(10),
            10m,
            "Listagem de orçamento");

        var itemServico = orcamento.AdicionarServico(servico.Id, peca.Id, 2, StatusOrcamento.Criado);
        DefinirPropriedade(itemServico, nameof(ItemServico.Servico), servico);
        DefinirPropriedade(itemServico, nameof(ItemServico.Peca), peca);

        orcamento.IniciarDiagnostico();
        orcamento.FinalizarDiagnostico();
        orcamento.AtualizarDesconto(10m);

        context.AddRange(servico, peca, orcamento, itemServico);
        await context.SaveChangesAsync();

        var repository = new OrcamentoRepository(context);

        var resultado = await repository.GetPagedAsync(new Pagination(1, 10));

        var orcamentoListagem = Assert.Single(resultado.Items);
        Assert.Equal(150m, orcamentoListagem.ValorTotal);
        Assert.Equal(135m, orcamentoListagem.ValorTotalDesconto);
    }

    private static void DefinirPropriedade<T>(T instancia, string propriedade, object? valor)
        where T : class
    {
        var property = typeof(T).GetProperty(propriedade, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        Assert.NotNull(property);
        property!.SetValue(instancia, valor);
    }
}