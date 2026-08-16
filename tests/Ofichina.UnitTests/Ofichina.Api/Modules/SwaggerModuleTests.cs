using Microsoft.Extensions.DependencyInjection;
using Ofichina.Api.Modules;

namespace Ofichina.UnitTests.Ofichina.Api.Modules;

public sealed class SwaggerModuleTests
{
    [Fact]
    public void AddSwaggerModule_Deve_Registrar_Servicos_Swagger()
    {
        var services = new ServiceCollection();

        services.AddSwaggerModule();

        Assert.Contains(services, x => x.ServiceType.FullName?.Contains("SwaggerGen") == true);
        Assert.Contains(services, x => x.ServiceType == typeof(Microsoft.AspNetCore.Mvc.ApiExplorer.IApiDescriptionGroupCollectionProvider));
    }
}