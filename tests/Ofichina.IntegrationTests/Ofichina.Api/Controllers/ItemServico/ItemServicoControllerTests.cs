using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Ofichina.Api.Controllers.ItensServico;
using Ofichina.Application.UseCases.ItensServico.Commands;
using Ofichina.Application.Validators.OrdensServico;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.ItensServico;

namespace Ofichina.IntegrationTests.Api.Controllers.ItensServico;

public sealed class ItemServicoControllerTests
{
    [Fact]
    public async Task AtualizarItemServico_Deve_RetornarNotFound_Quando_ServicoPecaNaoForEncontrada()
    {
        var mediator = new FakeMediator
        {
            UpdateItemServicoResult = Result.Failure("Peça de serviço não encontrada.")
        };

        var controller = CriarController(mediator);
        var request = new UpdateItemServicoRequest
        {
            Id = Guid.NewGuid(),
            OrdemServicoId = Guid.NewGuid(),
            ServicoPecaId = Guid.NewGuid()
        };

        var result = await controller.AtualizarItemServico(request, CancellationToken.None);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        Assert.NotNull(mediator.UpdateItemServicoCommandEnviado);
        Assert.Equal(request.Id, mediator.UpdateItemServicoCommandEnviado!.Id);
        Assert.Equal(request.OrdemServicoId, mediator.UpdateItemServicoCommandEnviado.OrdemServicoId);
        Assert.Equal(request.ServicoPecaId, mediator.UpdateItemServicoCommandEnviado.ServicoPecaId);
    }

    [Fact]
    public async Task AtualizarItemServico_Deve_RetornarNotFound_Quando_ItemNaoForEncontrado()
    {
        var mediator = new FakeMediator
        {
            UpdateItemServicoResult = Result.Failure("Item de serviço não encontrado.")
        };

        var controller = CriarController(mediator);
        var request = new UpdateItemServicoRequest
        {
            Id = Guid.NewGuid(),
            OrdemServicoId = Guid.NewGuid(),
            ServicoPecaId = Guid.NewGuid()
        };

        var result = await controller.AtualizarItemServico(request, CancellationToken.None);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
    }

    [Fact]
    public async Task AtualizarItemServico_Deve_RetornarBadRequest_Quando_DadosForem_Invalidos()
    {
        var mediator = new FakeMediator();
        var controller = CriarController(mediator);
        var request = new UpdateItemServicoRequest
        {
            Id = Guid.NewGuid(),
            OrdemServicoId = Guid.Empty,
            ServicoPecaId = Guid.Empty
        };

        var result = await controller.AtualizarItemServico(request, CancellationToken.None);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        Assert.Null(mediator.UpdateItemServicoCommandEnviado);
    }

    private static ItemServicoController CriarController(FakeMediator mediator)
    {
        return new ItemServicoController(
            new CreateItemServicoRequestValidator(),
            new UpdateItemServicoRequestValidator(),
            mediator,
            NullLogger<ItemServicoController>.Instance);
    }

    private sealed class FakeMediator : IMediator
    {
        public CreateItemServicoCommand? CreateItemServicoCommandEnviado { get; private set; }

        public UpdateItemServicoCommand? UpdateItemServicoCommandEnviado { get; private set; }

        public Result<Guid> CreateItemServicoResult { get; set; } = Result.Success(Guid.NewGuid());

        public Result UpdateItemServicoResult { get; set; } = Result.Success();

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            if (request is CreateItemServicoCommand createCommand)
            {
                CreateItemServicoCommandEnviado = createCommand;
                return Task.FromResult((TResponse)(object)CreateItemServicoResult);
            }

            if (request is UpdateItemServicoCommand updateCommand)
            {
                UpdateItemServicoCommandEnviado = updateCommand;
                return Task.FromResult((TResponse)(object)UpdateItemServicoResult);
            }

            throw new NotSupportedException();
        }

        Task ISender.Send<TRequest>(TRequest request, CancellationToken cancellationToken)
        {
            if (request is CreateItemServicoCommand createCommand)
            {
                CreateItemServicoCommandEnviado = createCommand;
                return Task.CompletedTask;
            }

            if (request is UpdateItemServicoCommand updateCommand)
            {
                UpdateItemServicoCommandEnviado = updateCommand;
                return Task.CompletedTask;
            }

            throw new NotSupportedException();
        }

        public Task<TResponse> Send<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
            where TRequest : IRequest<TResponse>
            => request is CreateItemServicoCommand createCommand
                ? Task.FromResult((TResponse)(object)CreateItemServicoResult)
                : request is UpdateItemServicoCommand updateCommand
                    ? Task.FromResult((TResponse)(object)UpdateItemServicoResult)
                    : throw new NotSupportedException();

        public Task<object?> Send(object request, CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request, CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public IAsyncEnumerable<object?> CreateStream(object request, CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public Task Publish(object notification, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
            where TNotification : INotification
            => Task.CompletedTask;
    }
}