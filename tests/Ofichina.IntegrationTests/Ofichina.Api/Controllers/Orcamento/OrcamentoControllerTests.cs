using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Ofichina.Api.Controllers.Orcamento;
using Ofichina.Application.UseCases.Orcamentos.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.Orcamento;
using Ofichina.Contracts.Requests.Orcamentos;

namespace Ofichina.IntegrationTests.Api.Controllers.Orcamento;

public sealed class OrcamentoControllerTests
{
    [Fact]
    public async Task CriarOrcamento_Deve_Enviar_Payload_Completo()
    {
        var mediator = new FakeMediator();
        var controller = new OrcamentoController(
            new InlineValidator<CreateOrcamentoRequest>(),
            new InlineValidator<UpdateOrcamentoRequest>(),
            mediator,
            NullLogger<OrcamentoController>.Instance);

        var request = new CreateOrcamentoRequest
        {
            PessoaId = Guid.NewGuid(),
            VeiculoId = Guid.NewGuid(),
            ResponsavelId = Guid.NewGuid(),
            MecanicoDiagnosticoId = Guid.NewGuid(),
            DataValidade = DateTime.UtcNow.AddDays(10),
            Desconto = 15,
            Observacoes = "Orçamento inicial",
            Servicos =
            [
                new CreateOrcamentoServicoRequest
                {
                    ServicoId = Guid.NewGuid(),
                    Quantidade = 2,
                    ValorUnitario = 120,
                    Observacoes = "Troca de óleo"
                }
            ],
            Pecas =
            [
                new CreateOrcamentoPecaRequest
                {
                    PecaId = Guid.NewGuid(),
                    Quantidade = 1,
                    ValorUnitario = 80,
                    Desconto = 5
                }
            ]
        };

        var result = await controller.CriarOrcamento(request, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.NotNull(mediator.CreateCommandEnviado);
        Assert.Equal(request.PessoaId, mediator.CreateCommandEnviado!.PessoaId);
        Assert.Equal(request.VeiculoId, mediator.CreateCommandEnviado.VeiculoId);
        Assert.Equal(request.MecanicoDiagnosticoId, mediator.CreateCommandEnviado.MecanicoDiagnosticoId);
        Assert.Equal(request.ResponsavelId, mediator.CreateCommandEnviado.ResponsavelId);
        Assert.Equal(request.DataValidade, mediator.CreateCommandEnviado.DataValidade);
        Assert.Equal(request.Desconto, mediator.CreateCommandEnviado.Desconto);
        Assert.Equal(request.Observacoes, mediator.CreateCommandEnviado.Observacoes);
    }

    [Fact]
    public async Task AtualizarOrcamento_Deve_Enviar_Payload_Completo()
    {
        var mediator = new FakeMediator();
        var controller = new OrcamentoController(
            new InlineValidator<CreateOrcamentoRequest>(),
            new InlineValidator<UpdateOrcamentoRequest>(),
            mediator,
            NullLogger<OrcamentoController>.Instance);

        var request = new UpdateOrcamentoRequest
        {
            Id = Guid.NewGuid(),
            PessoaId = Guid.NewGuid(),
            VeiculoId = Guid.NewGuid(),
            ResponsavelId = Guid.NewGuid(),
            MecanicoDiagnosticoId = Guid.NewGuid(),
            DataValidade = DateTime.UtcNow.AddDays(5),
            Desconto = 10,
            Observacoes = "Orçamento atualizado",
            Servicos =
            [
                new UpdateOrcamentoServicoRequest
                {
                    Id = Guid.NewGuid(),
                    ServicoId = Guid.NewGuid(),
                    Quantidade = 3
                }
            ],
            Pecas =
            [
                new UpdateOrcamentoPecaRequest
                {
                    Id = Guid.NewGuid(),
                    PecaId = Guid.NewGuid(),
                    Quantidade = 2
                }
            ]
        };

        var result = await controller.AtualizarOrcamento(request, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.NotNull(mediator.UpdateCommandEnviado);
        Assert.Equal(request.Id, mediator.UpdateCommandEnviado!.Id);
        Assert.Equal(request.PessoaId, mediator.UpdateCommandEnviado.PessoaId);
        Assert.Equal(request.VeiculoId, mediator.UpdateCommandEnviado.VeiculoId);
        Assert.Equal(request.MecanicoDiagnosticoId, mediator.UpdateCommandEnviado.MecanicoDiagnosticoId);
        Assert.Equal(request.ResponsavelId, mediator.UpdateCommandEnviado.ResponsavelId);
        Assert.Equal(request.DataValidade, mediator.UpdateCommandEnviado.DataValidade);
        Assert.Equal(request.Desconto, mediator.UpdateCommandEnviado.Desconto);
        Assert.Equal(request.Observacoes, mediator.UpdateCommandEnviado.Observacoes);
    }

    [Fact]
    public async Task AprovarOrcamento_Deve_Enviar_MecanicoReparoId()
    {
        var mediator = new FakeMediator();
        var controller = new OrcamentoController(
            new InlineValidator<CreateOrcamentoRequest>(),
            new InlineValidator<UpdateOrcamentoRequest>(),
            mediator,
            NullLogger<OrcamentoController>.Instance);

        var id = Guid.NewGuid();
        var mecanicoReparoId = Guid.NewGuid();

        var result = await controller.AprovarOrcamento(id, mecanicoReparoId, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.NotNull(mediator.AprovarCommandEnviado);
        Assert.Equal(id, mediator.AprovarCommandEnviado!.Id);
        Assert.Equal(mecanicoReparoId, mediator.AprovarCommandEnviado.MecanicoReparoId);
    }

    private sealed class FakeMediator : IMediator
    {
        public CreateOrcamentoCommand? CreateCommandEnviado { get; private set; }
        public UpdateOrcamentoCommand? UpdateCommandEnviado { get; private set; }
        public AprovarOrcamentoCommand? AprovarCommandEnviado { get; private set; }

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            if (request is CreateOrcamentoCommand createCommand)
            {
                CreateCommandEnviado = createCommand;
                return Task.FromResult((TResponse)(object)Result.Success());
            }

            if (request is UpdateOrcamentoCommand updateCommand)
            {
                UpdateCommandEnviado = updateCommand;
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
            if (request is CreateOrcamentoCommand createCommand)
            {
                CreateCommandEnviado = createCommand;
                return Task.CompletedTask;
            }

            if (request is UpdateOrcamentoCommand updateCommand)
            {
                UpdateCommandEnviado = updateCommand;
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
            => request is CreateOrcamentoCommand createCommand
                ? Task.FromResult((TResponse)(object)Registrar(createCommand))
                : request is UpdateOrcamentoCommand updateCommand
                    ? Task.FromResult((TResponse)(object)Registrar(updateCommand))
                    : request is AprovarOrcamentoCommand aprovarCommand
                        ? Task.FromResult((TResponse)(object)Registrar(aprovarCommand))
                        : throw new NotSupportedException();

        private static Result Registrar(CreateOrcamentoCommand command) => Result.Success();

        private static Result Registrar(UpdateOrcamentoCommand command) => Result.Success();

        private static Result Registrar(AprovarOrcamentoCommand command) => Result.Success();

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
