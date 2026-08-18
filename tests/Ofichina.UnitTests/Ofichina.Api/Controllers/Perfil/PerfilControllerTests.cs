using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Ofichina.Api.Controllers.Perfis;
using Ofichina.Application.UseCases.Perfis.Commands;
using Ofichina.Application.UseCases.Perfis.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.Perfis;
using Ofichina.Contracts.Responses.Perfis;
using Ofichina.UnitTests.Api.TestDoubles;

namespace Ofichina.UnitTests.Api.Controllers.Perfis;

public sealed class PerfisControllerTests
{
    // ============================================================
    // GetAllAsync
    // ============================================================

    [Fact]
    public async Task GetAllAsync_Deve_Retornar_Sucesso()
    {
        var mediator = new FakeMediator();

        var perfis = new List<PerfilResponse>
        {
            new()
            {
                PerfilId = Guid.NewGuid(),
                Nome = "ADMIN"
            },
            new()
            {
                PerfilId = Guid.NewGuid(),
                Nome = "CONSULTOR"
            }
        };

        mediator.RegistrarResposta<
            GetPerfisQuery,
            Result<IReadOnlyCollection<PerfilResponse>>>(
            Result<IReadOnlyCollection<PerfilResponse>>.Success(perfis));

        var controller = CriarController(mediator);

        var result = await controller.GetAllAsync(
            CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);

        var response =
            Assert.IsType<ApiResponse<IReadOnlyCollection<PerfilResponse>>>(
                ok.Value);

        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal(2, response.Data.Count);
    }

    [Fact]
    public async Task GetAllAsync_Deve_Retornar_BadRequest_Quando_Mediator_Falhar()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            GetPerfisQuery,
            Result<IReadOnlyCollection<PerfilResponse>>>(
            Result<IReadOnlyCollection<PerfilResponse>>.Failure(
                "Não foi possível obter os perfis."));

        var controller = CriarController(mediator);

        var result = await controller.GetAllAsync(
            CancellationToken.None);

        var badRequest =
            Assert.IsType<BadRequestObjectResult>(result.Result);

        var response =
            Assert.IsType<ApiResponse>(badRequest.Value);

