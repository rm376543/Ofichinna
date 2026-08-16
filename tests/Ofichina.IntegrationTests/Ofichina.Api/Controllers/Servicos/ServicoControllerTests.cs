using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Ofichina.Api.Controllers.Servicos;
using Ofichina.Application.UseCases.Servicos.Commands;
using Ofichina.Application.UseCases.Servicos.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.Servicos;
using Ofichina.Contracts.Responses.Servicos;

namespace Ofichina.IntegrationTests.Api.Controllers.Servicos;

public sealed class ServicoControllerTests
{
    [Fact]
    public async Task CriarServico_Deve_Validar_E_Retornar_Sucesso()
    {
        var mediator = new FakeMediator();
        var controller = new ServicoController(
            CriarCreateValidator(),
            new InlineValidator<UpdateServicoRequest>(),
            mediator,
            NullLogger<ServicoController>.Instance);

        var result = await controller.CriarServico(new CreateServicoRequest
        {
            Nome = "Troca de óleo",
            Descricao = "Serviço completo",
            Valor = 149.90m,
            Ativo = true
        }, CancellationToken.None);

        Assert.Null(result.Result);
        var response = Assert.IsType<ApiResponse>(result.Value);
        Assert.True(response.Success);
        Assert.Equal("Serviço criado com sucesso.", response.Message);
        Assert.IsType<CreateServicoCommand>(mediator.UltimoRequest);
    }

