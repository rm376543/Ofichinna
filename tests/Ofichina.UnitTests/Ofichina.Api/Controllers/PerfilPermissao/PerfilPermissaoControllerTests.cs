using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Ofichina.Api.Controllers.PerfilPermissoes;
using Ofichina.Application.UseCases.PerfilPermissoes.Commands;
using Ofichina.Application.UseCases.PerfilPermissoes.Queries;
using Ofichina.Application.Validators.PerfilPermissoes;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.PerfilPermissoes;
using Ofichina.Contracts.Responses.PerfilPermissoes;
using Ofichina.UnitTests.Api.TestDoubles;

namespace Ofichina.UnitTests.Api.Controllers.PerfilPermissoes;

public sealed class PerfilPermissaoControllerTests
{
    [Fact]
    public async Task GetAllPerfisPermissoesPaginadas_Deve_Retornar_Sucesso()
    {
        var mediator = new FakeMediator();
        mediator.RegistrarResposta<GetAllPerfisPermissoesPaginadasQuery, Result<PagedResponse<PerfilPermissaoResponse>>>(
            Result.Success(new PagedResponse<PerfilPermissaoResponse>
            {
                Items = [new PerfilPermissaoResponse { PerfilPermissaoId = Guid.NewGuid(), PerfilId = Guid.NewGuid(), PermissaoId = Guid.NewGuid(), Codigo = "PERFIL_LISTAR", Descricao = "Listar perfis" }],
                PageNumber = 1,
                PageSize = 10,
                TotalCount = 1,
                TotalPages = 1,
                HasNextPage = false,
                HasPreviousPage = false
            }));

        var controller = CriarController(mediator);
        var result = await controller.GetAllPerfisPermissoesPaginadas(new Pagination(1, 10), Guid.NewGuid(), CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<PagedResponse<PerfilPermissaoResponse>>>(ok.Value);
        Assert.True(response.Success);
        Assert.Single(response.Data!.Items);
        Assert.IsType<GetAllPerfisPermissoesPaginadasQuery>(mediator.UltimoRequest);
    }

    [Fact]
    public async Task VincularAsync_Deve_Rejeitar_Requisicao_Invalida()
    {
        var controller = CriarController(new FakeMediator());

        var result = await controller.VincularAsync(new VincularPermissaoPerfilRequest(), CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(badRequest.Value);
        Assert.False(response.Success);
    }

    [Fact]
    public async Task DesvincularAsync_Deve_Retornar_Sucesso()
    {
        var mediator = new FakeMediator();
        mediator.RegistrarResposta<DesvincularPermissaoPerfilCommand, Result>(Result.Success());

        var controller = CriarController(mediator);
        var request = new DesvincularPerfilPermissao { PerfilId = Guid.NewGuid(), PermissaoId = Guid.NewGuid() };

        var result = await controller.DesvincularAsync(request, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(ok.Value);
        Assert.True(response.Success);
        Assert.Equal("Permissão desvinculada do perfil com sucesso.", response.Message);
    }

    private static PerfilPermissaoController CriarController(FakeMediator mediator)
        => new(
            new VincularPermissaoPerfilRequestValidator(),
            mediator,
            NullLogger<PerfilPermissaoController>.Instance)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
}