using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Ofichina.Api.Controllers.Checklist;
using Ofichina.Application.UseCases.Checklists.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.Checklist;

namespace Ofichina.IntegrationTests.Api.Controllers.Checklist;

public sealed class ChecklistControllerTests
{
    [Fact]
    public async Task CriarChecklist_Deve_Enviar_Payload_Reduzido()
    {
        var mediator = new FakeMediator();
        var controller = new ChecklistController(
            mediator,
            NullLogger<ChecklistController>.Instance);

        var request = new CreateChecklistRequest
        {
            AgendamentoId = Guid.NewGuid(),
            ItensVerificados = "Luzes, freios e pneus",
            Observacoes = "Checklist inicial"
        };

        var result = await controller.CriarChecklist(request, CancellationToken.None);

        var createdResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
        Assert.NotNull(mediator.CreateCommandEnviado);
        Assert.Equal(request.AgendamentoId, mediator.CreateCommandEnviado!.AgendamentoId);
        Assert.Equal(request.ItensVerificados, mediator.CreateCommandEnviado.ItensVerificados);
        Assert.Equal(request.Observacoes, mediator.CreateCommandEnviado.Observacoes);
    }

    [Fact]
    public async Task FinalizarChecklist_Deve_Enviar_Id()
    {
        var mediator = new FakeMediator();
        var controller = new ChecklistController(
            mediator,
            NullLogger<ChecklistController>.Instance);

        var id = Guid.NewGuid();
        var result = await controller.FinalizarChecklist(new FinalizarChecklistRequest { AgendamentoId = id }, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.NotNull(mediator.FinalizarCommandEnviado);
        Assert.Equal(id, mediator.FinalizarCommandEnviado!.AgendamentoId);
    }

    private sealed class FakeMediator : IMediator
    {
        public CreateChecklistCommand? CreateCommandEnviado { get; private set; }
        public FinalizarChecklistCommand? FinalizarCommandEnviado { get; private set; }

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            if (request is CreateChecklistCommand createCommand)
            {
                CreateCommandEnviado = createCommand;
                return Task.FromResult((TResponse)(object)Result.Success());
            }

            if (request is FinalizarChecklistCommand finalizarCommand)
            {
                FinalizarCommandEnviado = finalizarCommand;
                return Task.FromResult((TResponse)(object)Result.Success());
            }

            throw new NotSupportedException();
        }

        Task ISender.Send<TRequest>(TRequest request, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task<TResponse> Send<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest<TResponse>
            => throw new NotSupportedException();

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
