using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Ofichina.Api.Controllers.OrdensServico;
using Ofichina.Application.UseCases.OrdensServico.Commands;
using Ofichina.Application.UseCases.OrdensServico.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.OrdemServico;
using Ofichina.Contracts.Responses.OrdensServico;
using Ofichina.UnitTests.Api.TestDoubles;

namespace Ofichina.UnitTests.Api.Controllers.OrdemServico;

public sealed class OrdemServicoControllerTests
{
    [Fact]
    public async Task BuscarTodasOrdensServicoPaginadas_Deve_Retornar_Sucesso()
    {
        var mediator = new FakeMediator();
        mediator.RegistrarResposta<GetAllOrdensServicoPaginadasQuery, Result<PagedResponse<OrdemServicoDetalheResponse>>>(
            Result.Success(new PagedResponse<OrdemServicoDetalheResponse>
            {
                Items = [new OrdemServicoDetalheResponse { OrdemServicoId = Guid.NewGuid(), Cliente = "Maria", Consultor = "João", ProblemaRelatado = "Ruído", Status = "Criada", DataAbetura = "2026-08-01", DataFinalizacao = string.Empty }],
                PageNumber = 1,
                PageSize = 10,
                TotalCount = 1,
                TotalPages = 1,
                HasNextPage = false,
                HasPreviousPage = false
            }));

        var controller = CriarController(mediator);

        var result = await controller.BuscarTodasOrdensServicoPaginadas(new Pagination(1, 10), CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<PagedResponse<OrdemServicoDetalheResponse>>>(ok.Value);
        Assert.Single(response.Data!.Items);
    }

    [Fact]
    public async Task BuscarOrdemServicoPorId_Deve_Retornar_BadRequest_Quando_Nao_Existir()
    {
        var mediator = new FakeMediator();
        mediator.RegistrarResposta<GetOrdemServicoByIdQuery, Result<OrdemServicoResponse>>(Result.Failure<OrdemServicoResponse>("Ordem de serviço não encontrada."));

        var controller = CriarController(mediator);

        var result = await controller.BuscarOrdemServicoPorId(Guid.NewGuid(), CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(badRequest.Value);
        Assert.Equal("Ordem de serviço não encontrada.", response.Message);
    }

    [Theory]
    [InlineData("EmExecucao", "Execução da ordem de serviço iniciada com sucesso.")]
    [InlineData("Finalizada", "Ordem de serviço finalizada com sucesso.")]
    [InlineData("Entregue", "Ordem de serviço entregue com sucesso.")]
    [InlineData("Cancelada", "Ordem de serviço cancelada com sucesso.")]
    public async Task AlterarStatus_Deve_Retornar_Sucesso(string status, string mensagem)
    {
        var mediator = new FakeMediator();
        mediator.RegistrarResposta<AlterarStatusOrdemServicoCommand, Result>(Result.Success());

        var controller = CriarController(mediator);

        var result = status switch
        {
            "EmExecucao" => await controller.IniciarExecucaoOrdemServico(new OrdemServicoRequest { OrdemServicoId = Guid.NewGuid() }, CancellationToken.None),
            "Finalizada" => await controller.FinalizarOrdemServico(new OrdemServicoRequest { OrdemServicoId = Guid.NewGuid() }, CancellationToken.None),
            "Entregue" => await controller.EntregarOrdemServico(new OrdemServicoRequest { OrdemServicoId = Guid.NewGuid() }, CancellationToken.None),
            _ => await controller.CancelarOrdemServico(new OrdemServicoRequest { OrdemServicoId = Guid.NewGuid() }, CancellationToken.None)
        };

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(ok.Value);
        Assert.Equal(mensagem, response.Message);
    }

    private static OrdemServicoController CriarController(FakeMediator mediator)
        => new(mediator, NullLogger<OrdemServicoController>.Instance)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
}