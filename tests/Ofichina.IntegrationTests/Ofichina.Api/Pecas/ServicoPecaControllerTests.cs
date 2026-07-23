using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using Microsoft.Extensions.Logging.Abstractions;
using Ofichina.Api.Controllers.ServicosPecas;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.Servicos;
using Ofichina.Application.UseCases.ServicosPecas.Commands;

namespace Ofichina.IntegrationTests.Ofichina.Api.ServicosPecas;

public sealed class ServicoPecaControllerTests
{
    [Fact]
    public async Task DesativarPeca_Deve_RetornarConflict_Quando_PecaJaEstiverUtilizada()
    {
        var mediator = new FakeMediator
        {
            DeleteServicoPecaResult = Result.Failure("Não é possível remover uma peça já utilizada.")
        };

        var controller = CriarController(mediator);

        var result = await controller.DesativarPeca(Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None);

        var conflictResult = Assert.IsType<ConflictObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status409Conflict, conflictResult.StatusCode);
        Assert.NotNull(mediator.DeleteServicoPecaCommandEnviado);
    }

    [Fact]
    public async Task DesativarPeca_Deve_RetornarBadRequest_Quando_Ocorre_Erro_Generico()
    {
        var mediator = new FakeMediator
        {
            DeleteServicoPecaResult = Result.Failure("Erro inesperado.")
        };

        var controller = CriarController(mediator);

        var result = await controller.DesativarPeca(Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
    }

    [Fact]
    public async Task DesativarPeca_Deve_RetornarNotFound_Quando_ServicoNaoForEncontrado()
    {
        var mediator = new FakeMediator
        {
            DeleteServicoPecaResult = Result.Failure("Serviço não encontrado.")
        };

        var controller = CriarController(mediator);

        var result = await controller.DesativarPeca(Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
    }

    [Fact]
    public async Task DesativarTodasAsPecas_Deve_RetornarBadRequest_Quando_Ocorre_Erro_Generico()
    {
        var mediator = new FakeMediator
        {
            DeleteAllServicoPecasResult = Result.Failure("Erro inesperado.")
        };

        var controller = CriarController(mediator);

        var result = await controller.DesativarTodasAsPecas(Guid.NewGuid(), CancellationToken.None);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
    }

    [Fact]
    public async Task DesativarTodasAsPecas_Deve_RetornarConflict_Quando_Existir_PecaUtilizada()
    {
        var mediator = new FakeMediator
        {
            DeleteAllServicoPecasResult = Result.Failure("Não é possível remover uma peça já utilizada.")
        };

        var controller = CriarController(mediator);

        var result = await controller.DesativarTodasAsPecas(Guid.NewGuid(), CancellationToken.None);

        var conflictResult = Assert.IsType<ConflictObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status409Conflict, conflictResult.StatusCode);
        Assert.NotNull(mediator.DeleteAllServicoPecasCommandEnviado);
    }

    [Fact]
    public async Task DesativarTodasAsPecas_Deve_RetornarNotFound_Quando_ServicoNaoForEncontrado()
    {
        var mediator = new FakeMediator
        {
            DeleteAllServicoPecasResult = Result.Failure("Serviço não encontrado.")
        };

        var controller = CriarController(mediator);

        var result = await controller.DesativarTodasAsPecas(Guid.NewGuid(), CancellationToken.None);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
    }

    [Fact]
    public async Task DesativarPeca_Deve_RetornarOk_Quando_Operacao_ForBemSucedida()
    {
        var mediator = new FakeMediator();
        var controller = CriarController(mediator);

        var result = await controller.DesativarPeca(Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
    }

    private sealed class FakeMediator : IMediator
    {
        public DeleteServicoPecaCommand? DeleteServicoPecaCommandEnviado { get; private set; }

        public DeleteAllServicoPecasCommand? DeleteAllServicoPecasCommandEnviado { get; private set; }

        public Result DeleteServicoPecaResult { get; set; } = Result.Success();

        public Result DeleteAllServicoPecasResult { get; set; } = Result.Success();

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            if (request is DeleteServicoPecaCommand deleteCommand)
            {
                DeleteServicoPecaCommandEnviado = deleteCommand;
                return Task.FromResult((TResponse)(object)DeleteServicoPecaResult);
            }

            if (request is DeleteAllServicoPecasCommand deleteAllCommand)
            {
                DeleteAllServicoPecasCommandEnviado = deleteAllCommand;
                return Task.FromResult((TResponse)(object)DeleteAllServicoPecasResult);
            }

            throw new NotSupportedException();
        }

        Task ISender.Send<TRequest>(TRequest request, CancellationToken cancellationToken)
        {
            if (request is DeleteServicoPecaCommand deleteCommand)
            {
                DeleteServicoPecaCommandEnviado = deleteCommand;
                return Task.CompletedTask;
            }

            if (request is DeleteAllServicoPecasCommand deleteAllCommand)
            {
                DeleteAllServicoPecasCommandEnviado = deleteAllCommand;
                return Task.CompletedTask;
            }

            throw new NotSupportedException();
        }

        public Task<TResponse> Send<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest<TResponse>
            => request is DeleteServicoPecaCommand deleteCommand
                ? Task.FromResult((TResponse)(object)DeleteServicoPecaResult)
                : request is DeleteAllServicoPecasCommand deleteAllCommand
                    ? Task.FromResult((TResponse)(object)DeleteAllServicoPecasResult)
                    : throw new NotSupportedException();

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

    private static ServicosPecasController CriarController(IMediator mediator)
    {
        var validator = new InlineValidator<CreateServicoPecaRequest>();
        return new ServicosPecasController(validator, mediator, NullLogger<ServicosPecasController>.Instance);
    }
}