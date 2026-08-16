using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Ofichina.Api.Controllers.Agendamento;
using Ofichina.Application.UseCases.Agendamentos.Commands;
using Ofichina.Application.UseCases.Agendamentos.Queries;
using Ofichina.Application.Validators.Agendamento;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.Agendamento;
using Ofichina.Contracts.Responses.Agendamento;
using Ofichina.Contracts.Responses.Agendamento.Consultor;
using Ofichina.UnitTests.Ofichina.Api.TestDoubles;

namespace Ofichina.UnitTests.Ofichina.Api.Controllers.Agendamento;

public sealed class AgendamentoControllerTests
{
    [Fact]
    public async Task BuscarHorarios_Deve_Retornar_Sucesso()
    {
        var mediator = new FakeMediator();
        mediator.RegistrarResposta<GetHorariosDisponiveisQuery, Result<PagedResponse<HorarioResponse>>>(
            Result.Success(new PagedResponse<HorarioResponse>
            {
                Items = [new HorarioResponse { HorarioId = Guid.NewGuid(), Horario = new TimeOnly(8, 0), Disponivel = true }],
                PageNumber = 1,
                PageSize = 10,
                TotalCount = 1,
                TotalPages = 1,
                HasNextPage = false,
                HasPreviousPage = false
            }));

        var controller = CriarController(mediator);

        var result = await controller.BuscarHorarios(new Pagination(1, 10));

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<PagedResponse<HorarioResponse>>>(ok.Value);
        Assert.Single(response.Data!.Items);
    }

    [Fact]
    public async Task CriarAsync_Deve_Rejeitar_Requisicao_Invalida()
    {
        var controller = CriarController(new FakeMediator());

        var result = await controller.CriarAsync(new CreateAgendamentoRequest(), CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(badRequest.Value);
        Assert.False(response.Success);
    }

    [Fact]
    public async Task CriarAsync_Deve_Retornar_Sucesso()
    {
        var mediator = new FakeMediator();
        mediator.RegistrarResposta<CreateAgendamentoCommand, Result>(Result.Success());

        var controller = CriarController(mediator);

        var result = await controller.CriarAsync(new CreateAgendamentoRequest
        {
            PessoaId = Guid.NewGuid(),
            AgendaConsultorId = Guid.NewGuid(),
            VeiculoId = Guid.NewGuid(),
            Hodometro = 1000,
            Descricao = "Troca de óleo"
        }, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(ok.Value);
        Assert.Equal("Agendamento criado com sucesso.", response.Message);
    }

    [Fact]
    public async Task CancelarAgendamentoAsync_Deve_Retornar_Sucesso()
    {
        var mediator = new FakeMediator();
        mediator.RegistrarResposta<CancelarAgendamentoCommand, Result>(Result.Success());

        var controller = CriarController(mediator);

        var result = await controller.CancelarAgendamentoAsync(new CancelarAgendamentoRequest { AgendamentoId = Guid.NewGuid() }, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(ok.Value);
        Assert.True(response.Success);
        Assert.Equal("Agendamento cancelado com sucesso.", response.Message);
    }

    [Fact]
    public async Task IniciarAsync_Deve_Retornar_Sucesso()
    {
        var mediator = new FakeMediator();
        mediator.RegistrarResposta<IniciarAgendamentoCommand, Result>(Result.Success());

        var controller = CriarController(mediator);

        var result = await controller.IniciarAsync(Guid.NewGuid(), CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(ok.Value);
        Assert.Equal("Agendamento iniciado com sucesso.", response.Message);
    }

    private static AgendamentoController CriarController(FakeMediator mediator)
        => new(
            new CreateAgendamentoRequestValidator(),
            mediator,
            NullLogger<AgendamentoController>.Instance)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
}