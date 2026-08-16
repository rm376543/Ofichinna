using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Ofichina.Api.Controllers.Pecas;
using Ofichina.Application.UseCases.Pecas.Commands;
using Ofichina.Application.UseCases.Pecas.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.Pecas;
using Ofichina.Contracts.Responses.Pecas;

namespace Ofichina.IntegrationTests.Api.Controllers.Pecas;

public sealed class PecaControllerTests
{
    [Fact]
    public async Task CriarPeca_Deve_Enviar_Command_E_Retornar_Sucesso()
    {
        var mediator = new FakeMediator();
        var controller = new PecaController(
            CriarCreateValidator(),
            CriarUpdateValidator(),
            mediator,
            NullLogger<PecaController>.Instance);

        var result = await controller.CriarPeca(new CreatePecaRequest
        {
            Nome = "Filtro de óleo",
            Descricao = "Filtro principal",
            Codigo = "FILTRO-001",
            Valor = 59.90m,
            QuantidadeEstoque = 10
        }, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(ok.Value);
        Assert.True(response.Success);
        Assert.Equal("Peça criada com sucesso.", response.Message);
        Assert.IsType<CreatePecaCommand>(mediator.UltimoRequest);
    }

    [Fact]
    public async Task CriarPeca_Deve_Rejeitar_Dados_Invalidos()
    {
        var mediator = new FakeMediator();
        var controller = new PecaController(
            CriarCreateValidator(),
            CriarUpdateValidator(),
            mediator,
            NullLogger<PecaController>.Instance);

        var result = await controller.CriarPeca(new CreatePecaRequest
        {
            Nome = string.Empty,
            Descricao = string.Empty,
            Codigo = string.Empty,
            Valor = 0m,
            QuantidadeEstoque = 0
        }, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Null(mediator.UltimoRequest);
    }

    [Fact]
    public async Task AtualizarPeca_Deve_Enviar_Command_E_Retornar_Sucesso()
    {
        var mediator = new FakeMediator();
        var controller = new PecaController(
            CriarCreateValidator(),
            CriarUpdateValidator(),
            mediator,
            NullLogger<PecaController>.Instance);

        var id = Guid.NewGuid();
        var result = await controller.AtualizarPeca(new UpdatePecaRequest
        {
            PecaId = id,
            Nome = "Filtro de óleo",
            Descricao = "Filtro principal",
            Codigo = "FILTRO-001",
            Valor = 59.90m,
            QuantidadeEstoque = 10
        }, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(ok.Value);
        Assert.True(response.Success);
        Assert.Equal("Peça atualizada com sucesso.", response.Message);
        var command = Assert.IsType<UpdatePecaCommand>(mediator.UltimoRequest);
        Assert.Equal(id, command.PecaId);
    }

    [Fact]
    public async Task BuscarPecaPorId_Deve_Retornar_NotFound_Quando_Nao_Existir()
    {
        var mediator = new FakeMediator { GetPecaByIdResult = Result.Failure<PecaResponse>("Peça não encontrada.") };
        var controller = new PecaController(
            CriarCreateValidator(),
            CriarUpdateValidator(),
            mediator,
            NullLogger<PecaController>.Instance);

        var result = await controller.BuscarPecaPorId(Guid.NewGuid(), CancellationToken.None);

        var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(notFound.Value);
        Assert.False(response.Success);
        Assert.Equal("Peça não encontrada.", response.Message);
    }

    [Fact]
    public async Task BuscarTodasPecasPaginadas_Deve_Retornar_Itens_Paginados()
    {
        var mediator = new FakeMediator
        {
            GetAllPecasResult = Result.Success(new PagedResponse<PecaResponse>
            {
                Items =
                [
                    new PecaResponse { PecaId = Guid.NewGuid(), Nome = "Filtro", Codigo = "F-1", Valor = 59.90m, QuantidadeEstoque = 10 },
                    new PecaResponse { PecaId = Guid.NewGuid(), Nome = "Velas", Codigo = "V-1", Valor = 25m, QuantidadeEstoque = 20 }
                ],
                PageNumber = 1,
                PageSize = 10,
                TotalCount = 2,
                TotalPages = 1,
                HasNextPage = false,
                HasPreviousPage = false
            })
        };

        var controller = new PecaController(
            CriarCreateValidator(),
            CriarUpdateValidator(),
            mediator,
            NullLogger<PecaController>.Instance);

        var result = await controller.BuscarTodasPecasPaginadas(new Pagination(1, 10), CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<PagedResponse<PecaResponse>>>(ok.Value);
        Assert.True(response.Success);
        Assert.Equal(2, response.Data!.Items.Count);
        Assert.IsType<GetAllPecasPaginadasQuery>(mediator.UltimoRequest);
    }

    [Fact]
    public async Task DeletarPeca_Deve_Enviar_Command_E_Retornar_Sucesso()
    {
        var mediator = new FakeMediator();
        var controller = new PecaController(
            CriarCreateValidator(),
            CriarUpdateValidator(),
            mediator,
            NullLogger<PecaController>.Instance);

        var id = Guid.NewGuid();
        var result = await controller.DeletarPeca(new RemovePecaRequest { PecaId = id }, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(ok.Value);
        Assert.True(response.Success);
        Assert.Equal("Peça removida com sucesso.", response.Message);
        var command = Assert.IsType<DeletePecaCommand>(mediator.UltimoRequest);
        Assert.Equal(id, command.PecaId);
    }

    private static InlineValidator<CreatePecaRequest> CriarCreateValidator()
    {
        var validator = new InlineValidator<CreatePecaRequest>();
        validator.RuleFor(x => x.Nome).NotEmpty();
        validator.RuleFor(x => x.Codigo).NotEmpty();
        validator.RuleFor(x => x.Valor).GreaterThan(0);
        validator.RuleFor(x => x.QuantidadeEstoque).GreaterThan(0);
        return validator;
    }

    private static InlineValidator<UpdatePecaRequest> CriarUpdateValidator()
    {
        var validator = new InlineValidator<UpdatePecaRequest>();
        validator.RuleFor(x => x.PecaId).NotEmpty();
        validator.RuleFor(x => x.Nome).NotEmpty();
        validator.RuleFor(x => x.Codigo).NotEmpty();
        validator.RuleFor(x => x.Valor).GreaterThan(0);
        validator.RuleFor(x => x.QuantidadeEstoque).GreaterThan(0);
        return validator;
    }

    private sealed class FakeMediator : IMediator
    {
        public object? UltimoRequest { get; private set; }

        public Result<PecaResponse> GetPecaByIdResult { get; set; } = Result.Success(new PecaResponse());

        public Result<PagedResponse<PecaResponse>> GetAllPecasResult { get; set; } = Result.Success(new PagedResponse<PecaResponse>());

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            UltimoRequest = request;

            object response = request switch
            {
                CreatePecaCommand => Result.Success(),
                UpdatePecaCommand => Result.Success(),
                DeletePecaCommand => Result.Success(),
                GetPecaByIdQuery => GetPecaByIdResult,
                GetAllPecasPaginadasQuery => GetAllPecasResult,
                _ => throw new NotSupportedException()
            };

            return Task.FromResult((TResponse)response);
        }

        Task ISender.Send<TRequest>(TRequest request, CancellationToken cancellationToken)
        {
            UltimoRequest = request;
            return Task.CompletedTask;
        }

        public Task<TResponse> Send<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest<TResponse>
            => Send((IRequest<TResponse>)request!, cancellationToken);

        public Task<object?> Send(object request, CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request, CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public IAsyncEnumerable<object?> CreateStream(object request, CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public Task Publish(object notification, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification
            => Task.CompletedTask;
    }
}