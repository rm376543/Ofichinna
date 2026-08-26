using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Ofichina.Api.Controllers.Agendamento;
using Ofichina.Application;
using Ofichina.Application.UseCases.Agendamentos.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.Agendamento;
using Ofichina.Contracts.Responses.Agendamento;

namespace Ofichina.IntegrationTests.Api.Controllers.Agendamento;

public sealed class AgendamentoControllerTests
{
    [Fact]
    public async Task IniciarAsync_Deve_Enviar_Id_Do_Agendamento()
    {
        // Arrange
        var mediator = new FakeMediator();

        var controller = new AgendamentoController(
            new InlineValidator<CreateAgendamentoRequest>(),
            mediator,
            NullLogger<AgendamentoController>.Instance);

        var id = Guid.NewGuid();

        // Act
        var result = await controller.IniciarAsync(
            id,
            CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);

        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.NotNull(mediator.CommandEnviado);
        Assert.Equal(id, mediator.CommandEnviado!.AgendamentoId);
    }

    [Fact]
    public async Task BuscarTodosAgendamentosPaginados_Deve_Retornar_Ok_Quando_Mediator_Retornar_Sucesso()
    {
        // Arrange
        var mediator = new FakeMediator();

        var controller = new AgendamentoController(
            new InlineValidator<CreateAgendamentoRequest>(),
            mediator,
            NullLogger<AgendamentoController>.Instance);

        var pagination = new Pagination
        {
            PageNumber = 1,
            PageSize = 10
        };

        var agendamento = new AgendamentoUsuarioResponse
        {
            AgendamentoId = Guid.NewGuid().ToString(),
            PessoaId = Guid.NewGuid().ToString(),
            VeiculoId = Guid.NewGuid().ToString(),
            Nome = "João da Silva",
            Documento = "12345678900",
            Telefone = "17999999999",
            Placa = "ABC1D23",
            Marca = "Toyota",
            Modelo = "Corolla",
            AnoFabricacao = 2024,
            Cor = "Prata",
            Hodometro = 10000,
            Consultor = "Carlos",
            DtAgendamento = "26/08/2026",
            HorarioAgendamento = new TimeOnly(10, 0)
        };

        var pagedResponse =
            new PagedResponse<AgendamentoUsuarioResponse>
            {
                Items = new List<AgendamentoUsuarioResponse>
                {
                    agendamento
                },
                TotalCount = 1,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };

        mediator.ResultAgendamentos =
            Result<PagedResponse<AgendamentoUsuarioResponse>>.Success(
                pagedResponse);

        // Act
        var result = await controller.BuscarTodosAgendamentosPaginados(
            pagination,
            CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);

        Assert.Equal(
            StatusCodes.Status200OK,
            okResult.StatusCode);

        Assert.NotNull(okResult.Value);

        Assert.NotNull(mediator.QueryEnviada);

        Assert.Equal(
            pagination.PageNumber,
            mediator.QueryEnviada!.Pagination.PageNumber);

        Assert.Equal(
            pagination.PageSize,
            mediator.QueryEnviada.Pagination.PageSize);

        var response =
            Assert.IsType<ApiResponse<PagedResponse<AgendamentoUsuarioResponse>>>(
                okResult.Value);

        Assert.True(response.Success);
        Assert.NotNull(response.Data);

        Assert.Single(response.Data.Items);

        Assert.Equal(
            agendamento.AgendamentoId,
            response.Data.Items.First().AgendamentoId);

        Assert.Equal(
            agendamento.PessoaId,
            response.Data.Items.First().PessoaId);

        Assert.Equal(
            agendamento.VeiculoId,
            response.Data.Items.First().VeiculoId);
    }

