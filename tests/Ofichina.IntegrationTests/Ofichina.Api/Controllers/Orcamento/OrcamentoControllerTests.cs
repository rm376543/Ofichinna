using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Ofichina.Api.Controllers.Orcamento;
using Ofichina.Application.UseCases.Orcamentos.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.Orcamento;
using Ofichina.Contracts.Requests.ItensServico;

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
            ItensServico =
            [
                new CreateItemServicoRequest
                {
                    ServicoId = Guid.NewGuid(),
                    PecaId = Guid.NewGuid(),
                    Quantidade = 2
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
            ItensServico =
            [
                new UpdateItemServicoRequest
                {
                    ServicoId = Guid.NewGuid(),
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
    public async Task AprovarOrcamento_Deve_Enviar_Apenas_Id()
    {
        var mediator = new FakeMediator();
        var controller = new OrcamentoController(
            new InlineValidator<CreateOrcamentoRequest>(),
            new InlineValidator<UpdateOrcamentoRequest>(),
            mediator,
            NullLogger<OrcamentoController>.Instance);

        var id = Guid.NewGuid();
        var result = await controller.AprovarOrcamento(id, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.NotNull(mediator.AprovarCommandEnviado);
        Assert.Equal(id, mediator.AprovarCommandEnviado!.Id);
    }

    [Fact]
    public async Task ReprovarOrcamento_Deve_Enviar_Id_E_Motivo()
    {
        var mediator = new FakeMediator();
        var controller = new OrcamentoController(
            new InlineValidator<CreateOrcamentoRequest>(),
            new InlineValidator<UpdateOrcamentoRequest>(),
            mediator,
            NullLogger<OrcamentoController>.Instance);

        var id = Guid.NewGuid();
        var request = new ReprovarOrcamentoRequest
        {
            Motivo = "Valor acima do esperado"
        };

        var result = await controller.ReprovarOrcamento(id, request, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.NotNull(mediator.ReprovarCommandEnviado);
        Assert.Equal(id, mediator.ReprovarCommandEnviado!.Id);
        Assert.Equal(request.Motivo, mediator.ReprovarCommandEnviado.Motivo);
    }

    [Fact]
    public async Task ReenviarOrcamentoAposReprovacao_Deve_Enviar_Apenas_Id()
    {
        var mediator = new FakeMediator();
        var controller = new OrcamentoController(
            new InlineValidator<CreateOrcamentoRequest>(),
            new InlineValidator<UpdateOrcamentoRequest>(),
            mediator,
            NullLogger<OrcamentoController>.Instance);

        var id = Guid.NewGuid();
        var result = await controller.ReenviarOrcamentoAposReprovacao(id, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.NotNull(mediator.ReenviarCommandEnviado);
        Assert.Equal(id, mediator.ReenviarCommandEnviado!.Id);
    }

    private sealed class FakeMediator : IMediator
    {
        public CreateOrcamentoCommand? CreateCommandEnviado { get; private set; }
        public UpdateOrcamentoCommand? UpdateCommandEnviado { get; private set; }
        public AprovarOrcamentoCommand? AprovarCommandEnviado { get; private set; }
        public ReprovarOrcamentoCommand? ReprovarCommandEnviado { get; private set; }
        public ReenviarOrcamentoAposReprovacaoCommand? ReenviarCommandEnviado { get; private set; }

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

            if (request is ReprovarOrcamentoCommand reprovarCommand)
            {
                ReprovarCommandEnviado = reprovarCommand;
                return Task.FromResult((TResponse)(object)Result.Success());
            }

            if (request is ReenviarOrcamentoAposReprovacaoCommand reenviarCommand)
            {
                ReenviarCommandEnviado = reenviarCommand;
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

            if (request is ReprovarOrcamentoCommand reprovarCommand)
            {
                ReprovarCommandEnviado = reprovarCommand;
                return Task.CompletedTask;
            }

            if (request is ReenviarOrcamentoAposReprovacaoCommand reenviarCommand)
            {
                ReenviarCommandEnviado = reenviarCommand;
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
                        : request is ReprovarOrcamentoCommand reprovarCommand
                            ? Task.FromResult((TResponse)(object)Registrar(reprovarCommand))
                            : request is ReenviarOrcamentoAposReprovacaoCommand reenviarCommand
                                ? Task.FromResult((TResponse)(object)Registrar(reenviarCommand))
                                : throw new NotSupportedException();

        private static Result Registrar(CreateOrcamentoCommand command) => Result.Success();

        private static Result Registrar(UpdateOrcamentoCommand command) => Result.Success();

        private static Result Registrar(AprovarOrcamentoCommand command) => Result.Success();

        private static Result Registrar(ReprovarOrcamentoCommand command) => Result.Success();

        private static Result Registrar(ReenviarOrcamentoAposReprovacaoCommand command) => Result.Success();

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
