using FluentValidation;
using MediatR;
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
using Ofichina.UnitTests.Ofichina.Api.TestDoubles;

namespace Ofichina.UnitTests.Ofichina.Api.Controllers.Veiculo;

public sealed class VeiculoControllerTests
{
    [Fact]
    public async Task BuscarVeiculosPorPessoaId_Deve_Retornar_Sucesso()
    {
        var mediator = new FakeMediator();
        mediator.RegistrarResposta<GetVeiculosByPessoaIdQuery, Result<PessoaVeiculoResponse>>(
            Result.Success(new PessoaVeiculoResponse()));

        var controller = CriarController(mediator);

        var result = await controller.BuscarVeiculosPorPessoaId(Guid.NewGuid(), CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<PessoaVeiculoResponse>>(ok.Value);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
    }

    [Fact]
    public async Task CriarVeiculo_Deve_Rejeitar_Requisicao_Invalida()
    {
        var controller = CriarController(new FakeMediator());

        var result = await controller.CriarVeiculo(new CreateVeiculoRequest(), CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.IsType<ApiResponse>(badRequest.Value);
    }

    [Fact]
    public async Task AtualizarVeiculo_Deve_Retornar_Sucesso()
    {
        var mediator = new FakeMediator();
        mediator.RegistrarResposta<UpdateVeiculoCommand, Result>(Result.Success());

        var controller = CriarController(mediator);

        var result = await controller.AtualizarVeiculo(CriarUpdateRequestValido(), CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(ok.Value);
        Assert.Equal("Veículo atualizado com sucesso.", response.Message);
    }

    [Fact]
    public async Task RemoverVeiculo_Deve_Retornar_NotFound_Quando_Nao_Existir()
    {
        var mediator = new FakeMediator();
        mediator.RegistrarResposta<DeleteVeiculoCommand, Result>(Result.Failure("Veículo não encontrado."));

        var controller = CriarController(mediator);

        var result = await controller.RemoverVeiculo(new RemoveVeiculoRequest { VeiculoId = Guid.NewGuid() }, CancellationToken.None);

        var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(notFound.Value);
        Assert.Equal("Veículo não encontrado.", response.Message);
    }

    private static VeiculoController CriarController(FakeMediator mediator)
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