using System.Reflection;
using NetArchTest.Rules;
using Ofichina.Api.Modules;
using Ofichina.Application.DependencyInjection;
using Ofichina.Authentication.DependencyInjection;
using Ofichina.Bootstrap;
using Ofichina.Contracts.Responses;
using Ofichina.Domain.Entities;
using Ofichina.Infrastructure.DependencyInjection;

namespace Ofichina.ArchitectureTests.Architecture;

public class ProjectDependencyTests
{
    [Fact]
    public void Domain_nao_deve_depender_de_outros_projetos()
        => AssertNoDependencies(
            typeof(Entity).Assembly,
            "Ofichina.Api",
            "Ofichina.Application",
            "Ofichina.Authentication",
            "Ofichina.Bootstrap",
            "Ofichina.Contracts",
            "Ofichina.Infrastructure");

    [Fact]
    public void Contracts_nao_deve_depender_de_outros_projetos()
        => AssertNoDependencies(
            typeof(ApiResponse).Assembly,
            "Ofichina.Api",
            "Ofichina.Application",
            "Ofichina.Authentication",
            "Ofichina.Bootstrap",
            "Ofichina.Domain",
            "Ofichina.Infrastructure");

    [Fact]
    public void Authentication_deve_depender_Application_Domain_e_Contracts()
        => AssertNoDependencies(
            typeof(AuthenticationModule).Assembly,
            "Ofichina.Api",
            "Ofichina.Bootstrap",
            "Ofichina.Infrastructure");

    [Fact]
    public void Application_deve_depender_de_Authentication_Domain_e_Contracts()
        => AssertNoDependencies(
            typeof(ApplicationModule).Assembly,
            "Ofichina.Api",
            "Ofichina.Bootstrap",
            "Ofichina.Infrastructure");

    [Fact]
    public void Infrastructure_deve_depender_de_Application_Authentication_e_Domain()
        => AssertNoDependencies(
            typeof(InfrastructureModule).Assembly,
            "Ofichina.Api",
            "Ofichina.Bootstrap",
            "Ofichina.Contracts");

    [Fact]
    public void Bootstrap_deve_depender_apenas_de_Application_Infrastructure_e_Authentication()
        => AssertNoDependencies(
            typeof(DependencyInjection).Assembly,
            "Ofichina.Api",
            "Ofichina.Contracts",
            "Ofichina.Domain");

    [Fact]
    public void Api_deve_depender_apenas_de_Application_Bootstrap_e_Contracts()
        => AssertNoDependencies(
            typeof(SwaggerModule).Assembly,
            "Ofichina.Domain",
            "Ofichina.Infrastructure",
            "Ofichina.Authentication");

    private static void AssertNoDependencies(Assembly assembly, params string[] forbiddenNamespaces)
    {
        foreach (var forbiddenNamespace in forbiddenNamespaces)
        {
            var result = Types.InAssembly(assembly)
                .ShouldNot()
                .HaveDependencyOn(forbiddenNamespace)
                .GetResult();

            Assert.True(
                result.IsSuccessful,
                $"A assembly '{assembly.GetName().Name}' não deve depender de '{forbiddenNamespace}'.");
        }
    }
}