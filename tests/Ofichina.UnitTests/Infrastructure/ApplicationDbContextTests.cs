using Microsoft.EntityFrameworkCore;
using Ofichina.Domain.Entities;
using Ofichina.Infrastructure.Persistence;

namespace Ofichina.UnitTests.Infrastructure;

public sealed class ApplicationDbContextTests
{
    [Fact]
    public async Task Deve_Aplicar_QueryFilter_De_SoftDelete_Para_Entidades_Do_Dominio()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var ativo = new Servico("Troca de óleo", null, 100m);
        var excluido = new Servico("Alinhamento", null, 90m);
        excluido.Desativar();

        await using (var context = new ApplicationDbContext(options))
        {
            context.Servicos.AddRange(ativo, excluido);
            await context.SaveChangesAsync();
        }

        await using var readContext = new ApplicationDbContext(options);

        var servicos = await readContext.Servicos.ToListAsync();

        Assert.Single(servicos);
        Assert.Equal(ativo.Id, servicos.Single().Id);
    }
}