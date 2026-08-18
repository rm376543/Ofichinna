using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Ofichina.Api.Controllers.Veiculo;
using Ofichina.Application.UseCases.Veiculos.Commands;
using Ofichina.Application.UseCases.Veiculos.Queries;
using Ofichina.Application.Validators.Veiculo;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.Veiculo;
using Ofichina.Contracts.Responses.Pessoa;
using Ofichina.Contracts.Responses.Veiculo;
using Ofichina.UnitTests.Api.TestDoubles;

namespace Ofichina.UnitTests.Api.Controllers.Veiculo;

public sealed class VeiculoControllerTests
{
    // ============================================================
    // BuscarVeiculosPorPessoaId
    // ============================================================

    [Fact]
    public async Task BuscarVeiculosPorPessoaId_Deve_Retornar_Sucesso()
    {
        var mediator = new FakeMediator();

        var pessoa = new PessoaVeiculoResponse();

        mediator.RegistrarResposta<
            GetVeiculosByPessoaIdQuery,
            Result<PessoaVeiculoResponse>>(
            Result<PessoaVeiculoResponse>.Success(pessoa));

        var controller = CriarController(mediator);

        var result = await controller.BuscarVeiculosPorPessoaId(
            Guid.NewGuid(),
            CancellationToken.None);

        var ok =
            Assert.IsType<OkObjectResult>(result.Result);

        var response =
            Assert.IsType<ApiResponse<PessoaVeiculoResponse>>(ok.Value);

        Assert.True(response.Success);
        Assert.NotNull(response.Data);
    }

    [Fact]
    public async Task BuscarVeiculosPorPessoaId_Deve_Retornar_BadRequest_Quando_Mediator_Falhar()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            GetVeiculosByPessoaIdQuery,
            Result<PessoaVeiculoResponse>>(
            Result<PessoaVeiculoResponse>.Failure(
                "Não foi possível obter os veículos."));

        var controller = CriarController(mediator);

        var result = await controller.BuscarVeiculosPorPessoaId(
            Guid.NewGuid(),
            CancellationToken.None);

        var badRequest =
            Assert.IsType<BadRequestObjectResult>(result.Result);

        var response =
            Assert.IsType<ApiResponse>(badRequest.Value);

        Assert.False(response.Success);

