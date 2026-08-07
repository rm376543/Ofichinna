using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Ofichina.Api.Controllers.Agendamento;
using Ofichina.Application.UseCases.Agendamentos.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.Agendamento;

namespace Ofichina.IntegrationTests.Api.Controllers.Agendamento;

public sealed class AgendamentoControllerTests
{
    [Fact]
    public async Task IniciarAsync_Deve_Enviar_Id_Do_Agendamento()
    {
        var mediator = new FakeMediator();
        var controller = new AgendamentoController(
            new InlineValidator<CreateAgendamentoRequest>(),
            mediator,
            NullLogger<AgendamentoController>.Instance);

        var id = Guid.NewGuid();
        var result = await controller.IniciarAsync(id, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.NotNull(mediator.CommandEnviado);
        Assert.Equal(id, mediator.CommandEnviado!.AgendamentoId);
    }

    private sealed class FakeMediator : IMediator
    {
        public IniciarAgendamentoCommand? CommandEnviado { get; private set; }

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            if (request is IniciarAgendamentoCommand command)
            {
                CommandEnviado = command;
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
