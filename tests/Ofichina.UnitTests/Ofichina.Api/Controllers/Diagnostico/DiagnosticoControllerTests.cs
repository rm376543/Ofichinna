using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Ofichina.Api.Controllers;
using Ofichina.Application.UseCases.Orcamentos.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.Orcamento;
using Ofichina.UnitTests.Ofichina.Api.TestDoubles;

namespace Ofichina.UnitTests.Ofichina.Api.Controllers.Diagnostico;

public sealed class DiagnosticoControllerTests
{
    // ============================================================
    // IniciarDiagnosticoOrcamento
    // ============================================================

    [Fact]
    public async Task IniciarDiagnosticoOrcamento_Deve_Retornar_Sucesso()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            IniciarDiagnosticoOrcamentoCommand,
            Result>(
            Result.Success());

        var controller = CriarController(mediator);

        var request = new OrcamentoRequest(Guid.NewGuid());

        var result = await controller.IniciarDiagnosticoOrcamento(
            request,
            CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);

        var response = Assert.IsType<ApiResponse>(ok.Value);

        Assert.True(response.Success);
        Assert.Equal(
            "Diagnóstico do orçamento iniciado com sucesso.",
            response.Message);
    }

    [Fact]
    public async Task IniciarDiagnosticoOrcamento_Deve_Retornar_BadRequest_Quando_Mediator_Falhar()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            IniciarDiagnosticoOrcamentoCommand,
            Result>(
            Result.Failure(
                "Não foi possível iniciar o diagnóstico."));

        var controller = CriarController(mediator);

        var request = new OrcamentoRequest(Guid.NewGuid());

        var result = await controller.IniciarDiagnosticoOrcamento(
            request,
            CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);

        var response = Assert.IsType<ApiResponse>(badRequest.Value);

        Assert.False(response.Success);
        Assert.Equal(
            "Não foi possível iniciar o diagnóstico.",
            response.Message);
    }

    // ============================================================
    // FinalizarOrcamento
    // ============================================================

    [Fact]
    public async Task FinalizarOrcamento_Deve_Retornar_Sucesso()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            FinalizarOrcamentoCommand,
            Result>(
            Result.Success());

        var controller = CriarController(mediator);

        var request = new OrcamentoRequest(Guid.NewGuid());

        var result = await controller.FinalizarOrcamento(
            request,
            CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);

        var response = Assert.IsType<ApiResponse>(ok.Value);

        Assert.True(response.Success);
        Assert.Equal(
            "Orçamento finalizado com sucesso.",
            response.Message);
    }

    [Fact]
    public async Task FinalizarOrcamento_Deve_Retornar_BadRequest_Quando_Mediator_Falhar()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            FinalizarOrcamentoCommand,
            Result>(
            Result.Failure(
                "Não foi possível finalizar o orçamento."));

        var controller = CriarController(mediator);

        var request = new OrcamentoRequest(Guid.NewGuid());

        var result = await controller.FinalizarOrcamento(
            request,
            CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);

        var response = Assert.IsType<ApiResponse>(badRequest.Value);

        Assert.False(response.Success);
        Assert.Equal(
            "Não foi possível finalizar o orçamento.",
            response.Message);
    }

    // ============================================================
    // Helpers
    // ============================================================

    private static DiagnosticoController CriarController(
        FakeMediator mediator)
    {
        return new DiagnosticoController(
            mediator,
            NullLogger<DiagnosticoController>.Instance)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }
}