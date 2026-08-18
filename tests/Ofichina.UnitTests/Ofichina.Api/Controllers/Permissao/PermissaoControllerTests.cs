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
using Ofichina.UnitTests.Api.TestDoubles;

namespace Ofichina.UnitTests.Api.Controllers.Permissao;

public sealed class PermissaoControllerTests
{
    // ============================================================
    // BuscarTodasPermissoesPaginadas
    // ============================================================

    [Fact]
    public async Task BuscarTodasPermissoesPaginadas_Deve_Retornar_Sucesso()
    {
        var mediator = new FakeMediator();

        var permissao = new PermissaoResponse
        {
            PermissaoId = Guid.NewGuid(),
            Codigo = "PERMISSAO_LISTAR",
            Descricao = "Listar permissões"
        };

        var pagedResponse = new PagedResponse<PermissaoResponse>
        {
            Items = [permissao],
            PageNumber = 1,
            PageSize = 10,
            TotalCount = 1,
            TotalPages = 1,
            HasNextPage = false,
            HasPreviousPage = false
        };

        mediator.RegistrarResposta<
            GetAllPermissoesPaginadasQuery,
            Result<PagedResponse<PermissaoResponse>>>(
            Result<PagedResponse<PermissaoResponse>>.Success(
                pagedResponse));

        var controller = CriarController(mediator);

        var result = await controller.BuscarTodasPermissoesPaginadas(
            new Pagination(1, 10),
            CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);

        var response =
            Assert.IsType<ApiResponse<PagedResponse<PermissaoResponse>>>(
                ok.Value);

        Assert.True(response.Success);
        Assert.NotNull(response.Data);

        var item = Assert.Single(response.Data.Items);

        Assert.Equal(
            permissao.PermissaoId,
            item.PermissaoId);

        Assert.Equal(
            "PERMISSAO_LISTAR",
            item.Codigo);
    }

    [Fact]
    public async Task BuscarTodasPermissoesPaginadas_Deve_Retornar_BadRequest_Quando_Mediator_Falhar()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            GetAllPermissoesPaginadasQuery,
            Result<PagedResponse<PermissaoResponse>>>(
            Result<PagedResponse<PermissaoResponse>>.Failure(
                "Não foi possível obter as permissões."));

        var controller = CriarController(mediator);

        var result = await controller.BuscarTodasPermissoesPaginadas(
            new Pagination(1, 10),
            CancellationToken.None);

        var badRequest =
            Assert.IsType<BadRequestObjectResult>(result.Result);

        var response =
            Assert.IsType<ApiResponse>(badRequest.Value);

        Assert.False(response.Success);

