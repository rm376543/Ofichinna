using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Ofichina.Api.Controllers.Permissoes;
using Ofichina.Application.UseCases.Permissoes.Commands;
using Ofichina.Application.UseCases.Permissoes.Queries;
using Ofichina.Application.Validators.Permissoes;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.Permissoes;
using Ofichina.Contracts.Responses.Permissoes;
using Ofichina.UnitTests.Ofichina.Api.TestDoubles;

namespace Ofichina.UnitTests.Ofichina.Api.Controllers.Permissao;

public sealed class PermissaoControllerTests
{
    [Fact]
    public async Task BuscarTodasPermissoesPaginadas_Deve_Retornar_Sucesso()
    {
        var mediator = new FakeMediator();
        mediator.RegistrarResposta<GetAllPermissoesPaginadasQuery, Result<PagedResponse<PermissaoResponse>>>(
            Result.Success(new PagedResponse<PermissaoResponse>
            {
                Items = [new PermissaoResponse { PermissaoId = Guid.NewGuid(), Codigo = "PERMISSAO_LISTAR", Descricao = "Listar permissões" }],
                PageNumber = 1,
                PageSize = 10,
                TotalCount = 1,
                TotalPages = 1,
                HasNextPage = false,
                HasPreviousPage = false
            }));

        var controller = CriarController(mediator);

        var result = await controller.BuscarTodasPermissoesPaginadas(new Pagination(1, 10), CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<PagedResponse<PermissaoResponse>>>(ok.Value);
        Assert.Single(response.Data!.Items);
    }

    [Fact]
    public async Task CreateAsync_Deve_Rejeitar_Requisicao_Invalida()
    {
        var controller = CriarController(new FakeMediator());

        var result = await controller.CreateAsync(new CreatePermissaoRequest(), CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.IsType<ApiResponse>(badRequest.Value);
    }

    [Fact]
    public async Task UpdateAsync_Deve_Retornar_Sucesso()
    {
        var mediator = new FakeMediator();
        mediator.RegistrarResposta<UpdatePermissaoCommand, Result>(Result.Success());

        var controller = CriarController(mediator);

        var result = await controller.UpdateAsync(new UpdatePermissaoRequest
        {
            PermissaoId = Guid.NewGuid(),
            Codigo = "PERMISSAO_ATUALIZAR",
            Descricao = "Atualizar permissões"
        }, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(ok.Value);
        Assert.Equal("Permissão atualizada com sucesso.", response.Message);
    }

    [Fact]
    public async Task DeleteAsync_Deve_Retornar_Sucesso()
    {
        var mediator = new FakeMediator();
        mediator.RegistrarResposta<DeletePermissaoCommand, Result>(Result.Success());

        var controller = CriarController(mediator);

        var result = await controller.DeleteAsync(new RemovePermissaoRequest { PermissaoId = Guid.NewGuid() }, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(ok.Value);
        Assert.True(response.Success);
        Assert.Equal("Permissão removida com sucesso.", response.Message);
    }

    private static PermissaoController CriarController(FakeMediator mediator)
        => new(
            new CreatePermissaoRequestValidator(),
            new UpdatePermissaoRequestValidator(),
            mediator,
            NullLogger<PermissaoController>.Instance)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
}