    [Fact]
    public async Task CriarServico_Deve_Rejeitar_Dados_Invalidos()
    {
        var mediator = new FakeMediator();
        var controller = new ServicoController(
            CriarCreateValidator(),
            new InlineValidator<UpdateServicoRequest>(),
            mediator,
            NullLogger<ServicoController>.Instance);

        var result = await controller.CriarServico(new CreateServicoRequest
        {
            Nome = string.Empty,
            Descricao = "",
            Valor = 0m,
            Ativo = true
        }, CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(badRequest.Value);
        Assert.False(response.Success);
        Assert.Empty(mediator.Enviados);
    }

    [Fact]
    public async Task AtualizarServico_Deve_Enviar_Command_E_Retornar_Sucesso()
    {
        var mediator = new FakeMediator();
        var controller = new ServicoController(
            new InlineValidator<CreateServicoRequest>(),
            CriarUpdateValidator(),
            mediator,
            NullLogger<ServicoController>.Instance);

        var id = Guid.NewGuid();
        var result = await controller.AtualizarServico(new UpdateServicoRequest
        {
            ServicoId = id,
            Nome = "Alinhamento",
            Descricao = "Ajuste de direção",
            Valor = 129.90m,
            Ativo = false
        }, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(ok.Value);
        Assert.True(response.Success);
        Assert.Equal("Serviço atualizado com sucesso.", response.Message);
        var command = Assert.IsType<UpdateServicoCommand>(mediator.UltimoRequest);
        Assert.Equal(id, command.ServicoId);
        Assert.False(command.Ativo);
    }

    [Fact]
    public async Task BuscarServicoPorId_Deve_Retornar_NotFound_Quando_Nao_Existir()
    {
        var mediator = new FakeMediator
        {
            GetServicoByIdResult = Result.Failure<ServicoResponse>("Serviço não encontrado.")
        };

        var controller = new ServicoController(
            new InlineValidator<CreateServicoRequest>(),
            new InlineValidator<UpdateServicoRequest>(),
            mediator,
            NullLogger<ServicoController>.Instance);

        var result = await controller.BuscarServicoPorId(Guid.NewGuid(), CancellationToken.None);

        var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(notFound.Value);
        Assert.False(response.Success);
        Assert.Equal("Serviço não encontrado.", response.Message);
    }

    [Fact]
    public async Task BuscarTodosServicosPaginados_Deve_Retornar_Paginacao_Com_Servicos()
    {
        var mediator = new FakeMediator
        {
            GetAllServicosResult = Result.Success(new PagedResponse<ServicoResponse>
            {
                Items =
                [
                    new ServicoResponse { ServicoId = Guid.NewGuid(), Nome = "Troca de óleo", Valor = 149.90m, Ativo = true },
                    new ServicoResponse { ServicoId = Guid.NewGuid(), Nome = "Alinhamento", Valor = 129.90m, Ativo = false }
                ],
                PageNumber = 1,
                PageSize = 10,
                TotalCount = 2,
                TotalPages = 1,
                HasNextPage = false,
                HasPreviousPage = false
            })
        };

        var controller = new ServicoController(
            new InlineValidator<CreateServicoRequest>(),
            new InlineValidator<UpdateServicoRequest>(),
            mediator,
            NullLogger<ServicoController>.Instance);

        var pagination = new Pagination(1, 10);
        var result = await controller.BuscarTodosServicosPaginados(pagination, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<PagedResponse<ServicoResponse>>>(ok.Value);
        Assert.True(response.Success);
        Assert.Equal(2, response.Data!.Items.Count);
        Assert.IsType<GetAllServicosPaginadosQuery>(mediator.UltimoRequest);
    }

    [Fact]
    public async Task RemoverServico_Deve_Enviar_Command_E_Retornar_Sucesso()
    {
        var mediator = new FakeMediator();
        var controller = new ServicoController(
            new InlineValidator<CreateServicoRequest>(),
            new InlineValidator<UpdateServicoRequest>(),
            mediator,
            NullLogger<ServicoController>.Instance);

        var id = Guid.NewGuid();
        var result = await controller.RemoverServico(new RemoveServicoRequest { ServicoId = id }, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(ok.Value);
        Assert.True(response.Success);
        Assert.Equal("Serviço removido com sucesso.", response.Message);
        var command = Assert.IsType<DeleteServicoCommand>(mediator.UltimoRequest);
        Assert.Equal(id, command.Id);
    }

    private static InlineValidator<CreateServicoRequest> CriarCreateValidator()
    {
        var validator = new InlineValidator<CreateServicoRequest>();
        validator.RuleFor(x => x.Nome).NotEmpty();
        validator.RuleFor(x => x.Valor).GreaterThan(0);
        return validator;
    }

    private static InlineValidator<UpdateServicoRequest> CriarUpdateValidator()
    {
        var validator = new InlineValidator<UpdateServicoRequest>();
        validator.RuleFor(x => x.ServicoId).NotEmpty();
        validator.RuleFor(x => x.Nome).NotEmpty();
        validator.RuleFor(x => x.Valor).GreaterThan(0);
        return validator;
    }

    private sealed class FakeMediator : IMediator
    {
        public object? UltimoRequest { get; private set; }

        public List<object> Enviados { get; } = [];

        public Result<ServicoResponse> GetServicoByIdResult { get; set; } = Result.Success(new ServicoResponse());

        public Result<PagedResponse<ServicoResponse>> GetAllServicosResult { get; set; } = Result.Success(new PagedResponse<ServicoResponse>());

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            UltimoRequest = request;
            Enviados.Add(request!);

            object response = request switch
            {
                CreateServicoCommand => Result.Success(),
                UpdateServicoCommand => Result.Success(),
                DeleteServicoCommand => Result.Success(),
                GetServicoByIdQuery => GetServicoByIdResult,
                GetAllServicosPaginadosQuery => GetAllServicosResult,
                _ => throw new NotSupportedException()
            };

            return Task.FromResult((TResponse)response);
        }

        Task ISender.Send<TRequest>(TRequest request, CancellationToken cancellationToken)
        {
            UltimoRequest = request;
            Enviados.Add(request!);
            return Task.CompletedTask;
        }

        public Task<TResponse> Send<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest<TResponse>
            => Send((IRequest<TResponse>)request!, cancellationToken);

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