using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Ofichina.Api.Controllers.Orcamento;
using Ofichina.Application.UseCases.Orcamentos.Commands;
using Ofichina.Application.UseCases.Orcamentos.Queries;
using Ofichina.Application.Validators.Orcamento;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.Orcamento;
using Ofichina.Contracts.Responses.Orcamento;
using Ofichina.UnitTests.Ofichina.Api.TestDoubles;

namespace Ofichina.UnitTests.Ofichina.Api.Controllers.Orcamento;

public sealed class OrcamentoControllerTests
{
    // ============================================================
    // BuscarTodosOrcamentosPaginados
    // ============================================================

    [Fact]
    public async Task BuscarTodosOrcamentosPaginados_Deve_Retornar_Sucesso()
    {
        var mediator = new FakeMediator();

        var response = new PagedResponse<OrcamentoDetalheResponse>
        {
            Items = [],
            PageNumber = 1,
            PageSize = 10,
            TotalCount = 0,
            TotalPages = 0,
            HasNextPage = false,
            HasPreviousPage = false
        };

        mediator.RegistrarResposta<
            GetAllOrcamentosPaginadosQuery,
            Result<PagedResponse<OrcamentoDetalheResponse>>>(
            Result<PagedResponse<OrcamentoDetalheResponse>>.Success(response));

        var controller = CriarController(mediator);

        var result = await controller.BuscarTodosOrcamentosPaginados(
            new Pagination(1, 10),
            CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);

        var apiResponse =
            Assert.IsType<ApiResponse<PagedResponse<OrcamentoDetalheResponse>>>(ok.Value);

        Assert.True(apiResponse.Success);
        Assert.NotNull(apiResponse.Data);
        Assert.Empty(apiResponse.Data.Items);
    }

    [Fact]
    public async Task BuscarTodosOrcamentosPaginados_Deve_Retornar_BadRequest_Quando_Mediator_Falhar()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            GetAllOrcamentosPaginadosQuery,
            Result<PagedResponse<OrcamentoDetalheResponse>>>(
            Result<PagedResponse<OrcamentoDetalheResponse>>.Failure(
                "Erro ao obter os orçamentos."));

        var controller = CriarController(mediator);

        var result = await controller.BuscarTodosOrcamentosPaginados(
            new Pagination(1, 10),
            CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);

        var response = Assert.IsType<ApiResponse>(badRequest.Value);

