using Microsoft.EntityFrameworkCore;
using Ofichina.Domain.Entities;
using Ofichina.Infrastructure.Persistence;
using Ofichina.Infrastructure.Repositories;

namespace Ofichina.UnitTests.Infrastructure.Repositories;

public sealed class DiaDisponibilidadeRepositoryTests
{
    [Fact]
    public void DiaDisponibilidadeRepository_ComContextoValido_Deve_CriarInstancia()
    {
        // Arrange
        using var context = CriarContexto(Guid.NewGuid().ToString());

        // Act
        var repository = new DiaDisponibilidadeRepository(context);

        // Assert
        Assert.NotNull(repository);
    }

    [Fact]
    public async Task GetDiasDisponiveisAsync_ComDiasDesordenados_Deve_Retornar_Ordenado_Crescente_E_Sem_Tracking()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        var diaMaisRecente = new DiaDisponibilidade(new DateOnly(2026, 1, 15));
        var diaMaisAntigo = new DiaDisponibilidade(new DateOnly(2026, 1, 10));
        var diaIntermediario = new DiaDisponibilidade(new DateOnly(2026, 1, 12));

        await using (var seedContext = CriarContexto(dbName))
        {
            seedContext.DiasDisponibilidade.AddRange(diaMaisRecente, diaMaisAntigo, diaIntermediario);
            await seedContext.SaveChangesAsync();
        }

        await using var readContext = CriarContexto(dbName);
        var repository = new DiaDisponibilidadeRepository(readContext);

        // Act
        var resultado = await repository.GetDiasDisponiveisAsync();

        // Assert
        Assert.Equal(
            new[] { diaMaisAntigo.Data, diaIntermediario.Data, diaMaisRecente.Data },
            resultado.Select(x => x.Data));
        Assert.Empty(readContext.ChangeTracker.Entries<DiaDisponibilidade>());
    }

    private static ApplicationDbContext CriarContexto(string dbName)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        return new ApplicationDbContext(options);
    }
}