        Assert.Equal(
            "Não foi possível obter os veículos.",
            response.Message);
    }

    // ============================================================
    // BuscarTodosVeiculosPaginado
    // ============================================================

    [Fact]
    public async Task BuscarTodosVeiculosPaginado_Deve_Retornar_Sucesso()
    {
        var mediator = new FakeMediator();

        var veiculo = new VeiculoResponse
        {
            VeiculoId = Guid.NewGuid(),
            Placa = "ABC1D23"
        };

        var pagedResponse = new PagedResponse<VeiculoResponse>
        {
            Items = [veiculo],
            PageNumber = 1,
            PageSize = 10,
            TotalCount = 1,
            TotalPages = 1,
            HasNextPage = false,
            HasPreviousPage = false
        };

        mediator.RegistrarResposta<
            GetAllVeiculosPaginadosQuery,
            Result<PagedResponse<VeiculoResponse>>>(
            Result<PagedResponse<VeiculoResponse>>.Success(
                pagedResponse));

        var controller = CriarController(mediator);

        var result = await controller.BuscarTodosVeiculosPaginado(
            new Pagination(1, 10),
            CancellationToken.None);

        var ok =
            Assert.IsType<OkObjectResult>(result.Result);

        var response =
            Assert.IsType<
                ApiResponse<PagedResponse<VeiculoResponse>>>(
                ok.Value);

        Assert.True(response.Success);
        Assert.NotNull(response.Data);

        var item = Assert.Single(response.Data.Items);

        Assert.Equal(
            veiculo.VeiculoId,
            item.VeiculoId);

        Assert.Equal(
            "ABC1D23",
            item.Placa);
    }

    [Fact]
    public async Task BuscarTodosVeiculosPaginado_Deve_Retornar_BadRequest_Quando_Mediator_Falhar()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            GetAllVeiculosPaginadosQuery,
            Result<PagedResponse<VeiculoResponse>>>(
            Result<PagedResponse<VeiculoResponse>>.Failure(
                "Não foi possível obter os veículos."));

        var controller = CriarController(mediator);

        var result = await controller.BuscarTodosVeiculosPaginado(
            new Pagination(1, 10),
            CancellationToken.None);

        var badRequest =
            Assert.IsType<BadRequestObjectResult>(result.Result);

        var response =
            Assert.IsType<ApiResponse>(badRequest.Value);

        Assert.False(response.Success);

        Assert.Equal(
            "Não foi possível obter os veículos.",
            response.Message);
    }

    // ============================================================
    // BuscarVeiculoPorId
    // ============================================================

    [Fact]
    public async Task BuscarVeiculoPorId_Deve_Retornar_Sucesso()
    {
        var mediator = new FakeMediator();

        var veiculoId = Guid.NewGuid();

        var veiculo = new VeiculoResponse
        {
            VeiculoId = veiculoId,
            Placa = "ABC1D23"
        };

        mediator.RegistrarResposta<
            GetVeiculoByIdQuery,
            Result<VeiculoResponse>>(
            Result<VeiculoResponse>.Success(veiculo));

        var controller = CriarController(mediator);

        var result = await controller.BuscarVeiculoPorId(
            veiculoId,
            CancellationToken.None);

        var ok =
            Assert.IsType<OkObjectResult>(result.Result);

        var response =
            Assert.IsType<ApiResponse<VeiculoResponse>>(ok.Value);

        Assert.True(response.Success);
        Assert.NotNull(response.Data);

        Assert.Equal(
            veiculoId,
            response.Data.VeiculoId);

        Assert.Equal(
            "ABC1D23",
            response.Data.Placa);
    }

    [Fact]
    public async Task BuscarVeiculoPorId_Deve_Retornar_NotFound_Quando_Mediator_Falhar()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            GetVeiculoByIdQuery,
            Result<VeiculoResponse>>(
            Result<VeiculoResponse>.Failure(
                "Veículo não encontrado."));

        var controller = CriarController(mediator);

        var result = await controller.BuscarVeiculoPorId(
            Guid.NewGuid(),
            CancellationToken.None);

        var notFound =
            Assert.IsType<NotFoundObjectResult>(result.Result);

        var response =
            Assert.IsType<ApiResponse>(notFound.Value);

        Assert.False(response.Success);

        Assert.Equal(
            "Veículo não encontrado.",
            response.Message);
    }

    [Fact]
    public async Task BuscarVeiculoPorId_Deve_Retornar_NotFound_Quando_Value_For_Null()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            GetVeiculoByIdQuery,
            Result<VeiculoResponse>>(
            Result<VeiculoResponse>.Success(null!));

        var controller = CriarController(mediator);

        var result = await controller.BuscarVeiculoPorId(
            Guid.NewGuid(),
            CancellationToken.None);

        var notFound =
            Assert.IsType<NotFoundObjectResult>(result.Result);

        var response =
            Assert.IsType<ApiResponse>(notFound.Value);

        Assert.False(response.Success);

        Assert.Equal(
            "Veículo não encontrado.",
            response.Message);
    }

    // ============================================================
    // CriarVeiculo
    // ============================================================

    [Fact]
    public async Task CriarVeiculo_Deve_Rejeitar_Requisicao_Invalida()
    {
        var controller = CriarController(
            new FakeMediator());

        var result = await controller.CriarVeiculo(
            new CreateVeiculoRequest(),
            CancellationToken.None);

        var badRequest =
            Assert.IsType<BadRequestObjectResult>(result.Result);

        var response =
            Assert.IsType<ApiResponse>(badRequest.Value);

        Assert.False(response.Success);
    }

    [Fact]
    public async Task CriarVeiculo_Deve_Retornar_Sucesso()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            CreateVeiculoCommand,
            Result>(
            Result.Success());

        var controller = CriarController(mediator);

        var result = await controller.CriarVeiculo(
            CriarCreateRequestValido(),
            CancellationToken.None);

        var ok =
            Assert.IsType<OkObjectResult>(result.Result);

        var response =
            Assert.IsType<ApiResponse>(ok.Value);

        Assert.True(response.Success);

        Assert.Equal(
            "Veículo cadastrado com sucesso.",
            response.Message);
    }

    [Fact]
    public async Task CriarVeiculo_Deve_Retornar_BadRequest_Quando_Mediator_Falhar()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            CreateVeiculoCommand,
            Result>(
            Result.Failure(
                "Não foi possível criar o veículo."));

        var controller = CriarController(mediator);

        var result = await controller.CriarVeiculo(
            CriarCreateRequestValido(),
            CancellationToken.None);

        var badRequest =
            Assert.IsType<BadRequestObjectResult>(result.Result);

        var response =
            Assert.IsType<ApiResponse>(badRequest.Value);

        Assert.False(response.Success);

        Assert.Equal(
            "Não foi possível criar o veículo.",
            response.Message);
    }

    // ============================================================
    // AtualizarVeiculo
    // ============================================================

    [Fact]
    public async Task AtualizarVeiculo_Deve_Rejeitar_Requisicao_Invalida()
    {
        var controller = CriarController(
            new FakeMediator());

        var result = await controller.AtualizarVeiculo(
            new UpdateVeiculoRequest(),
            CancellationToken.None);

        var badRequest =
            Assert.IsType<BadRequestObjectResult>(result.Result);

        var response =
            Assert.IsType<ApiResponse>(badRequest.Value);

        Assert.False(response.Success);
    }

    [Fact]
    public async Task AtualizarVeiculo_Deve_Retornar_Sucesso()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            UpdateVeiculoCommand,
            Result>(
            Result.Success());

        var controller = CriarController(mediator);

        var result = await controller.AtualizarVeiculo(
            CriarUpdateRequestValido(),
            CancellationToken.None);

        var ok =
            Assert.IsType<OkObjectResult>(result.Result);

        var response =
            Assert.IsType<ApiResponse>(ok.Value);

        Assert.True(response.Success);

        Assert.Equal(
            "Veículo atualizado com sucesso.",
            response.Message);
    }

    [Fact]
    public async Task AtualizarVeiculo_Deve_Retornar_BadRequest_Quando_Mediator_Falhar()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            UpdateVeiculoCommand,
            Result>(
            Result.Failure(
                "Não foi possível atualizar o veículo."));

        var controller = CriarController(mediator);

        var result = await controller.AtualizarVeiculo(
            CriarUpdateRequestValido(),
            CancellationToken.None);

        var badRequest =
            Assert.IsType<BadRequestObjectResult>(result.Result);

        var response =
            Assert.IsType<ApiResponse>(badRequest.Value);

        Assert.False(response.Success);

        Assert.Equal(
            "Não foi possível atualizar o veículo.",
            response.Message);
    }

    // ============================================================
    // RemoverVeiculo
    // ============================================================

    [Fact]
    public async Task RemoverVeiculo_Deve_Retornar_Sucesso()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            DeleteVeiculoCommand,
            Result>(
            Result.Success());

        var controller = CriarController(mediator);

        var result = await controller.RemoverVeiculo(
            new RemoveVeiculoRequest
            {
                VeiculoId = Guid.NewGuid()
            },
            CancellationToken.None);

        var ok =
            Assert.IsType<OkObjectResult>(result.Result);

        var response =
            Assert.IsType<ApiResponse>(ok.Value);

        Assert.True(response.Success);

        Assert.Equal(
            "Veículo removido com sucesso.",
            response.Message);
    }

    [Fact]
    public async Task RemoverVeiculo_Deve_Retornar_NotFound_Quando_Mediator_Falhar()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            DeleteVeiculoCommand,
            Result>(
            Result.Failure(
                "Veículo não encontrado."));

        var controller = CriarController(mediator);

        var result = await controller.RemoverVeiculo(
            new RemoveVeiculoRequest
            {
                VeiculoId = Guid.NewGuid()
            },
            CancellationToken.None);

        var notFound =
            Assert.IsType<NotFoundObjectResult>(result.Result);

        var response =
            Assert.IsType<ApiResponse>(notFound.Value);

        Assert.False(response.Success);

        Assert.Equal(
            "Veículo não encontrado.",
            response.Message);
    }

    // ============================================================
    // Helpers
    // ============================================================

    private static VeiculoController CriarController(
        FakeMediator mediator)
        => new(
            new CreateVeiculoRequestValidator(),
            new UpdateVeiculoRequestValidator(),
            mediator,
            NullLogger<VeiculoController>.Instance)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };

    private static CreateVeiculoRequest CriarCreateRequestValido()
        => new()
        {
            PessoaId = Guid.NewGuid(),
            Placa = "ABC1D23",
            Marca = "Ford",
            Modelo = "Ka",
            AnoFabricacao = DateTime.UtcNow.Year,
            Cor = "Prata",
            Hodometro = 10000
        };

    private static UpdateVeiculoRequest CriarUpdateRequestValido()
        => new()
        {
            VeiculoId = Guid.NewGuid(),
            PessoaId = Guid.NewGuid(),
            Placa = "ABC1D23",
            Marca = "Ford",
            Modelo = "Ka",
            AnoFabricacao = DateTime.UtcNow.Year,
            Cor = "Prata",
            Hodometro = 10000
        };
}