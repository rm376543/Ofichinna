using Microsoft.EntityFrameworkCore;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Entities;
using Ofichina.Infrastructure.Persistence;
using Ofichina.Infrastructure.Repositories;

namespace Ofichina.UnitTests.Infrastructure.Repositories;

public sealed class RepositoryTests
{
    [Fact]
    public async Task AddAsync_Deve_Lancar_Excecao_Quando_Entidade_For_Nula()
    {
        await using var context = CriarContexto(Guid.NewGuid().ToString());
        var repository = new Repository<Servico>(context);

        await Assert.ThrowsAsync<ArgumentNullException>(() => repository.AddAsync(null!));
    }

    [Fact]
    public async Task UpdateAsync_Deve_Lancar_Excecao_Quando_Entidade_For_Nula()
    {
        await using var context = CriarContexto(Guid.NewGuid().ToString());
        var repository = new Repository<Servico>(context);

        await Assert.ThrowsAsync<ArgumentNullException>(() => repository.UpdateAsync(null!));
    }

    [Fact]
    public async Task DeleteAsync_Deve_Lancar_Excecao_Quando_Entidade_For_Nula()
    {
        await using var context = CriarContexto(Guid.NewGuid().ToString());
        var repository = new Repository<Servico>(context);

        await Assert.ThrowsAsync<ArgumentNullException>(() => repository.DeleteAsync(null!));
    }

    [Fact]
    public async Task HardDeleteAsync_Deve_Lancar_Excecao_Quando_Entidade_For_Nula()
    {
        await using var context = CriarContexto(Guid.NewGuid().ToString());
        var repository = new Repository<Servico>(context);

        await Assert.ThrowsAsync<ArgumentNullException>(() => repository.HardDeleteAsync(null!));
    }

    [Fact]
    public async Task GetByIdAsync_Deve_Respeitar_Tracking()
    {
        var dbName = Guid.NewGuid().ToString();
        var servico = new Servico("Troca de óleo", null, 100m);

        await using (var context = CriarContexto(dbName))
        {
            context.Servicos.Add(servico);
            await context.SaveChangesAsync();
        }

        await using var readContext = CriarContexto(dbName);
        var repository = new Repository<Servico>(readContext);

        var semTracking = await repository.GetByIdAsync(servico.Id, tracking: false);
        var comTracking = await repository.GetByIdAsync(servico.Id, tracking: true);

        Assert.NotNull(semTracking);
        Assert.NotNull(comTracking);
        Assert.Equal(servico.Id, semTracking!.Id);
        Assert.Equal(servico.Id, comTracking!.Id);
    }

    [Fact]
    public async Task GetAllAsync_Deve_Retornar_Todas_As_Entidades_Ativas()
    {
        var dbName = Guid.NewGuid().ToString();
        var servicos = new[]
        {
            new Servico("Troca de óleo", null, 100m),
            new Servico("Alinhamento", null, 90m)
        };

        await using (var context = CriarContexto(dbName))
        {
            context.Servicos.AddRange(servicos);
            await context.SaveChangesAsync();
        }

        await using var readContext = CriarContexto(dbName);
        var repository = new Repository<Servico>(readContext);

        var resultado = await repository.GetAllAsync();

        Assert.Equal(2, resultado.Count());
    }

    [Fact]
    public async Task GetPagedAsync_Deve_Validar_Paginacao_E_Normalizar_Valores()
    {
        var dbName = Guid.NewGuid().ToString();
        var servicos = new[]
        {
            new Servico("Serviço 1", null, 10m),
            new Servico("Serviço 2", null, 20m),
            new Servico("Serviço 3", null, 30m)
        };

        await using (var context = CriarContexto(dbName))
        {
            context.Servicos.AddRange(servicos);
            await context.SaveChangesAsync();
        }

        await using var readContext = CriarContexto(dbName);
        var repository = new Repository<Servico>(readContext);

        var resultado = await repository.GetPagedAsync(new Pagination(2, 2));

        Assert.Equal(3, resultado.TotalCount);
        Assert.Equal(2, resultado.PageNumber);
        Assert.Equal(2, resultado.PageSize);
        Assert.Single(resultado.Items);
        Assert.Contains(resultado.Items.First().Nome, new[] { "Serviço 1", "Serviço 2", "Serviço 3" });
    }

    [Fact]
    public async Task GetPagedAsync_Deve_Lancar_Excecao_Quando_Pagination_For_Nula()
    {
        await using var context = CriarContexto(Guid.NewGuid().ToString());
        var repository = new Repository<Servico>(context);

        await Assert.ThrowsAsync<ArgumentNullException>(() => repository.GetPagedAsync(null!));
    }

    [Fact]
    public async Task UpdateAsync_Deve_Atualizar_DataDeModificacao()
    {
        var dbName = Guid.NewGuid().ToString();
        var servico = new Servico("Troca de óleo", null, 100m);

        await using (var context = CriarContexto(dbName))
        {
            context.Servicos.Add(servico);
            await context.SaveChangesAsync();
        }

        await using var updateContext = CriarContexto(dbName);
        var repository = new Repository<Servico>(updateContext);

        var encontrado = await repository.GetByIdAsync(servico.Id, tracking: true);
        Assert.NotNull(encontrado);

        var antes = encontrado!.UpdatedAt;
        await repository.UpdateAsync(encontrado);
        await updateContext.SaveChangesAsync();

        Assert.NotNull(encontrado.UpdatedAt);
        Assert.NotEqual(antes, encontrado.UpdatedAt);
    }

    [Fact]
    public async Task DeleteAsync_Deve_Realizar_SoftDelete()
    {
        var dbName = Guid.NewGuid().ToString();
        var servico = new Servico("Troca de óleo", null, 100m);

        await using (var context = CriarContexto(dbName))
        {
            context.Servicos.Add(servico);
            await context.SaveChangesAsync();
        }

        await using var deleteContext = CriarContexto(dbName);
        var repository = new Repository<Servico>(deleteContext);

        var encontrado = await repository.GetByIdAsync(servico.Id, tracking: true);
        Assert.NotNull(encontrado);

        await repository.DeleteAsync(encontrado!);
        await deleteContext.SaveChangesAsync();

        await using var readContext = CriarContexto(dbName);
        var excluido = await readContext.Servicos.FirstOrDefaultAsync(x => x.Id == servico.Id);

        Assert.Null(excluido);
    }

    [Fact]
    public async Task HardDeleteAsync_Deve_Remover_Entidade_Do_Banco()
    {
        var dbName = Guid.NewGuid().ToString();
        var servico = new Servico("Troca de óleo", null, 100m);

        await using (var context = CriarContexto(dbName))
        {
            context.Servicos.Add(servico);
            await context.SaveChangesAsync();
        }

        await using var deleteContext = CriarContexto(dbName);
        var repository = new Repository<Servico>(deleteContext);

        var encontrado = await repository.GetByIdAsync(servico.Id, tracking: true);
        Assert.NotNull(encontrado);

        await repository.HardDeleteAsync(encontrado!);
        await deleteContext.SaveChangesAsync();

        await using var readContext = CriarContexto(dbName);
        var existente = await readContext.Servicos.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == servico.Id);

        Assert.Null(existente);
    }

    private static ApplicationDbContext CriarContexto(string dbName)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        return new ApplicationDbContext(options);
    }
}