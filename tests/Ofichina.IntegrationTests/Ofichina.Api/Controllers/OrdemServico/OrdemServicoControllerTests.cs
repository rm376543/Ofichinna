using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Ofichina.Api.Controllers.OrdensServico;
using Ofichina.Application.UseCases.OrdensServico.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Enums;

namespace Ofichina.IntegrationTests.Api.Controllers.OrdensServico;

public sealed class OrdemServicoControllerTests
{
    [Fact]
    public async Task IniciarExecucaoOrdemServico_Deve_Enviar_Status_EmExecucao()
    {
        var mediator = new FakeMediator();
        var controller = new OrdemServicoController(
            mediator,
            NullLogger<OrdemServicoController>.Instance);

        var id = Guid.NewGuid();
        var result = await controller.IniciarExecucaoOrdemServico(id, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.NotNull(mediator.CommandEnviado);
        Assert.Equal(id, mediator.CommandEnviado!.Id);
        Assert.Equal(StatusOrdemServico.EmExecucao, mediator.CommandEnviado.StatusDestino);
    }

    [Fact]
    public async Task CancelarOrdemServico_Deve_Enviar_Status_Cancelada()
    {
        var mediator = new FakeMediator();
        var controller = new OrdemServicoController(
            mediator,
            NullLogger<OrdemServicoController>.Instance);

        var id = Guid.NewGuid();
        var result = await controller.CancelarOrdemServico(id, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.NotNull(mediator.CommandEnviado);
        Assert.Equal(id, mediator.CommandEnviado!.Id);
        Assert.Equal(StatusOrdemServico.Cancelada, mediator.CommandEnviado.StatusDestino);
    }

    private sealed class FakeMediator : IMediator
    {
        public AlterarStatusOrdemServicoCommand? CommandEnviado { get; private set; }

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            if (request is AlterarStatusOrdemServicoCommand command)
            {
                CommandEnviado = command;
                return Task.FromResult((TResponse)(object)Result.Success());
            }

            throw new NotSupportedException();
        }

        Task ISender.Send<TRequest>(TRequest request, CancellationToken cancellationToken)
        {
            if (request is AlterarStatusOrdemServicoCommand command)
            {
                CommandEnviado = command;
                return Task.CompletedTask;
            }

            throw new NotSupportedException();
        }

        public Task<TResponse> Send<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest<TResponse>
            => request is AlterarStatusOrdemServicoCommand command
                ? Task.FromResult((TResponse)(object)Result.Success())
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
}
