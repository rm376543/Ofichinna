using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Ofichina.Api.Controllers.Orcamento;
using Ofichina.Application.UseCases.Orcamentos.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.Orcamento;
namespace Ofichina.IntegrationTests.Api.Controllers.Orcamento;

public sealed class OrcamentoControllerTests
{
    [Fact]
    public async Task AtualizarDescontoOrcamento_Deve_Enviar_Comando_Com_Valor_Do_Desconto()
    {
        var mediator = new FakeMediator();
        var controller = new OrcamentoController(
            new InlineValidator<CreateOrcamentoRequest>(),
            new InlineValidator<UpdateOrcamentoRequest>(),
            new InlineValidator<UpdateOrcamentoDescontoRequest>(),
            mediator,
            NullLogger<OrcamentoController>.Instance);

        var orcamentoId = Guid.NewGuid();
        var result = await controller.AtualizarDescontoOrcamento(
            new UpdateOrcamentoDescontoRequest { OrcamentoId = orcamentoId, Desconto = 18m, DescontoEmDinheiro = false },
            CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.NotNull(mediator.CommandEnviado);
        Assert.Equal(orcamentoId, mediator.CommandEnviado!.OrcamentoId);
        Assert.Equal(18m, mediator.CommandEnviado.Desconto);
        Assert.False(mediator.CommandEnviado.DescontoEmDinheiro);
    }

    [Fact]
    public async Task AprovarOrcamento_Deve_Enviar_Comando_Com_Hodometro()
    {
        var mediator = new FakeMediator();
        var controller = new OrcamentoController(
            new InlineValidator<CreateOrcamentoRequest>(),
            new InlineValidator<UpdateOrcamentoRequest>(),
            new InlineValidator<UpdateOrcamentoDescontoRequest>(),
            mediator,
            NullLogger<OrcamentoController>.Instance);

        var orcamentoId = Guid.NewGuid();
        var result = await controller.AprovarOrcamento(
            new AprovarOrcamentoRequest(orcamentoId),
            CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.NotNull(mediator.AprovarCommandEnviado);
        Assert.Equal(orcamentoId, mediator.AprovarCommandEnviado!.Id);
    }

    private sealed class FakeMediator : IMediator
    {
        public UpdateOrcamentoDescontoCommand? CommandEnviado { get; private set; }
        public AprovarOrcamentoCommand? AprovarCommandEnviado { get; private set; }

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            if (request is UpdateOrcamentoDescontoCommand command)
            {
                CommandEnviado = command;
                return Task.FromResult((TResponse)(object)Result.Success());
            }

            if (request is AprovarOrcamentoCommand aprovarCommand)
            {
                AprovarCommandEnviado = aprovarCommand;
                return Task.FromResult((TResponse)(object)Result.Success());
            }

            throw new NotSupportedException();
        }

        Task ISender.Send<TRequest>(TRequest request, CancellationToken cancellationToken)
        {
            if (request is UpdateOrcamentoDescontoCommand command)
            {
                CommandEnviado = command;
                return Task.CompletedTask;
            }

            if (request is AprovarOrcamentoCommand aprovarCommand)
            {
                AprovarCommandEnviado = aprovarCommand;
                return Task.CompletedTask;
            }

            throw new NotSupportedException();
        }

        public Task<TResponse> Send<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest<TResponse>
            => request is UpdateOrcamentoDescontoCommand
                ? Task.FromResult((TResponse)(object)Result.Success())
                : request is AprovarOrcamentoCommand
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