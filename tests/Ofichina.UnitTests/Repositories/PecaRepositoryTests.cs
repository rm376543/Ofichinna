using Microsoft.EntityFrameworkCore;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Entities;
using Ofichina.Infrastructure.Persistence;
using Ofichina.Infrastructure.Repositories;

namespace Ofichina.UnitTests.Repositories;

public sealed class PecaRepositoryTests
{
    [Fact]
    public void Constructor_ContextoValido_Deve_CriarInstancia()
    {
        using var context = CriarContexto(Guid.NewGuid().ToString());

        var repository = new PecaRepository(context);

        Assert.NotNull(repository);
    }

    [Fact]
    public async Task GetPagedAsync_PaginationNula_Deve_LancarArgumentNullException()
    {
        await using var context = CriarContexto(Guid.NewGuid().ToString());
        var repository = new PecaRepository(context);

        await Assert.ThrowsAsync<ArgumentNullException>(() => repository.GetPagedAsync(null!));
    }

    [Fact]
    public async Task GetPagedAsync_PaginacaoInvalida_Deve_Normalizar_E_Ordenar_E_Excluir_Pecas_Desativadas()
    {
        var dbName = Guid.NewGuid().ToString();
        var pecaB = new Peca("Filtro de ar", null, "FIL-002", 20m, 5);
        var pecaA = new Peca("Bateria", null, "BAT-001", 500m, 2);
        var pecaExcluida = new Peca("Alternador", null, "ALT-003", 700m, 1);
        pecaExcluida.Excluir();

        await using (var context = CriarContexto(dbName))
        {
            context.AddRange(pecaB, pecaA, pecaExcluida);
            await context.SaveChangesAsync();
        }

        await using var readContext = CriarContexto(dbName);
        var repository = new PecaRepository(readContext);

        var resultado = await repository.GetPagedAsync(new Pagination(0, 0));

        Assert.Equal(2, resultado.TotalCount);
        Assert.Equal(1, resultado.PageNumber);
        Assert.Equal(10, resultado.PageSize);
        Assert.Collection(
            resultado.Items,
            item => Assert.Equal("Bateria", item.Nome),
            item => Assert.Equal("Filtro de ar", item.Nome));
    }

    [Fact]
    public async Task GetPagedAsync_PaginaDois_Deve_Retornar_Somente_Os_Itens_Da_Pagina()
    {
        var dbName = Guid.NewGuid().ToString();
        var peca1 = new Peca("Bateria", null, "BAT-001", 500m, 2);
        var peca2 = new Peca("Filtro de ar", null, "FIL-002", 20m, 5);
        var peca3 = new Peca("Alternador", null, "ALT-003", 700m, 1);

        await using (var context = CriarContexto(dbName))
        {
            context.AddRange(peca1, peca2, peca3);
            await context.SaveChangesAsync();
        }

        await using var readContext = CriarContexto(dbName);
        var repository = new PecaRepository(readContext);

        var resultado = await repository.GetPagedAsync(new Pagination(2, 2));

        Assert.Equal(3, resultado.TotalCount);
        Assert.Equal(2, resultado.PageNumber);
        Assert.Equal(2, resultado.PageSize);
        Assert.Single(resultado.Items);
        Assert.Equal("Filtro de ar", resultado.Items.Single().Nome);
    }

    private static ApplicationDbContext CriarContexto(string dbName)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        return new ApplicationDbContext(options);
    }
}
