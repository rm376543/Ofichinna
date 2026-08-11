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
using Ofichina.Contracts.Responses;
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
            new InlineValidator<CreateServicoOrcamentoRequest>(),
            new InlineValidator<CreateServicoOrdemServicoRequest>(),
            new InlineValidator<UpdateItemOrcamentoRequest>(),
            new InlineValidator<UpdateServicoOrcamentoRequest>(),
            new InlineValidator<UpdateItemServicoRequest>(),
            new InlineValidator<UpdateServicoOrdemServicoRequest>(),
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
    public async Task BuscarItensOrcamento_Deve_Preservar_Agrupamento_De_Servicos_E_Pecas()
    {
        var orcamentoId = Guid.NewGuid();
        var responseEsperada = new List<OrcamentoItemResponse>
        {
            new()
            {
                OrcamentoId = orcamentoId,
                Servicos =
                [
                    new ServicoItemResponse
                    {
                        ServicoId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                        Descricao = "Troca de óleo",
                        ValorServico = 120m,
                        ValorTotal = 210m,
                        Pecas =
                        [
                            new PecaItemResponse
                            {
                                PecaId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                                Descricao = "Filtro de óleo",
                                Quantidade = 1,
                                ValorUnitario = 60m,
                                ValorTotal = 60m
                            },
                            new PecaItemResponse
                            {
                                PecaId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                                Descricao = "Anel de vedação",
                                Quantidade = 2,
                                ValorUnitario = 15m,
                                ValorTotal = 30m
                            }
                        ]
                    },
                    new ServicoItemResponse
                    {
                        ServicoId = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                        Descricao = "Alinhamento",
                        ValorServico = 80m,
                        ValorTotal = 80m,
                        Pecas = []
                    }
                ]
            }
        };

        var mediator = new FakeMediator
        {
            ItensOrcamentoResponse = responseEsperada
        };
        var controller = new ItemServicoController(
            new InlineValidator<CreateItemServicoRequest>(),
            new InlineValidator<CreateItemOrcamentoRequest>(),
            new InlineValidator<CreateServicoOrcamentoRequest>(),
            new InlineValidator<CreateServicoOrdemServicoRequest>(),
            new InlineValidator<UpdateItemOrcamentoRequest>(),
            new InlineValidator<UpdateServicoOrcamentoRequest>(),
            new InlineValidator<UpdateItemServicoRequest>(),
            new InlineValidator<UpdateServicoOrdemServicoRequest>(),
            mediator,
            NullLogger<ItemServicoController>.Instance);

        var result = await controller.BuscarItensOrcamento(orcamentoId, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);

        var response = Assert.IsType<ApiResponse<IReadOnlyCollection<OrcamentoItemResponse>>>(okResult.Value);
        Assert.True(response.Success);
        Assert.Single(response.Data!);

        var orcamentoResponse = response.Data!.Single();
        Assert.Equal(orcamentoId, orcamentoResponse.OrcamentoId);
        Assert.Equal(2, orcamentoResponse.Servicos.Count);

        var trocaOleo = Assert.Single(orcamentoResponse.Servicos.Where(x => x.Descricao == "Troca de óleo"));
        Assert.Equal(210m, trocaOleo.ValorTotal);
        Assert.Equal(2, trocaOleo.Pecas.Count);

        var alinhamento = Assert.Single(orcamentoResponse.Servicos.Where(x => x.Descricao == "Alinhamento"));
        Assert.Empty(alinhamento.Pecas);
        Assert.Equal(80m, alinhamento.ValorTotal);
    }

    [Fact]
    public async Task BuscarItemServicoOrcamentoPorId_Deve_Enviar_Query_Com_Ids()
    {
        var mediator = new FakeMediator();
        var controller = new ItemServicoController(
            new InlineValidator<CreateItemServicoRequest>(),
            new InlineValidator<CreateItemOrcamentoRequest>(),
            new InlineValidator<CreateServicoOrcamentoRequest>(),
            new InlineValidator<CreateServicoOrdemServicoRequest>(),
            new InlineValidator<UpdateItemOrcamentoRequest>(),
            new InlineValidator<UpdateServicoOrcamentoRequest>(),
            new InlineValidator<UpdateItemServicoRequest>(),
            new InlineValidator<UpdateServicoOrdemServicoRequest>(),
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
            new InlineValidator<CreateServicoOrcamentoRequest>(),
            new InlineValidator<CreateServicoOrdemServicoRequest>(),
            new InlineValidator<UpdateItemOrcamentoRequest>(),
            new InlineValidator<UpdateServicoOrcamentoRequest>(),
            new InlineValidator<UpdateItemServicoRequest>(),
            new InlineValidator<UpdateServicoOrdemServicoRequest>(),
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

    [Fact]
    public async Task CriarServicoOrcamento_Deve_Enviar_Comando_Com_Ids()
    {
        var mediator = new FakeMediator();
        var controller = new ItemServicoController(
            new InlineValidator<CreateItemServicoRequest>(),
            new InlineValidator<CreateItemOrcamentoRequest>(),
            new InlineValidator<CreateServicoOrcamentoRequest>(),
            new InlineValidator<CreateServicoOrdemServicoRequest>(),
            new InlineValidator<UpdateItemOrcamentoRequest>(),
            new InlineValidator<UpdateServicoOrcamentoRequest>(),
            new InlineValidator<UpdateItemServicoRequest>(),
            new InlineValidator<UpdateServicoOrdemServicoRequest>(),
            mediator,
            NullLogger<ItemServicoController>.Instance);

        var orcamentoId = Guid.NewGuid();
        var servicoId = Guid.NewGuid();

        var result = await controller.CriarServicoOrcamento(new CreateServicoOrcamentoRequest
        {
            OrcamentoId = orcamentoId,
            ServicoId = servicoId
        }, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.NotNull(mediator.CreateServicoOrcamentoCommandEnviado);
        Assert.Equal(orcamentoId, mediator.CreateServicoOrcamentoCommandEnviado!.OrcamentoId);
        Assert.Equal(servicoId, mediator.CreateServicoOrcamentoCommandEnviado.ServicoId);
    }

    [Fact]
    public async Task AtualizarServicoOrcamento_Deve_Enviar_Comando_Com_Ids()
    {
        var mediator = new FakeMediator();
        var controller = new ItemServicoController(
            new InlineValidator<CreateItemServicoRequest>(),
            new InlineValidator<CreateItemOrcamentoRequest>(),
            new InlineValidator<CreateServicoOrcamentoRequest>(),
            new InlineValidator<CreateServicoOrdemServicoRequest>(),
            new InlineValidator<UpdateItemOrcamentoRequest>(),
            new InlineValidator<UpdateServicoOrcamentoRequest>(),
            new InlineValidator<UpdateItemServicoRequest>(),
            new InlineValidator<UpdateServicoOrdemServicoRequest>(),
            mediator,
            NullLogger<ItemServicoController>.Instance);

        var orcamentoId = Guid.NewGuid();
        var itemServicoId = Guid.NewGuid();
        var servicoId = Guid.NewGuid();

        var result = await controller.AtualizarServicoOrcamento(new UpdateServicoOrcamentoRequest
        {
            ItemServicoId = itemServicoId,
            OrcamentoId = orcamentoId,
            ServicoId = servicoId
        }, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.NotNull(mediator.UpdateServicoOrcamentoCommandEnviado);
        Assert.Equal(orcamentoId, mediator.UpdateServicoOrcamentoCommandEnviado!.OrcamentoId);
        Assert.Equal(itemServicoId, mediator.UpdateServicoOrcamentoCommandEnviado.ItemServicoId);
        Assert.Equal(servicoId, mediator.UpdateServicoOrcamentoCommandEnviado.ServicoId);
    }

    [Fact]
    public async Task CriarServicoOrdemServico_Deve_Enviar_Comando_Com_Ids()
    {
        var mediator = new FakeMediator();
        var controller = new ItemServicoController(
            new InlineValidator<CreateItemServicoRequest>(),
            new InlineValidator<CreateItemOrcamentoRequest>(),
            new InlineValidator<CreateServicoOrcamentoRequest>(),
            new InlineValidator<CreateServicoOrdemServicoRequest>(),
            new InlineValidator<UpdateItemOrcamentoRequest>(),
            new InlineValidator<UpdateServicoOrcamentoRequest>(),
            new InlineValidator<UpdateItemServicoRequest>(),
            new InlineValidator<UpdateServicoOrdemServicoRequest>(),
            mediator,
            NullLogger<ItemServicoController>.Instance);

        var ordemServicoId = Guid.NewGuid();
        var servicoId = Guid.NewGuid();

        var result = await controller.CriarServicoOrdemServico(new CreateServicoOrdemServicoRequest
        {
            OrdemServicoId = ordemServicoId,
            ServicoId = servicoId
        }, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.NotNull(mediator.CreateServicoOrdemServicoCommandEnviado);
        Assert.Equal(ordemServicoId, mediator.CreateServicoOrdemServicoCommandEnviado!.OrdemServicoId);
        Assert.Equal(servicoId, mediator.CreateServicoOrdemServicoCommandEnviado.ServicoId);
    }

    [Fact]
    public async Task AtualizarServicoOrdemServico_Deve_Enviar_Comando_Com_Ids()
    {
        var mediator = new FakeMediator();
        var controller = new ItemServicoController(
            new InlineValidator<CreateItemServicoRequest>(),
            new InlineValidator<CreateItemOrcamentoRequest>(),
            new InlineValidator<CreateServicoOrcamentoRequest>(),
            new InlineValidator<CreateServicoOrdemServicoRequest>(),
            new InlineValidator<UpdateItemOrcamentoRequest>(),
            new InlineValidator<UpdateServicoOrcamentoRequest>(),
            new InlineValidator<UpdateItemServicoRequest>(),
            new InlineValidator<UpdateServicoOrdemServicoRequest>(),
            mediator,
            NullLogger<ItemServicoController>.Instance);

        var ordemServicoId = Guid.NewGuid();
        var itemServicoId = Guid.NewGuid();
        var servicoId = Guid.NewGuid();

        var result = await controller.AtualizarServicoOrdemServico(new UpdateServicoOrdemServicoRequest
        {
            ItemServicoId = itemServicoId,
            OrdemServicoId = ordemServicoId,
            ServicoId = servicoId
        }, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.NotNull(mediator.UpdateServicoOrdemServicoCommandEnviado);
        Assert.Equal(ordemServicoId, mediator.UpdateServicoOrdemServicoCommandEnviado!.OrdemServicoId);
        Assert.Equal(itemServicoId, mediator.UpdateServicoOrdemServicoCommandEnviado.ItemServicoId);
        Assert.Equal(servicoId, mediator.UpdateServicoOrdemServicoCommandEnviado.ServicoId);
    }

    private sealed class FakeMediator : IMediator
    {
        public GetItemServicosByOrcamentoQuery? QueryEnviada { get; private set; }
        public GetItemServicoByOrcamentoIdQuery? QueryPorIdEnviada { get; private set; }
        public UpdateItemOrcamentoCommand? UpdateItemOrcamentoCommandEnviado { get; private set; }
        public CreateServicoOrcamentoCommand? CreateServicoOrcamentoCommandEnviado { get; private set; }
        public UpdateServicoOrcamentoCommand? UpdateServicoOrcamentoCommandEnviado { get; private set; }
        public CreateServicoOrdemServicoCommand? CreateServicoOrdemServicoCommandEnviado { get; private set; }
        public UpdateServicoOrdemServicoCommand? UpdateServicoOrdemServicoCommandEnviado { get; private set; }
        public IReadOnlyCollection<OrcamentoItemResponse>? ItensOrcamentoResponse { get; init; }

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            if (request is GetItemServicosByOrcamentoQuery query)
            {
                QueryEnviada = query;
                return Task.FromResult((TResponse)(object)Result.Success<IReadOnlyCollection<OrcamentoItemResponse>>(ItensOrcamentoResponse ?? []));
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

            if (request is CreateServicoOrcamentoCommand createServicoOrcamentoCommand)
            {
                CreateServicoOrcamentoCommandEnviado = createServicoOrcamentoCommand;
                return Task.FromResult((TResponse)(object)Result.Success());
            }

            if (request is UpdateServicoOrcamentoCommand updateServicoOrcamentoCommand)
            {
                UpdateServicoOrcamentoCommandEnviado = updateServicoOrcamentoCommand;
                return Task.FromResult((TResponse)(object)Result.Success());
            }

            if (request is CreateServicoOrdemServicoCommand createServicoOrdemServicoCommand)
            {
                CreateServicoOrdemServicoCommandEnviado = createServicoOrdemServicoCommand;
                return Task.FromResult((TResponse)(object)Result.Success());
            }

            if (request is UpdateServicoOrdemServicoCommand updateServicoOrdemServicoCommand)
            {
                UpdateServicoOrdemServicoCommandEnviado = updateServicoOrdemServicoCommand;
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

            if (request is CreateServicoOrcamentoCommand createServicoOrcamentoCommand)
            {
                CreateServicoOrcamentoCommandEnviado = createServicoOrcamentoCommand;
                return Task.CompletedTask;
            }

            if (request is UpdateServicoOrcamentoCommand updateServicoOrcamentoCommand)
            {
                UpdateServicoOrcamentoCommandEnviado = updateServicoOrcamentoCommand;
                return Task.CompletedTask;
            }

            if (request is CreateServicoOrdemServicoCommand createServicoOrdemServicoCommand)
            {
                CreateServicoOrdemServicoCommandEnviado = createServicoOrdemServicoCommand;
                return Task.CompletedTask;
            }

            if (request is UpdateServicoOrdemServicoCommand updateServicoOrdemServicoCommand)
            {
                UpdateServicoOrdemServicoCommandEnviado = updateServicoOrdemServicoCommand;
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

            if (request is CreateServicoOrcamentoCommand createServicoOrcamentoCommand)
            {
                CreateServicoOrcamentoCommandEnviado = createServicoOrcamentoCommand;
                return Task.CompletedTask;
            }

            if (request is UpdateServicoOrcamentoCommand updateServicoOrcamentoCommand)
            {
                UpdateServicoOrcamentoCommandEnviado = updateServicoOrcamentoCommand;
                return Task.CompletedTask;
            }

            if (request is CreateServicoOrdemServicoCommand createServicoOrdemServicoCommand)
            {
                CreateServicoOrdemServicoCommandEnviado = createServicoOrdemServicoCommand;
                return Task.CompletedTask;
            }

            if (request is UpdateServicoOrdemServicoCommand updateServicoOrdemServicoCommand)
            {
                UpdateServicoOrdemServicoCommandEnviado = updateServicoOrdemServicoCommand;
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
