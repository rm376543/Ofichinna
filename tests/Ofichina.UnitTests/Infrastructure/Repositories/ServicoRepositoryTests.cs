using Microsoft.EntityFrameworkCore;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Entities;
using Ofichina.Infrastructure.Persistence;
using Ofichina.Infrastructure.Repositories;

namespace Ofichina.UnitTests.Infrastructure.Repositories;

public sealed class ServicoRepositoryTests
{
    [Fact]
    public async Task GetByIdAsync_Deve_Respeitar_SoftDelete_E_Tracking()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var servicoAtivo = new Servico("Troca de óleo", null, 100m);
        var servicoExcluido = new Servico("Alinhamento", null, 90m);
        servicoExcluido.Desativar();

        await using (var context = new ApplicationDbContext(options))
        {
            context.AddRange(servicoAtivo, servicoExcluido);
            await context.SaveChangesAsync();
        }

        await using var readContext = new ApplicationDbContext(options);
        var repository = new ServicoRepository(readContext);

        var encontrado = await repository.GetByIdAsync(servicoAtivo.Id);
        var excluido = await repository.GetByIdAsync(servicoExcluido.Id);

        Assert.NotNull(encontrado);
        Assert.Equal(servicoAtivo.Id, encontrado!.Id);
        Assert.Null(excluido);
    }

    [Fact]
    public async Task GetAllAsync_Deve_Retornar_Somente_Servicos_Ativos()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var servicoAtivo = new Servico("Troca de óleo", null, 100m);
        var servicoExcluido = new Servico("Alinhamento", null, 90m);
        servicoExcluido.Desativar();

        await using (var context = new ApplicationDbContext(options))
        {
            context.AddRange(servicoAtivo, servicoExcluido);
            await context.SaveChangesAsync();
        }

        await using var readContext = new ApplicationDbContext(options);
        var repository = new ServicoRepository(readContext);

        var resultado = await repository.GetAllAsync();

        Assert.Single(resultado);
        Assert.Contains(resultado, x => x.Id == servicoAtivo.Id);
    }

    [Fact]
    public async Task GetPagedAsync_Deve_Ordenar_E_Paginar_Servicos()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var servicos = new[]
        {
            new Servico("Serviço 1", null, 10m),
            new Servico("Serviço 2", null, 20m),
            new Servico("Serviço 3", null, 30m)
        };

        await using (var context = new ApplicationDbContext(options))
        {
            context.AddRange(servicos);
            await context.SaveChangesAsync();
        }

        await using var readContext = new ApplicationDbContext(options);
        var repository = new ServicoRepository(readContext);

        var resultado = await repository.GetPagedAsync(new Pagination(2, 2));

        Assert.Equal(3, resultado.TotalCount);
        Assert.Equal(2, resultado.PageNumber);
        Assert.Equal(2, resultado.PageSize);
        Assert.Single(resultado.Items);
        Assert.Equal("Serviço 3", resultado.Items.First().Nome);
    }
}