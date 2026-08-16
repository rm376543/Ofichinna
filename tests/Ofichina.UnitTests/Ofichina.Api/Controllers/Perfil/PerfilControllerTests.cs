using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Ofichina.Api.Controllers.Perfis;
using Ofichina.Application.UseCases.Perfis.Commands;
using Ofichina.Application.UseCases.Perfis.Queries;
using Ofichina.Application.Validators.Perfis;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.Perfis;
using Ofichina.Contracts.Responses.Perfis;
using Ofichina.UnitTests.Ofichina.Api.TestDoubles;

namespace Ofichina.UnitTests.Ofichina.Api.Controllers.Perfil;

public sealed class PerfilControllerTests
{
    [Fact]
    public async Task GetAllAsync_Deve_Retornar_Sucesso()
    {
        var mediator = new FakeMediator();
        mediator.RegistrarResposta<GetPerfisQuery, Result<IReadOnlyCollection<PerfilResponse>>>(
            Result.Success<IReadOnlyCollection<PerfilResponse>>(
                [new PerfilResponse { PerfilId = Guid.NewGuid(), Nome = "ADMIN", Descricao = "Administrador" }]));

        var controller = CriarController(mediator);

        var result = await controller.GetAllAsync(CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<IReadOnlyCollection<PerfilResponse>>>(ok.Value);
        Assert.True(response.Success);
        Assert.Single(response.Data!);
    }

    [Fact]
    public async Task CreateAsync_Deve_Rejeitar_Requisicao_Invalida()
    {
        var controller = CriarController(new FakeMediator());

        var result = await controller.CreateAsync(new CreatePerfilRequest(), CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(badRequest.Value);
        Assert.False(response.Success);
    }

    [Fact]
    public async Task CreateAsync_Deve_Retornar_Sucesso()
    {
        var mediator = new FakeMediator();
        mediator.RegistrarResposta<CreatePerfilCommand, Result>(Result.Success());

        var controller = CriarController(mediator);

        var result = await controller.CreateAsync(new CreatePerfilRequest
        {
            NomePerfil = "ADMIN",
            Descricao = "Administrador"
        }, CancellationToken.None);

        Assert.Null(result.Result);
        var response = Assert.IsType<ApiResponse>(result.Value);
        Assert.True(response.Success);
        Assert.Equal("Perfil criado com sucesso, Nome: ADMIN", response.Message);
    }

    [Fact]
    public async Task DeleteAsync_Deve_Retornar_NotFound_Quando_Mediador_Falhar()
    {
        var mediator = new FakeMediator();
        mediator.RegistrarResposta<DeletePerfilCommand, Result>(Result.Failure("Perfil não encontrado."));

        var controller = CriarController(mediator);

        var result = await controller.DeleteAsync(new RemovePerfilRequest { PerfilId = Guid.NewGuid() }, CancellationToken.None);

        var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(notFound.Value);
        Assert.Equal("Perfil não encontrado.", response.Message);
    }

    private static PerfisController CriarController(FakeMediator mediator)
        => new(
            new CreatePerfilRequestValidator(),
            new UpdatePerfilRequestValidator(),
            mediator,
            NullLogger<PerfisController>.Instance)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
}