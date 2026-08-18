using Microsoft.EntityFrameworkCore;
using Ofichina.Infrastructure.Persistence;

namespace Ofichina.UnitTests.Infrastructure.Persistence;

public sealed class DatabaseSeederTests
{
    [Fact]
    public async Task SeedAsync_Quando_Usa_Provider_InMemory_Deve_Popular_Dados_E_Lancar_InvalidOperationException_Ao_Criar_View()
    {
        // Arrange
        await using var context = TestContextFactory.CreateContext();

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => DatabaseSeeder.SeedAsync(context));

        // Assert
        Assert.Equal(
            "Erro ao popular banco de dados inicial: Verifique se a migration foi executada corretamente.",
            exception.Message);
        Assert.IsType<InvalidOperationException>(exception.InnerException);

        Assert.True(await context.Perfis.IgnoreQueryFilters().AnyAsync());
        Assert.True(await context.Usuarios.IgnoreQueryFilters().AnyAsync());
        Assert.True(await context.Pessoas.IgnoreQueryFilters().AnyAsync());
        Assert.True(await context.Veiculos.IgnoreQueryFilters().AnyAsync());
        Assert.True(await context.Servicos.IgnoreQueryFilters().AnyAsync());
        Assert.True(await context.Pecas.IgnoreQueryFilters().AnyAsync());
        Assert.True(await context.Permissoes.IgnoreQueryFilters().AnyAsync());
        Assert.True(await context.DiasDisponibilidade.IgnoreQueryFilters().AnyAsync());
        Assert.True(await context.HorariosDisponibilidade.IgnoreQueryFilters().AnyAsync());
        Assert.True(await context.PerfisPermissoes.IgnoreQueryFilters().AnyAsync());
        Assert.True(await context.DiasHorariosDisponibilidade.IgnoreQueryFilters().AnyAsync());
        Assert.True(await context.HorariosConsultores.IgnoreQueryFilters().AnyAsync());
        Assert.True(await context.HorariosConsultorDisponibilidade.IgnoreQueryFilters().AnyAsync());
        Assert.True(await context.Agendamentos.IgnoreQueryFilters().AnyAsync());
    }

    [Fact]
    public async Task SeedAsync_Quando_Executado_Duas_Vezes_Deve_Preservar_As_Quantidades_Se_Retornar_Erro()
    {
        // Arrange
        await using var context = TestContextFactory.CreateContext();

        // Act
        await Assert.ThrowsAsync<InvalidOperationException>(() => DatabaseSeeder.SeedAsync(context));
        var snapshot = await TestContextFactory.GetCountsAsync(context);

        await Assert.ThrowsAsync<InvalidOperationException>(() => DatabaseSeeder.SeedAsync(context));
        var afterSecondRun = await TestContextFactory.GetCountsAsync(context);

        // Assert
        Assert.Equal(snapshot, afterSecondRun);
    }

    [Fact]
    public async Task SeedAsync_Quando_Usa_SQLServer_Deve_Popular_Dados_E_Ser_Idempotente()
    {
        var databaseName = $"Ofichina_DatabaseSeederTests_{Guid.NewGuid():N}";
        await using var context = TestContextFactory.CreateRelationalContext(databaseName);

        try
        {
            await context.Database.MigrateAsync();

            await DatabaseSeeder.SeedAsync(context);

            var snapshot = await TestContextFactory.GetCountsAsync(context);

            Assert.True(snapshot.Perfis > 0);
            Assert.True(snapshot.Usuarios > 0);
            Assert.True(snapshot.Pessoas > 0);
            Assert.True(snapshot.Veiculos > 0);
            Assert.True(snapshot.Servicos > 0);
            Assert.True(snapshot.Pecas > 0);
            Assert.True(snapshot.Permissoes > 0);
            Assert.True(snapshot.DiasDisponibilidade > 0);
            Assert.True(snapshot.HorariosDisponibilidade > 0);
            Assert.True(snapshot.PerfisPermissoes > 0);
            Assert.True(snapshot.DiasHorariosDisponibilidade > 0);
            Assert.True(snapshot.HorariosConsultores > 0);
            Assert.True(snapshot.HorariosConsultorDisponibilidade > 0);
            Assert.True(snapshot.Agendamentos > 0);

            await DatabaseSeeder.SeedAsync(context);

            var afterSecondRun = await TestContextFactory.GetCountsAsync(context);

            Assert.Equal(snapshot, afterSecondRun);
        }
        finally
        {
            await context.Database.EnsureDeletedAsync();
        }
    }

    private sealed record SeedCounts(
        int Perfis,
        int Usuarios,
        int Pessoas,
        int Veiculos,
        int Servicos,
        int Pecas,
        int Permissoes,
        int DiasDisponibilidade,
        int HorariosDisponibilidade,
        int PerfisPermissoes,
        int DiasHorariosDisponibilidade,
        int HorariosConsultores,
        int HorariosConsultorDisponibilidade,
        int Agendamentos);

    private static class TestContextFactory
    {
        public static ApplicationDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        public static ApplicationDbContext CreateRelationalContext(string databaseName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer($"Server=(localdb)\\MSSQLLocalDB;Database={databaseName};Trusted_Connection=True;TrustServerCertificate=True;")
                .Options;

            return new ApplicationDbContext(options);
        }

        public static async Task<SeedCounts> GetCountsAsync(ApplicationDbContext context)
        {
            return new SeedCounts(
                await context.Perfis.IgnoreQueryFilters().CountAsync(),
                await context.Usuarios.IgnoreQueryFilters().CountAsync(),
                await context.Pessoas.IgnoreQueryFilters().CountAsync(),
                await context.Veiculos.IgnoreQueryFilters().CountAsync(),
                await context.Servicos.IgnoreQueryFilters().CountAsync(),
                await context.Pecas.IgnoreQueryFilters().CountAsync(),
                await context.Permissoes.IgnoreQueryFilters().CountAsync(),
                await context.DiasDisponibilidade.IgnoreQueryFilters().CountAsync(),
                await context.HorariosDisponibilidade.IgnoreQueryFilters().CountAsync(),
                await context.PerfisPermissoes.IgnoreQueryFilters().CountAsync(),
                await context.DiasHorariosDisponibilidade.IgnoreQueryFilters().CountAsync(),
                await context.HorariosConsultores.IgnoreQueryFilters().CountAsync(),
                await context.HorariosConsultorDisponibilidade.IgnoreQueryFilters().CountAsync(),
                await context.Agendamentos.IgnoreQueryFilters().CountAsync());
        }
    }
}
