using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Ofichina.Api.Controllers.Pessoa;
using Ofichina.Application.UseCases.Pessoas.Commands;
using Ofichina.Application.UseCases.Pessoas.Queries;
using Ofichina.Application.Validators.Pessoa;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.Pessoa;
using Ofichina.Contracts.Responses.Pessoa;
using Ofichina.UnitTests.Api.TestDoubles;

namespace Ofichina.UnitTests.Api.Controllers.Pessoa;

public sealed class PessoaControllerTests
{
    [Fact]
    public async Task BuscarPessoaPorId_Deve_Retornar_NotFound_Quando_Nao_Existir()
    {
        var mediator = new FakeMediator();
        mediator.RegistrarResposta<GetPessoaByIdQuery, Result<PessoaResponse>>(Result.Failure<PessoaResponse>("Pessoa não encontrada."));

        var controller = CriarController(mediator);

        var result = await controller.BuscarPessoaPorId(Guid.NewGuid(), CancellationToken.None);

        var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(notFound.Value);
        Assert.False(response.Success);
    }

    [Fact]
    public async Task CriarPessoa_Deve_Rejeitar_Requisicao_Invalida()
    {
        var controller = CriarController(new FakeMediator());

        var result = await controller.CriarPessoa(new CreatePessoaRequest(), CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(badRequest.Value);
        Assert.False(response.Success);
    }

    [Fact]
    public async Task CriarPessoa_Deve_Retornar_Sucesso()
    {
        var mediator = new FakeMediator();
        mediator.RegistrarResposta<CreatePessoaCommand, Result>(Result.Success());

        var controller = CriarController(mediator);
        var request = CriarRequestValido();

        var result = await controller.CriarPessoa(request, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(ok.Value);
        Assert.True(response.Success);
        Assert.Equal("Pessoa criada com sucesso.", response.Message);
    }

    [Fact]
    public async Task AtualizarPessoa_Deve_Retornar_Sucesso()
    {
        var mediator = new FakeMediator();
        mediator.RegistrarResposta<UpdatePessoaCommand, Result>(Result.Success());

        var controller = CriarController(mediator);
        var request = CriarUpdateRequestValido();

        var result = await controller.AtualizarPessoa(request, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(ok.Value);
        Assert.True(response.Success);
        Assert.Equal("Pessoa atualizada com sucesso.", response.Message);
    }

    [Fact]
    public async Task DeletarPessoa_Deve_Retornar_NotFound_Quando_Mediador_Falhar()
    {
        var mediator = new FakeMediator();
        mediator.RegistrarResposta<DeletePessoaCommand, Result>(Result.Failure("Pessoa não encontrada."));

        var controller = CriarController(mediator);

        var result = await controller.DeletarPessoa(new RemovePessoaRequest { PessoaId = Guid.NewGuid() }, CancellationToken.None);

        var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(notFound.Value);
        Assert.Equal("Pessoa não encontrada.", response.Message);
    }

    private static PessoaController CriarController(FakeMediator mediator)
        => new(
            new CreatePessoaRequestValidator(),
            new UpdatePessoaRequestValidator(),
            mediator,
            NullLogger<PessoaController>.Instance)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };

    private static CreatePessoaRequest CriarRequestValido()
        => new()
        {
            Nome = "João Silva",
            Documento = "12345678901",
            Telefone = "11999999999",
            Logradouro = "Rua A",
            Numero = "123",
            Complemento = "Apto 1",
            Bairro = "Centro",
            Cidade = "São Paulo",
            Estado = "SP",
            Cep = "12345678",
            UsuarioId = Guid.NewGuid()
        };

    private static UpdatePessoaRequest CriarUpdateRequestValido()
        => new()
        {
            PessoaId = Guid.NewGuid(),
            Nome = "João Silva",
            Telefone = "11999999999",
            Logradouro = "Rua A",
            Numero = "123",
            Complemento = "Apto 1",
            Bairro = "Centro",
            Cidade = "São Paulo",
            Estado = "SP",
            Cep = "12345678"
        };
}