    [Fact]
    public async Task BuscarTodosAgendamentosPaginados_Deve_Retornar_BadRequest_Quando_Mediator_Retornar_Falha()
    {
        // Arrange
        var mediator = new FakeMediator();

        var controller = new AgendamentoController(
            new InlineValidator<CreateAgendamentoRequest>(),
            mediator,
            NullLogger<AgendamentoController>.Instance);

        var pagination = new Pagination
        {
            PageNumber = 1,
            PageSize = 10
        };

        const string mensagemErro =
            "Não foi possível obter os agendamentos.";

        mediator.ResultAgendamentos =
            Result<PagedResponse<AgendamentoUsuarioResponse>>.Failure(
                mensagemErro);

        // Act
        var result = await controller.BuscarTodosAgendamentosPaginados(
            pagination,
            CancellationToken.None);

        // Assert
        var badRequestResult =
            Assert.IsType<BadRequestObjectResult>(result.Result);

        Assert.Equal(
            StatusCodes.Status400BadRequest,
            badRequestResult.StatusCode);

        Assert.NotNull(badRequestResult.Value);

        var response =
            Assert.IsType<ApiResponse>(badRequestResult.Value);

        Assert.False(response.Success);
        Assert.Equal(
            mensagemErro,
            response.Message);
    }

    [Fact]
    public async Task BuscarTodosAgendamentosPaginados_Deve_Usar_Mensagem_Padrao_Quando_Erro_For_Nulo()
    {
        // Arrange
        var mediator = new FakeMediator();

        var controller = new AgendamentoController(
            new InlineValidator<CreateAgendamentoRequest>(),
            mediator,
            NullLogger<AgendamentoController>.Instance);

        var pagination = new Pagination
        {
            PageNumber = 1,
            PageSize = 10
        };

        mediator.ResultAgendamentos =
            Result<PagedResponse<AgendamentoUsuarioResponse>>.Failure(
                (string?)null);

        // Act
        var result = await controller.BuscarTodosAgendamentosPaginados(
            pagination,
            CancellationToken.None);

        // Assert
        var badRequestResult =
            Assert.IsType<BadRequestObjectResult>(result.Result);

        Assert.Equal(
            StatusCodes.Status400BadRequest,
            badRequestResult.StatusCode);

        Assert.NotNull(badRequestResult.Value);

        var response =
            Assert.IsType<ApiResponse>(badRequestResult.Value);

        Assert.False(response.Success);

        Assert.Equal(
            "Não foi possível obter os agendamentos.",
            response.Message);
    }

    private sealed class FakeMediator : IMediator
    {
        public IniciarAgendamentoCommand? CommandEnviado { get; private set; }

        public GetAllAgendamentosPaginadosQuery? QueryEnviada { get; private set; }

        public Result<PagedResponse<AgendamentoUsuarioResponse>>?
            ResultAgendamentos
        {
            get;
            set;
        }

        public Task<TResponse> Send<TResponse>(
            IRequest<TResponse> request,
            CancellationToken cancellationToken = default)
        {
            if (request is IniciarAgendamentoCommand command)
            {
                CommandEnviado = command;

                return Task.FromResult(
                    (TResponse)(object)Result.Success());
            }

            if (request is GetAllAgendamentosPaginadosQuery query)
            {
                QueryEnviada = query;

                if (ResultAgendamentos is null)
                {
                    throw new InvalidOperationException(
                        "ResultAgendamentos precisa ser configurado no teste.");
                }

                return Task.FromResult(
                    (TResponse)(object)ResultAgendamentos);
            }

            throw new NotSupportedException(
                $"Request não suportado pelo FakeMediator: {request.GetType().Name}");
        }

        public Task<object?> Send(
            object request,
            CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public IAsyncEnumerable<TResponse> CreateStream<TResponse>(
            IStreamRequest<TResponse> request,
            CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public IAsyncEnumerable<object?> CreateStream(
            object request,
            CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task Publish(
            object notification,
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task Publish<TNotification>(
            TNotification notification,
            CancellationToken cancellationToken = default)
            where TNotification : INotification
        {
            return Task.CompletedTask;
        }

        public Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest
        {
            throw new NotImplementedException();
        }
    }
}