        Assert.False(response.Success);
        Assert.Equal(
            "Não foi possível obter os perfis.",
            response.Message);
    }

    // ============================================================
    // GetByIdAsync
    // ============================================================

    [Fact]
    public async Task GetByIdAsync_Deve_Retornar_Sucesso()
    {
        var mediator = new FakeMediator();

        var perfilId = Guid.NewGuid();

        var perfil = new PerfilResponse
        {
            PerfilId = perfilId,
            Nome = "ADMIN"
        };

        mediator.RegistrarResposta<
            GetPerfilByIdQuery,
            Result<PerfilResponse>>(
            Result<PerfilResponse>.Success(perfil));

        var controller = CriarController(mediator);

        var result = await controller.GetByIdAsync(
            perfilId,
            CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);

        var response =
            Assert.IsType<ApiResponse<PerfilResponse>>(ok.Value);

        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal(perfilId, response.Data.PerfilId);
        Assert.Equal("ADMIN", response.Data.Nome);
    }

    [Fact]
    public async Task GetByIdAsync_Deve_Retornar_NotFound_Quando_Mediator_Falhar()
    {
        var mediator = new FakeMediator();

        var perfilId = Guid.NewGuid();

        mediator.RegistrarResposta<
            GetPerfilByIdQuery,
            Result<PerfilResponse>>(
            Result<PerfilResponse>.Failure(
                "Perfil não encontrado."));

        var controller = CriarController(mediator);

        var result = await controller.GetByIdAsync(
            perfilId,
            CancellationToken.None);

        var notFound =
            Assert.IsType<NotFoundObjectResult>(result.Result);

        var response =
            Assert.IsType<ApiResponse>(notFound.Value);

        Assert.False(response.Success);
        Assert.Equal(
            "Perfil não encontrado.",
            response.Message);
    }

    [Fact]
    public async Task GetByIdAsync_Deve_Retornar_NotFound_Quando_Value_For_Null()
    {
        var mediator = new FakeMediator();

        var perfilId = Guid.NewGuid();

        mediator.RegistrarResposta<
            GetPerfilByIdQuery,
            Result<PerfilResponse>>(
            Result<PerfilResponse>.Success(null!));

        var controller = CriarController(mediator);

        var result = await controller.GetByIdAsync(
            perfilId,
            CancellationToken.None);

        var notFound =
            Assert.IsType<NotFoundObjectResult>(result.Result);

        var response =
            Assert.IsType<ApiResponse>(notFound.Value);

        Assert.False(response.Success);
        Assert.Equal(
            "Perfil não encontrado.",
            response.Message);
    }

    // ============================================================
    // CreateAsync
    // ============================================================

    [Fact]
    public async Task CreateAsync_Deve_Retornar_Sucesso()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            CreatePerfilCommand,
            Result>(
            Result.Success());

        var controller = CriarController(mediator);

        var request = new CreatePerfilRequest
        {
            NomePerfil = "ADMIN",
            Descricao = "Perfil administrador"
        };

        var result = await controller.CreateAsync(
            request,
            CancellationToken.None);

        var response = Assert.IsType<ApiResponse>(result.Value);

        Assert.True(response.Success);
        Assert.Equal(
            "Perfil criado com sucesso, Nome: ADMIN",
            response.Message);
    }

    [Fact]
    public async Task CreateAsync_Deve_Retornar_BadRequest_Quando_Validacao_Falhar()
    {
        var mediator = new FakeMediator();

        var createValidator =
            new InlineValidator<CreatePerfilRequest>();

        createValidator
            .RuleFor(x => x.NomePerfil)
            .NotEmpty()
            .WithMessage("Nome do perfil é obrigatório.");

        var updateValidator =
            new InlineValidator<UpdatePerfilRequest>();

        var request = new CreatePerfilRequest
        {
            NomePerfil = string.Empty,
            Descricao = "Perfil administrativo"
        };

        var controller = CriarController(
            mediator,
            createValidator,
            updateValidator);

        var result = await controller.CreateAsync(
            request,
            CancellationToken.None);

        var badRequest =
            Assert.IsType<BadRequestObjectResult>(result.Result);

        var response =
            Assert.IsType<ApiResponse>(badRequest.Value);

        Assert.False(response.Success);

        Assert.Contains(
            "Nome do perfil é obrigatório.",
            response.Errors);
    }

    // ============================================================
    // UpdateAsync
    // ============================================================

    [Fact]
    public async Task UpdateAsync_Deve_Retornar_Sucesso()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            UpdatePerfilCommand,
            Result>(
            Result.Success());

        var createValidator =
            new InlineValidator<CreatePerfilRequest>();

        var updateValidator =
            new InlineValidator<UpdatePerfilRequest>();

        var request = new UpdatePerfilRequest
        {
            PerfilId = Guid.NewGuid(),
            NomePerfil = "ADMIN",
            Descricao = "Perfil administrativo atualizado"
        };

        var controller = CriarController(
            mediator,
            createValidator,
            updateValidator);

        var result = await controller.UpdateAsync(
            request,
            CancellationToken.None);

        var ok =
            Assert.IsType<OkObjectResult>(result.Result);

        var response =
            Assert.IsType<ApiResponse>(ok.Value);

        Assert.True(response.Success);
        Assert.Equal(
            "Perfil atualizado com sucesso.",
            response.Message);
    }

    [Fact]
    public async Task UpdateAsync_Deve_Retornar_BadRequest_Quando_Validacao_Falhar()
    {
        var mediator = new FakeMediator();

        var createValidator =
            new InlineValidator<CreatePerfilRequest>();

        var updateValidator =
            new InlineValidator<UpdatePerfilRequest>();

        updateValidator
            .RuleFor(x => x.NomePerfil)
            .NotEmpty()
            .WithMessage("Nome do perfil é obrigatório.");

        var request = new UpdatePerfilRequest
        {
            PerfilId = Guid.NewGuid(),
            NomePerfil = string.Empty,
            Descricao = "Perfil administrativo"
        };

        var controller = CriarController(
            mediator,
            createValidator,
            updateValidator);

        var result = await controller.UpdateAsync(
            request,
            CancellationToken.None);

        var badRequest =
            Assert.IsType<BadRequestObjectResult>(result.Result);

        var response =
            Assert.IsType<ApiResponse>(badRequest.Value);

        Assert.False(response.Success);

        Assert.Contains(
            "Nome do perfil é obrigatório.",
            response.Errors);
    }

    [Fact]
    public async Task UpdateAsync_Deve_Retornar_BadRequest_Quando_Mediator_Falhar()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            UpdatePerfilCommand,
            Result>(
            Result.Failure(
                "Não foi possível atualizar o perfil."));

        var createValidator =
            new InlineValidator<CreatePerfilRequest>();

        var updateValidator =
            new InlineValidator<UpdatePerfilRequest>();

        var request = new UpdatePerfilRequest
        {
            PerfilId = Guid.NewGuid(),
            NomePerfil = "ADMIN",
            Descricao = "Perfil administrativo"
        };

        var controller = CriarController(
            mediator,
            createValidator,
            updateValidator);

        var result = await controller.UpdateAsync(
            request,
            CancellationToken.None);

        var badRequest =
            Assert.IsType<BadRequestObjectResult>(result.Result);

        var response =
            Assert.IsType<ApiResponse>(badRequest.Value);

        Assert.False(response.Success);
        Assert.Equal(
            "Não foi possível atualizar o perfil.",
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
            DeletePerfilCommand,
            Result>(
            Result.Success());

        var createValidator =
            new InlineValidator<CreatePerfilRequest>();

        var updateValidator =
            new InlineValidator<UpdatePerfilRequest>();

        var request = new RemovePerfilRequest
        {
            PerfilId = Guid.NewGuid()
        };

        var controller = CriarController(
            mediator,
            createValidator,
            updateValidator);

        var result = await controller.DeleteAsync(
            request,
            CancellationToken.None);

        var ok =
            Assert.IsType<OkObjectResult>(result.Result);

        var response =
            Assert.IsType<ApiResponse>(ok.Value);

        Assert.True(response.Success);
        Assert.Equal(
            "Perfil desativado com sucesso.",
            response.Message);
    }

    [Fact]
    public async Task DeleteAsync_Deve_Retornar_NotFound_Quando_Mediator_Falhar()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            DeletePerfilCommand,
            Result>(
            Result.Failure(
                "Perfil não encontrado."));

        var createValidator =
            new InlineValidator<CreatePerfilRequest>();

        var updateValidator =
            new InlineValidator<UpdatePerfilRequest>();

        var request = new RemovePerfilRequest
        {
            PerfilId = Guid.NewGuid()
        };

        var controller = CriarController(
            mediator,
            createValidator,
            updateValidator);

        var result = await controller.DeleteAsync(
            request,
            CancellationToken.None);

        var notFound =
            Assert.IsType<NotFoundObjectResult>(result.Result);

        var response =
            Assert.IsType<ApiResponse>(notFound.Value);

        Assert.False(response.Success);
        Assert.Equal(
            "Perfil não encontrado.",
            response.Message);
    }

    // ============================================================
    // Helpers
    // ============================================================

    private static PerfisController CriarController(
        FakeMediator mediator,
        IValidator<CreatePerfilRequest>? createValidator = null,
        IValidator<UpdatePerfilRequest>? updateValidator = null)
    {
        createValidator ??=
            new InlineValidator<CreatePerfilRequest>();

        updateValidator ??=
            new InlineValidator<UpdatePerfilRequest>();

        return new PerfisController(
            createValidator,
            updateValidator,
            mediator,
            NullLogger<PerfisController>.Instance)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }
}