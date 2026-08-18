using Ofichina.Infrastructure.Persistence;

namespace Ofichina.UnitTests.Infrastructure;

public sealed class ApplicationDbContextFactoryTests
{
    [Fact]
    public void CreateDbContext_Deve_Lancar_Excecao_Quando_ConnectionString_Nao_Estiver_Configurada()
    {
        const string envName = "ConnectionStrings__DefaultConnection";
        var previousValue = Environment.GetEnvironmentVariable(envName);
        var previousDirectory = Directory.GetCurrentDirectory();
        var tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempDirectory);

        try
        {
            Directory.SetCurrentDirectory(tempDirectory);
            Environment.SetEnvironmentVariable(envName, null);

            var factory = new ApplicationDbContextFactory();

            var exception = Assert.Throws<InvalidOperationException>(() => factory.CreateDbContext([]));

            Assert.Equal("ConnectionStrings__DefaultConnection não configurada.", exception.Message);
        }
        finally
        {
            Directory.SetCurrentDirectory(previousDirectory);
            Directory.Delete(tempDirectory, recursive: true);
            Environment.SetEnvironmentVariable(envName, previousValue);
        }
    }

    [Fact]
    public void CreateDbContext_Deve_Criar_Contexto_Com_Provider_SqlServer()
    {
        const string envName = "ConnectionStrings__DefaultConnection";
        var previousValue = Environment.GetEnvironmentVariable(envName);
        var previousDirectory = Directory.GetCurrentDirectory();
        var tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempDirectory);

        try
        {
            Directory.SetCurrentDirectory(tempDirectory);
            Environment.SetEnvironmentVariable(envName, "Server=(localdb)\\mssqllocaldb;Database=Ofichinna_Testes;Trusted_Connection=True;TrustServerCertificate=True;");

            var factory = new ApplicationDbContextFactory();
            using var context = factory.CreateDbContext([]);

            Assert.NotNull(context);
            Assert.Equal("Microsoft.EntityFrameworkCore.SqlServer", context.Database.ProviderName);
        }
        finally
        {
            Directory.SetCurrentDirectory(previousDirectory);
            Directory.Delete(tempDirectory, recursive: true);
            Environment.SetEnvironmentVariable(envName, previousValue);
        }
    }
}