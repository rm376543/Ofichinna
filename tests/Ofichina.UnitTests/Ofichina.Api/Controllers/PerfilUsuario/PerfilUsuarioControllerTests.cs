using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Ofichina.Api.Controllers.PerfilUsuario;
using Ofichina.Application.UseCases.PerfilUsuario.Commands;
using Ofichina.Application.UseCases.PerfilUsuario.Queries;
using Ofichina.Application.Validators.PerfilUsuario;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.PerfilUsuario;
using Ofichina.Contracts.Responses.PerfilUsuario;
using Ofichina.UnitTests.Api.TestDoubles;

namespace Ofichina.UnitTests.Api.Controllers.PerfilUsuario;

public sealed class PerfilUsuarioControllerTests
{
    [Fact]
    public async Task ObterPerfisAsync_Deve_Retornar_Sucesso()
    {
        var mediator = new FakeMediator();
        var perfis = new[] { "ADMIN", "MECANICO" };
        mediator.RegistrarResposta<ObterPerfisDoUsuarioQuery, IReadOnlyCollection<string>>(perfis);

        var controller = CriarController(mediator);
        var usuarioId = Guid.NewGuid();

        var result = await controller.ObterPerfisAsync(usuarioId);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<IReadOnlyCollection<string>>>(ok.Value);
        Assert.Equal(perfis, response.Data);
        Assert.IsType<ObterPerfisDoUsuarioQuery>(mediator.UltimoRequest);
    }

    [Fact]
    public async Task VincularAsync_Deve_Rejeitar_Requisicao_Invalida()
    {
        var mediator = new FakeMediator();
        var controller = CriarController(mediator);

        var result = await controller.VincularAsync(new VincularPerfilUsuarioRequest(), CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(badRequest.Value);
        Assert.False(response.Success);
        Assert.Empty(mediator.Enviados);
    }

    [Fact]
    public async Task VincularAsync_Deve_Retornar_Sucesso_Quando_Mediador_Concluir()
    {
        var mediator = new FakeMediator();
        mediator.RegistrarResposta<VincularPerfilUsuarioCommand, Result<VincularPerfilUsuarioResponse>>(
            Result.Success(new VincularPerfilUsuarioResponse
            {
                UsuarioId = Guid.NewGuid(),
                PerfilId = Guid.NewGuid(),
                Mensagem = "Perfil vinculado com sucesso."
            }));

        var controller = CriarController(mediator);
        var request = new VincularPerfilUsuarioRequest
        {
            UsuarioId = Guid.NewGuid(),
            PerfilId = Guid.NewGuid()
        };

        var result = await controller.VincularAsync(request, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(ok.Value);
        Assert.True(response.Success);
        Assert.Equal("Perfil vinculado com sucesso.", response.Message);
        Assert.IsType<VincularPerfilUsuarioCommand>(mediator.UltimoRequest);
    }

    private static PerfilUsuarioController CriarController(FakeMediator mediator)
        => new(
            new VincularPerfilUsuarioRequestValidator(),
            mediator,
            NullLogger<PerfilUsuarioController>.Instance)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
}