        Assert.Equal(
            "Não foi possível obter as permissões.",
            response.Message);
    }

    // ============================================================
    // GetByIdAsync
    // ============================================================

    [Fact]
    public async Task GetByIdAsync_Deve_Retornar_Sucesso()
    {
        var mediator = new FakeMediator();

        var permissaoId = Guid.NewGuid();

        var permissao = new PermissaoResponse
        {
            PermissaoId = permissaoId,
            Codigo = "PERMISSAO_LISTAR",
            Descricao = "Listar permissões"
        };

        mediator.RegistrarResposta<
            GetPermissaoByIdQuery,
            Result<PermissaoResponse>>(
            Result<PermissaoResponse>.Success(permissao));

        var controller = CriarController(mediator);

        var result = await controller.GetByIdAsync(
            permissaoId,
            CancellationToken.None);

        var ok =
            Assert.IsType<OkObjectResult>(result.Result);

        var response =
            Assert.IsType<ApiResponse<PermissaoResponse>>(ok.Value);

        Assert.True(response.Success);
        Assert.NotNull(response.Data);

        Assert.Equal(
            permissaoId,
            response.Data.PermissaoId);

        Assert.Equal(
            "PERMISSAO_LISTAR",
            response.Data.Codigo);
    }

    [Fact]
    public async Task GetByIdAsync_Deve_Retornar_NotFound_Quando_Mediator_Falhar()
    {
        var mediator = new FakeMediator();

        var permissaoId = Guid.NewGuid();

        mediator.RegistrarResposta<
            GetPermissaoByIdQuery,
            Result<PermissaoResponse>>(
            Result<PermissaoResponse>.Failure(
                "Permissão não encontrada."));

        var controller = CriarController(mediator);

        var result = await controller.GetByIdAsync(
            permissaoId,
            CancellationToken.None);

        var notFound =
            Assert.IsType<NotFoundObjectResult>(result.Result);

        var response =
            Assert.IsType<ApiResponse>(notFound.Value);

        Assert.False(response.Success);

        Assert.Equal(
            "Permissão não encontrada.",
            response.Message);
    }

    [Fact]
    public async Task GetByIdAsync_Deve_Retornar_NotFound_Quando_Value_For_Null()
    {
        var mediator = new FakeMediator();

        var permissaoId = Guid.NewGuid();

        mediator.RegistrarResposta<
            GetPermissaoByIdQuery,
            Result<PermissaoResponse>>(
            Result<PermissaoResponse>.Success(null!));

        var controller = CriarController(mediator);

        var result = await controller.GetByIdAsync(
            permissaoId,
            CancellationToken.None);

        var notFound =
            Assert.IsType<NotFoundObjectResult>(result.Result);

        var response =
            Assert.IsType<ApiResponse>(notFound.Value);

        Assert.False(response.Success);

        Assert.Equal(
            "Permissão não encontrada.",
            response.Message);
    }

    // ============================================================
    // CreateAsync
    // ============================================================

    [Fact]
    public async Task CreateAsync_Deve_Rejeitar_Requisicao_Invalida()
    {
        var controller = CriarController(
            new FakeMediator());

        var result = await controller.CreateAsync(
            new CreatePermissaoRequest(),
            CancellationToken.None);

        var badRequest =
            Assert.IsType<BadRequestObjectResult>(result.Result);

        var response =
            Assert.IsType<ApiResponse>(badRequest.Value);

        Assert.False(response.Success);
    }

    [Fact]
    public async Task CreateAsync_Deve_Retornar_Sucesso()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            CreatePermissaoCommand,
            Result>(
            Result.Success());

        var controller = CriarController(mediator);

        var result = await controller.CreateAsync(
            new CreatePermissaoRequest
            {
                Codigo = "PERMISSAO_CRIAR",
                Descricao = "Criar permissões"
            },
            CancellationToken.None);

        var ok =
            Assert.IsType<OkObjectResult>(result.Result);

        var response =
            Assert.IsType<ApiResponse>(ok.Value);

        Assert.True(response.Success);

        Assert.Equal(
            "Permissão criada com sucesso.",
            response.Message);
    }

    [Fact]
    public async Task CreateAsync_Deve_Retornar_BadRequest_Quando_Mediator_Falhar()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            CreatePermissaoCommand,
            Result>(
            Result.Failure(
                "Não foi possível criar a permissão."));

        var controller = CriarController(mediator);

        var result = await controller.CreateAsync(
            new CreatePermissaoRequest
            {
                Codigo = "PERMISSAO_CRIAR",
                Descricao = "Criar permissões"
            },
            CancellationToken.None);

        var badRequest =
            Assert.IsType<BadRequestObjectResult>(result.Result);

        var response =
            Assert.IsType<ApiResponse>(badRequest.Value);

        Assert.False(response.Success);

        Assert.Equal(
            "Não foi possível criar a permissão.",
            response.Message);
    }

    // ============================================================
    // UpdateAsync
    // ============================================================

    [Fact]
    public async Task UpdateAsync_Deve_Retornar_Sucesso()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            UpdatePermissaoCommand,
            Result>(
            Result.Success());

        var controller = CriarController(mediator);

        var result = await controller.UpdateAsync(
            new UpdatePermissaoRequest
            {
                PermissaoId = Guid.NewGuid(),
                Codigo = "PERMISSAO_ATUALIZAR",
                Descricao = "Atualizar permissões"
            },
            CancellationToken.None);

        var ok =
            Assert.IsType<OkObjectResult>(result.Result);

        var response =
            Assert.IsType<ApiResponse>(ok.Value);

        Assert.True(response.Success);

        Assert.Equal(
            "Permissão atualizada com sucesso.",
            response.Message);
    }

    [Fact]
    public async Task UpdateAsync_Deve_Rejeitar_Requisicao_Invalida()
    {
        var controller = CriarController(
            new FakeMediator());

        var result = await controller.UpdateAsync(
            new UpdatePermissaoRequest
            {
                PermissaoId = Guid.Empty,
                Codigo = string.Empty,
                Descricao = string.Empty
            },
            CancellationToken.None);

        var badRequest =
            Assert.IsType<BadRequestObjectResult>(result.Result);

        var response =
            Assert.IsType<ApiResponse>(badRequest.Value);

        Assert.False(response.Success);
    }

    [Fact]
    public async Task UpdateAsync_Deve_Retornar_BadRequest_Quando_Mediator_Falhar()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            UpdatePermissaoCommand,
            Result>(
            Result.Failure(
                "Não foi possível atualizar a permissão."));

        var controller = CriarController(mediator);

        var result = await controller.UpdateAsync(
            new UpdatePermissaoRequest
            {
                PermissaoId = Guid.NewGuid(),
                Codigo = "PERMISSAO_ATUALIZAR",
                Descricao = "Atualizar permissões"
            },
            CancellationToken.None);

        var badRequest =
            Assert.IsType<BadRequestObjectResult>(result.Result);

        var response =
            Assert.IsType<ApiResponse>(badRequest.Value);

        Assert.False(response.Success);

        Assert.Equal(
            "Não foi possível atualizar a permissão.",
            response.Message);
    }

    // ============================================================
    // DeleteAsync
    // ============================================================

    [Fact]
    public async Task DeleteAsync_Deve_Retornar_Sucesso()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            DeletePermissaoCommand,
            Result>(
            Result.Success());

        var controller = CriarController(mediator);

        var result = await controller.DeleteAsync(
            new RemovePermissaoRequest
            {
                PermissaoId = Guid.NewGuid()
            },
            CancellationToken.None);

        var ok =
            Assert.IsType<OkObjectResult>(result.Result);

        var response =
            Assert.IsType<ApiResponse>(ok.Value);

        Assert.True(response.Success);

        Assert.Equal(
            "Permissão removida com sucesso.",
            response.Message);
    }

    [Fact]
    public async Task DeleteAsync_Deve_Retornar_BadRequest_Quando_Mediator_Falhar()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            DeletePermissaoCommand,
            Result>(
            Result.Failure(
                "Não foi possível remover a permissão."));

        var controller = CriarController(mediator);

        var result = await controller.DeleteAsync(
            new RemovePermissaoRequest
            {
                PermissaoId = Guid.NewGuid()
            },
            CancellationToken.None);

        var badRequest =
            Assert.IsType<BadRequestObjectResult>(result.Result);

        var response =
            Assert.IsType<ApiResponse>(badRequest.Value);

        Assert.False(response.Success);

        Assert.Equal(
            "Não foi possível remover a permissão.",
            response.Message);
    }

    // ============================================================
    // Helpers
    // ============================================================

    private static PermissaoController CriarController(
        FakeMediator mediator)
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