        Assert.False(response.Success);
        Assert.Equal("Erro ao obter os orçamentos.", response.Message);
    }

    // ============================================================
    // BuscarOrcamentoPorId
    // ============================================================

    [Fact]
    public async Task BuscarOrcamentoPorId_Deve_Retornar_Sucesso()
    {
        var orcamentoId = Guid.NewGuid();

        var mediator = new FakeMediator();

        var response = new OrcamentoResponse();

        mediator.RegistrarResposta<
            GetOrcamentoByIdQuery,
            Result<OrcamentoResponse>>(
            Result<OrcamentoResponse>.Success(response));

        var controller = CriarController(mediator);

        var result = await controller.BuscarOrcamentoPorId(
            orcamentoId,
            CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);

        var apiResponse =
            Assert.IsType<ApiResponse<OrcamentoResponse>>(ok.Value);

        Assert.True(apiResponse.Success);
        Assert.Same(response, apiResponse.Data);
    }

    [Fact]
    public async Task BuscarOrcamentoPorId_Deve_Retornar_NotFound_Quando_Mediator_Falhar()
    {
        var orcamentoId = Guid.NewGuid();

        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            GetOrcamentoByIdQuery,
            Result<OrcamentoResponse>>(
            Result<OrcamentoResponse>.Failure(
                "Orçamento não encontrado."));

        var controller = CriarController(mediator);

        var result = await controller.BuscarOrcamentoPorId(
            orcamentoId,
            CancellationToken.None);

        var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);

        var response = Assert.IsType<ApiResponse>(notFound.Value);

        Assert.False(response.Success);
        Assert.Equal("Orçamento não encontrado.", response.Message);
    }

    [Fact]
    public async Task BuscarOrcamentoPorId_Deve_Retornar_NotFound_Quando_Result_For_Sucesso_Mas_Value_For_Null()
    {
        var orcamentoId = Guid.NewGuid();

        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            GetOrcamentoByIdQuery,
            Result<OrcamentoResponse>>(
            Result<OrcamentoResponse>.Success(null!));

        var controller = CriarController(mediator);

        var result = await controller.BuscarOrcamentoPorId(
            orcamentoId,
            CancellationToken.None);

        var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);

        var response = Assert.IsType<ApiResponse>(notFound.Value);

        Assert.False(response.Success);
        Assert.Equal("Orçamento não encontrado.", response.Message);
    }

    // ============================================================
    // CriarOrcamento
    // ============================================================

    [Fact]
    public async Task CriarOrcamento_Deve_Rejeitar_Requisicao_Invalida()
    {
        var controller = CriarController(new FakeMediator());

        var request = new CreateOrcamentoRequest();

        var result = await controller.CriarOrcamento(
            request,
            CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);

        var response = Assert.IsType<ApiResponse>(badRequest.Value);

        Assert.False(response.Success);
    }

    [Fact]
    public async Task CriarOrcamento_Deve_Retornar_Sucesso()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            CreateOrcamentoCommand,
            Result>(
            Result.Success());

        var controller = CriarController(mediator);

        var request = CriarCreateOrcamentoRequestValido();

        var result = await controller.CriarOrcamento(
            request,
            CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);

        var response = Assert.IsType<ApiResponse>(ok.Value);

        Assert.True(response.Success);
        Assert.Equal(
            "Orçamento criado com sucesso.",
            response.Message);
    }

    [Fact]
    public async Task CriarOrcamento_Deve_Retornar_BadRequest_Quando_Mediator_Falhar()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            CreateOrcamentoCommand,
            Result>(
            Result.Failure(
                "Erro ao criar orçamento."));

        var controller = CriarController(mediator);

        var request = CriarCreateOrcamentoRequestValido();

        var result = await controller.CriarOrcamento(
            request,
            CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);

        var response = Assert.IsType<ApiResponse>(badRequest.Value);

        Assert.False(response.Success);
        Assert.Equal(
            "Erro ao criar orçamento.",
            response.Message);
    }

    // ============================================================
    // EnviarOrcamentoParaCliente
    // ============================================================

    [Fact]
    public async Task EnviarOrcamentoParaCliente_Deve_Retornar_Sucesso()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            EnviarOrcamentoParaClienteCommand,
            Result>(
            Result.Success());

        var controller = CriarController(mediator);

        var request = new OrcamentoRequest(Guid.NewGuid());

        var result = await controller.EnviarOrcamentoParaCliente(
            request,
            CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);

        var response = Assert.IsType<ApiResponse>(ok.Value);

        Assert.True(response.Success);
        Assert.Equal(
            "Orçamento enviado para o cliente com sucesso.",
            response.Message);
    }

    [Fact]
    public async Task EnviarOrcamentoParaCliente_Deve_Retornar_BadRequest_Quando_Mediator_Falhar()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            EnviarOrcamentoParaClienteCommand,
            Result>(
            Result.Failure(
                "Não foi possível enviar o orçamento."));

        var controller = CriarController(mediator);

        var request = new OrcamentoRequest(Guid.NewGuid());

        var result = await controller.EnviarOrcamentoParaCliente(
            request,
            CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);

        var response = Assert.IsType<ApiResponse>(badRequest.Value);

        Assert.False(response.Success);
        Assert.Equal(
            "Não foi possível enviar o orçamento.",
            response.Message);
    }

    // ============================================================
    // AprovarOrcamento
    // ============================================================

    [Fact]
    public async Task AprovarOrcamento_Deve_Retornar_Sucesso()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            AprovarOrcamentoCommand,
            Result>(
            Result.Success());

        var controller = CriarController(mediator);

        var request = new AprovarOrcamentoRequest(Guid.NewGuid());

        var result = await controller.AprovarOrcamento(
            request,
            CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);

        var response = Assert.IsType<ApiResponse>(ok.Value);

        Assert.True(response.Success);
        Assert.Equal(
            "Orçamento aprovado com sucesso.",
            response.Message);
    }

    [Fact]
    public async Task AprovarOrcamento_Deve_Retornar_BadRequest_Quando_Mediator_Falhar()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            AprovarOrcamentoCommand,
            Result>(
            Result.Failure(
                "Não foi possível aprovar o orçamento."));

        var controller = CriarController(mediator);

        var request = new AprovarOrcamentoRequest(Guid.NewGuid());

        var result = await controller.AprovarOrcamento(
            request,
            CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);

        var response = Assert.IsType<ApiResponse>(badRequest.Value);

        Assert.False(response.Success);
        Assert.Equal(
            "Não foi possível aprovar o orçamento.",
            response.Message);
    }

    // ============================================================
    // ReprovarOrcamento
    // ============================================================

    [Fact]
    public async Task ReprovarOrcamento_Deve_Retornar_Sucesso()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            ReprovarOrcamentoCommand,
            Result>(
            Result.Success());

        var controller = CriarController(mediator);

        var request = new ReprovarOrcamentoRequest(
            Guid.NewGuid(),
            "Serviço não autorizado pelo cliente.");

        var result = await controller.ReprovarOrcamento(
            request,
            CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);

        var response = Assert.IsType<ApiResponse>(ok.Value);

        Assert.True(response.Success);
        Assert.Equal(
            "Orçamento reprovado com sucesso.",
            response.Message);
    }

    [Fact]
    public async Task ReprovarOrcamento_Deve_Retornar_BadRequest_Quando_Mediator_Falhar()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            ReprovarOrcamentoCommand,
            Result>(
            Result.Failure(
                "Não foi possível reprovar o orçamento."));

        var controller = CriarController(mediator);

        var request = new ReprovarOrcamentoRequest(
            Guid.NewGuid(),
            "Cliente recusou.");

        var result = await controller.ReprovarOrcamento(
            request,
            CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);

        var response = Assert.IsType<ApiResponse>(badRequest.Value);

        Assert.False(response.Success);
        Assert.Equal(
            "Não foi possível reprovar o orçamento.",
            response.Message);
    }

    // ============================================================
    // ReenviarOrcamentoAposReprovacao
    // ============================================================

    [Fact]
    public async Task ReenviarOrcamentoAposReprovacao_Deve_Retornar_Sucesso()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            ReenviarOrcamentoAposReprovacaoCommand,
            Result>(
            Result.Success());

        var controller = CriarController(mediator);

        var request = new OrcamentoRequest(Guid.NewGuid());

        var result = await controller.ReenviarOrcamentoAposReprovacao(
            request,
            CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);

        var response = Assert.IsType<ApiResponse>(ok.Value);

        Assert.True(response.Success);
        Assert.Equal(
            "Orçamento reenviado com sucesso.",
            response.Message);
    }

    [Fact]
    public async Task ReenviarOrcamentoAposReprovacao_Deve_Retornar_BadRequest_Quando_Mediator_Falhar()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            ReenviarOrcamentoAposReprovacaoCommand,
            Result>(
            Result.Failure(
                "Não foi possível reenviar o orçamento."));

        var controller = CriarController(mediator);

        var request = new OrcamentoRequest(Guid.NewGuid());

        var result = await controller.ReenviarOrcamentoAposReprovacao(
            request,
            CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);

        var response = Assert.IsType<ApiResponse>(badRequest.Value);

        Assert.False(response.Success);
        Assert.Equal(
            "Não foi possível reenviar o orçamento.",
            response.Message);
    }

    // ============================================================
    // AtualizarOrcamento
    // ============================================================

    [Fact]
    public async Task AtualizarOrcamento_Deve_Rejeitar_Requisicao_Invalida()
    {
        var controller = CriarController(new FakeMediator());

        var request = new UpdateOrcamentoRequest();

        var result = await controller.AtualizarOrcamento(
            request,
            CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);

        var response = Assert.IsType<ApiResponse>(badRequest.Value);

        Assert.False(response.Success);
    }

    [Fact]
    public async Task AtualizarOrcamento_Deve_Retornar_Sucesso()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            UpdateOrcamentoCommand,
            Result>(
            Result.Success());

        var controller = CriarController(mediator);

        var request = CriarUpdateOrcamentoRequestValido();

        var result = await controller.AtualizarOrcamento(
            request,
            CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);

        var response = Assert.IsType<ApiResponse>(ok.Value);

        Assert.True(response.Success);
        Assert.Equal(
            "Orçamento atualizado com sucesso.",
            response.Message);
    }

    [Fact]
    public async Task AtualizarOrcamento_Deve_Retornar_BadRequest_Quando_Mediator_Falhar()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            UpdateOrcamentoCommand,
            Result>(
            Result.Failure(
                "Erro ao atualizar orçamento."));

        var controller = CriarController(mediator);

        var request = CriarUpdateOrcamentoRequestValido();

        var result = await controller.AtualizarOrcamento(
            request,
            CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);

        var response = Assert.IsType<ApiResponse>(badRequest.Value);

        Assert.False(response.Success);
        Assert.Equal(
            "Erro ao atualizar orçamento.",
            response.Message);
    }

    // ============================================================
    // AtualizarDescontoOrcamento
    // ============================================================

    [Fact]
    public async Task AtualizarDescontoOrcamento_Deve_Rejeitar_Requisicao_Invalida()
    {
        var mediator = new FakeMediator();

        var controller = CriarController(mediator);

        var request = new UpdateOrcamentoDescontoRequest
        {
            OrcamentoId = Guid.Empty,
            Desconto = 0
        };

        var result = await controller.AtualizarDescontoOrcamento(
            request,
            CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);

        var response = Assert.IsType<ApiResponse>(badRequest.Value);

        Assert.False(response.Success);
    }

    [Fact]
    public async Task AtualizarDescontoOrcamento_Deve_Retornar_Sucesso()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            UpdateOrcamentoDescontoCommand,
            Result>(
            Result.Success());

        var controller = CriarController(mediator);

        var request = CriarUpdateOrcamentoDescontoRequestValido();

        var result = await controller.AtualizarDescontoOrcamento(
            request,
            CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);

        var response = Assert.IsType<ApiResponse>(ok.Value);

        Assert.True(response.Success);
        Assert.Equal(
            "Desconto do orçamento atualizado com sucesso.",
            response.Message);
    }

    [Fact]
    public async Task AtualizarDescontoOrcamento_Deve_Retornar_BadRequest_Quando_Mediator_Falhar()
    {
        var mediator = new FakeMediator();

        mediator.RegistrarResposta<
            UpdateOrcamentoDescontoCommand,
            Result>(
            Result.Failure(
                "Erro ao atualizar desconto."));

        var controller = CriarController(mediator);

        var request = CriarUpdateOrcamentoDescontoRequestValido();

        var result = await controller.AtualizarDescontoOrcamento(
            request,
            CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);

        var response = Assert.IsType<ApiResponse>(badRequest.Value);

        Assert.False(response.Success);
        Assert.Equal(
            "Erro ao atualizar desconto.",
            response.Message);
    }

    // ============================================================
    // Helpers
    // ============================================================

    private static OrcamentoController CriarController(
        FakeMediator mediator)
    {
        return new OrcamentoController(
            new CreateOrcamentoRequestValidator(),
            new UpdateOrcamentoRequestValidator(),
            new UpdateOrcamentoDescontoRequestValidator(),
            mediator,
            NullLogger<OrcamentoController>.Instance)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    private static CreateOrcamentoRequest CriarCreateOrcamentoRequestValido()
    {
        return new CreateOrcamentoRequest
        {
            PessoaId = Guid.NewGuid(),
            VeiculoId = Guid.NewGuid(),
            AgendamentoId = Guid.NewGuid()
        };
    }

    private static UpdateOrcamentoRequest CriarUpdateOrcamentoRequestValido()
    {
        return new UpdateOrcamentoRequest
        {
            OrcamentoId = Guid.NewGuid(),
            PessoaId = Guid.NewGuid(),
            VeiculoId = Guid.NewGuid(),
            ConsultorId = Guid.NewGuid(),
            MecanicoId = Guid.NewGuid(),
            DataValidade = DateOnly.FromDateTime(
                DateTime.Today.AddDays(30)),
            Observacoes = "Atualização de teste."
        };
    }

    private static UpdateOrcamentoDescontoRequest
        CriarUpdateOrcamentoDescontoRequestValido()
    {
        return new UpdateOrcamentoDescontoRequest
        {
            OrcamentoId = Guid.NewGuid(),
            Desconto = 10m,
            DescontoEmDinheiro = false
        };
    }
}