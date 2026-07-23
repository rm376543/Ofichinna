using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Ofichina.Api.Controllers.OrdensServico;
using Ofichina.Application.UseCases.OrdensServico.Commands;
using Ofichina.Application.Validators.OrdensServico;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.OrdensServico;

namespace Ofichina.IntegrationTests.Api.Controllers.OrdensServico;

public sealed class OrdemServicoControllerTests
{
    [Fact]
    public async Task CriarOrdemServico_Deve_Enviar_Apenas_Dados_Base()
    {
        var mediator = new FakeMediator();
        var controller = new OrdemServicoController(
            new CreateOrdemServicoRequestValidator(),
            new InlineValidator<UpdateOrdemServicoRequest>(),
            mediator,
            NullLogger<OrdemServicoController>.Instance);

        var request = new CreateOrdemServicoRequest
        {
            PessoaId = Guid.NewGuid(),
            VeiculoId = Guid.NewGuid(),
            FuncionarioId = Guid.NewGuid(),
            HodometroEntrada = 12345,
            ProblemaRelatado = "Barulho ao acelerar",
            Observacoes = "teste"
        };

        var result = await controller.CriarOrdemServico(request, CancellationToken.None);

        var objectResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status201Created, objectResult.StatusCode);
        Assert.NotNull(mediator.CommandEnviado);
        Assert.Equal(request.PessoaId, mediator.CommandEnviado!.PessoaId);
        Assert.Equal(request.VeiculoId, mediator.CommandEnviado.VeiculoId);
        Assert.Equal(request.FuncionarioId, mediator.CommandEnviado.FuncionarioId);
        Assert.Equal(request.HodometroEntrada, mediator.CommandEnviado.HodometroEntrada);
        Assert.Equal(request.ProblemaRelatado, mediator.CommandEnviado.ProblemaRelatado);
        Assert.Equal(request.Observacoes, mediator.CommandEnviado.Observacoes);
    }

    [Fact]
    public async Task AtualizarOrdemServico_Deve_Enviar_Payload_Completo()
    {
        var mediator = new FakeMediator();
        var controller = new OrdemServicoController(
            new CreateOrdemServicoRequestValidator(),
            new UpdateOrdemServicoRequestValidator(),
            mediator,
            NullLogger<OrdemServicoController>.Instance);

        var request = new UpdateOrdemServicoRequest
        {
            Id = Guid.NewGuid(),
            PessoaId = Guid.NewGuid(),
            VeiculoId = Guid.NewGuid(),
            FuncionarioId = Guid.NewGuid(),
            HodometroEntrada = 77290,
            ProblemaRelatado = "Carro esta fazendo barulhos durante a aceleração em aclive",
            Observacoes = "carro de dev"
        };

        var result = await controller.AtualizarOrdemServico(request, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.NotNull(mediator.UpdateCommandEnviado);
        Assert.Equal(request.Id, mediator.UpdateCommandEnviado!.Id);
        Assert.Equal(request.PessoaId, mediator.UpdateCommandEnviado.PessoaId);
        Assert.Equal(request.VeiculoId, mediator.UpdateCommandEnviado.VeiculoId);
        Assert.Equal(request.FuncionarioId, mediator.UpdateCommandEnviado.FuncionarioId);
        Assert.Equal(request.HodometroEntrada, mediator.UpdateCommandEnviado.HodometroEntrada);
        Assert.Equal(request.ProblemaRelatado, mediator.UpdateCommandEnviado.ProblemaRelatado);
        Assert.Equal(request.Observacoes, mediator.UpdateCommandEnviado.Observacoes);
    }

    private sealed class FakeMediator : IMediator
    {
        public CreateOrdemServicoCommand? CommandEnviado { get; private set; }
        public UpdateOrdemServicoCommand? UpdateCommandEnviado { get; private set; }

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            if (request is CreateOrdemServicoCommand command)
            {
                CommandEnviado = command;
                return Task.FromResult((TResponse)(object)Result.Success(Guid.NewGuid()));
            }

            if (request is UpdateOrdemServicoCommand updateCommand)
            {
                UpdateCommandEnviado = updateCommand;
                return Task.FromResult((TResponse)(object)Result.Success());
            }

            throw new NotSupportedException();
        }

        Task ISender.Send<TRequest>(TRequest request, CancellationToken cancellationToken)
        {
            if (request is CreateOrdemServicoCommand command)
            {
                CommandEnviado = command;
                return Task.CompletedTask;
            }

            if (request is UpdateOrdemServicoCommand updateCommand)
            {
                UpdateCommandEnviado = updateCommand;
                return Task.CompletedTask;
            }

            throw new NotSupportedException();
        }

        public Task<TResponse> Send<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest<TResponse>
            => request is CreateOrdemServicoCommand command
                ? Task.FromResult((TResponse)(object)Registrar(command))
                : request is UpdateOrdemServicoCommand updateCommand
                    ? Task.FromResult((TResponse)(object)Registrar(updateCommand))
                : throw new NotSupportedException();

        private static Result<Guid> Registrar(CreateOrdemServicoCommand command)
        {
            return Result.Success(Guid.NewGuid());
        }

        private static Result Registrar(UpdateOrdemServicoCommand command)
        {
            return Result.Success();
        }

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