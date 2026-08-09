using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Ofichina.Api.Controllers.ItensServico;
using Ofichina.Application.UseCases.ItensServico.Commands;
using Ofichina.Application.UseCases.ItensServico.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.ItensServico;
using Ofichina.Contracts.Responses.Orcamento;
using Ofichina.Contracts.Responses.OrdemServico;

namespace Ofichina.IntegrationTests.Api.Controllers.ItemServico;

public sealed class ItemServicoControllerTests
{
    [Fact]
    public async Task BuscarItensOrcamento_Deve_Enviar_Query_Com_OrcamentoId()
    {
        var mediator = new FakeMediator();
        var controller = new ItemServicoController(
            new InlineValidator<CreateItemServicoRequest>(),
            new InlineValidator<CreateItemOrcamentoRequest>(),
            new InlineValidator<UpdateItemOrcamentoRequest>(),
            new InlineValidator<UpdateItemServicoRequest>(),
            mediator,
            NullLogger<ItemServicoController>.Instance);

        var orcamentoId = Guid.NewGuid();
        var result = await controller.BuscarItensOrcamento(orcamentoId, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.NotNull(mediator.QueryEnviada);
        Assert.Equal(orcamentoId, mediator.QueryEnviada!.OrcamentoId);
    }

    [Fact]
    public async Task BuscarItemServicoOrcamentoPorId_Deve_Enviar_Query_Com_Ids()
    {
        var mediator = new FakeMediator();
        var controller = new ItemServicoController(
            new InlineValidator<CreateItemServicoRequest>(),
            new InlineValidator<CreateItemOrcamentoRequest>(),
            new InlineValidator<UpdateItemOrcamentoRequest>(),
            new InlineValidator<UpdateItemServicoRequest>(),
            mediator,
            NullLogger<ItemServicoController>.Instance);

        var orcamentoId = Guid.NewGuid();
        var itemServicoId = Guid.NewGuid();
        var result = await controller.BuscarItemServicoOrcamentoPorId(orcamentoId, itemServicoId, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.NotNull(mediator.QueryPorIdEnviada);
        Assert.Equal(orcamentoId, mediator.QueryPorIdEnviada!.OrcamentoId);
        Assert.Equal(itemServicoId, mediator.QueryPorIdEnviada.ItemServicoId);
    }

    [Fact]
    public async Task AtualizarItemOrcamento_Deve_Enviar_Comando_Com_Ids_E_Quantidade()
    {
        var mediator = new FakeMediator();
        var controller = new ItemServicoController(
            new InlineValidator<CreateItemServicoRequest>(),
            new InlineValidator<CreateItemOrcamentoRequest>(),
            new InlineValidator<UpdateItemOrcamentoRequest>(),
            new InlineValidator<UpdateItemServicoRequest>(),
            mediator,
            NullLogger<ItemServicoController>.Instance);

        var orcamentoId = Guid.NewGuid();
        var itemServicoId = Guid.NewGuid();
        var servicoId = Guid.NewGuid();

        var result = await controller.AtualizarItemOrcamento(new UpdateItemOrcamentoRequest
        {
            ItemServicoId = itemServicoId,
            OrcamentoId = orcamentoId,
            ServicoId = servicoId,
            PecaId = null,
            Quantidade = 3
        }, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.NotNull(mediator.UpdateItemOrcamentoCommandEnviado);
        Assert.Equal(orcamentoId, mediator.UpdateItemOrcamentoCommandEnviado!.OrcamentoId);
        Assert.Equal(itemServicoId, mediator.UpdateItemOrcamentoCommandEnviado.ItemServicoId);
        Assert.Equal(servicoId, mediator.UpdateItemOrcamentoCommandEnviado.ServicoId);
        Assert.Equal(3, mediator.UpdateItemOrcamentoCommandEnviado.Quantidade);
    }

    private sealed class FakeMediator : IMediator
    {
        public GetItemServicosByOrcamentoQuery? QueryEnviada { get; private set; }
        public GetItemServicoByOrcamentoIdQuery? QueryPorIdEnviada { get; private set; }
        public UpdateItemOrcamentoCommand? UpdateItemOrcamentoCommandEnviado { get; private set; }

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            if (request is GetItemServicosByOrcamentoQuery query)
            {
                QueryEnviada = query;
                return Task.FromResult((TResponse)(object)Result.Success<IReadOnlyCollection<OrcamentoItemResponse>>([]));
            }

            if (request is GetItemServicoByOrcamentoIdQuery queryById)
            {
                QueryPorIdEnviada = queryById;
                return Task.FromResult((TResponse)(object)Result.Success(new OrcamentoItemResponse()));
            }

            if (request is UpdateItemOrcamentoCommand command)
            {
                UpdateItemOrcamentoCommandEnviado = command;
                return Task.FromResult((TResponse)(object)Result.Success());
            }

            throw new NotSupportedException();
        }

        public Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default)
            where TRequest : IRequest
        {
            if (request is GetItemServicosByOrcamentoQuery query)
            {
                QueryEnviada = query;
                return Task.CompletedTask;
            }

            if (request is GetItemServicoByOrcamentoIdQuery queryById)
            {
                QueryPorIdEnviada = queryById;
                return Task.CompletedTask;
            }

            if (request is UpdateItemOrcamentoCommand command)
            {
                UpdateItemOrcamentoCommandEnviado = command;
                return Task.CompletedTask;
            }

            throw new NotSupportedException();
        }

        Task ISender.Send<TRequest>(TRequest request, CancellationToken cancellationToken)
        {
            if (request is GetItemServicosByOrcamentoQuery query)
            {
                QueryEnviada = query;
                return Task.CompletedTask;
            }

            if (request is GetItemServicoByOrcamentoIdQuery queryById)
            {
                QueryPorIdEnviada = queryById;
                return Task.CompletedTask;
            }

            if (request is UpdateItemOrcamentoCommand command)
            {
                UpdateItemOrcamentoCommandEnviado = command;
                return Task.CompletedTask;
            }

            throw new NotSupportedException();
        }

        Task<TResponse> ISender.Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken)
            => Send(request, cancellationToken);

        public Task<TResponse> Send<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest<TResponse>
            => Send((IRequest<TResponse>)request, cancellationToken);

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
