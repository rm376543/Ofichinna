using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Ofichina.Api.Controllers.ItensServico;
using Ofichina.Application.UseCases.ItensServico.Commands;
using Ofichina.Application.UseCases.ItensServico.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.ItensServico;
using Ofichina.Contracts.Responses.Orcamento;
using Ofichina.Contracts.Responses.OrdemServico;
using System.Runtime.CompilerServices;

namespace Ofichina.UnitTests.Api.Controllers.ItemServico;

public sealed class ItemServicoControllerTests
{
    private readonly Mock<IValidator<CreateItemOrcamentoRequest>> _createItemOrcamentoValidatorMock;
    private readonly Mock<IValidator<CreateServicoOrcamentoRequest>> _createServicoOrcamentoValidatorMock;
    private readonly Mock<IValidator<UpdateItemOrcamentoRequest>> _updateOrcamentoValidatorMock;
    private readonly Mock<IValidator<UpdateServicoOrcamentoRequest>> _updateServicoOrcamentoValidatorMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ILogger<ItemServicoController>> _loggerMock;

    private readonly ItemServicoController _controller;

    public ItemServicoControllerTests()
    {
        _createItemOrcamentoValidatorMock =
            new Mock<IValidator<CreateItemOrcamentoRequest>>();

        _createServicoOrcamentoValidatorMock =
            new Mock<IValidator<CreateServicoOrcamentoRequest>>();

        _updateOrcamentoValidatorMock =
            new Mock<IValidator<UpdateItemOrcamentoRequest>>();

        _updateServicoOrcamentoValidatorMock =
            new Mock<IValidator<UpdateServicoOrcamentoRequest>>();

        _mediatorMock = new Mock<IMediator>();

        _loggerMock = new Mock<ILogger<ItemServicoController>>();

        _controller = new ItemServicoController(
            _createItemOrcamentoValidatorMock.Object,
            _createServicoOrcamentoValidatorMock.Object,
            _updateOrcamentoValidatorMock.Object,
            _updateServicoOrcamentoValidatorMock.Object,
            _mediatorMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public void Constructor_DependenciasValidas_Deve_CriarController()
    {
        // Arrange
        var mediator = new Mock<IMediator>();
        var logger = new Mock<ILogger<ItemServicoController>>();

        var createItemOrcamentoValidator =
            new Mock<IValidator<CreateItemOrcamentoRequest>>();

        var createServicoOrcamentoValidator =
            new Mock<IValidator<CreateServicoOrcamentoRequest>>();

        var updateOrcamentoValidator =
            new Mock<IValidator<UpdateItemOrcamentoRequest>>();

        var updateServicoOrcamentoValidator =
            new Mock<IValidator<UpdateServicoOrcamentoRequest>>();

        // Act
        var controller = new ItemServicoController(
            createItemOrcamentoValidator.Object,
            createServicoOrcamentoValidator.Object,
            updateOrcamentoValidator.Object,
            updateServicoOrcamentoValidator.Object,
            mediator.Object,
            logger.Object);

        // Assert
        Assert.NotNull(controller);
    }

    // ============================================================
    // BUSCAR ITENS POR ORÇAMENTO
    // ============================================================

    [Fact]
    public async Task BuscarItensOrcamento_ComSucessoEListaNaoNula_Deve_RetornarOk()
    {
        // Arrange
        var orcamentoId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        var itens = new List<OrcamentoItemResponse>
        {
            CriarOrcamentoItemResponse()
        };

        _mediatorMock
            .Setup(x => x.Send(
                It.IsAny<GetItemServicosByOrcamentoQuery>(),
                cancellationToken))
            .ReturnsAsync(
                Result<IReadOnlyCollection<OrcamentoItemResponse>>
                    .Success(itens));

        // Act
        var resultado = await _controller.BuscarItensOrcamento(
            orcamentoId,
            cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultado.Result);

        Assert.Equal(
            StatusCodes.Status200OK,
            okResult.StatusCode);

        Assert.NotNull(okResult.Value);

        _mediatorMock.Verify(
            x => x.Send(
                It.IsAny<GetItemServicosByOrcamentoQuery>(),
                cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task BuscarItensOrcamento_ComSucessoEListaNula_Deve_RetornarOk()
    {
        // Arrange
        var orcamentoId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        _mediatorMock
            .Setup(x => x.Send(
                It.IsAny<GetItemServicosByOrcamentoQuery>(),
                cancellationToken))
            .ReturnsAsync(
                Result<IReadOnlyCollection<OrcamentoItemResponse>>
                    .Success(null!));

        // Act
        var resultado = await _controller.BuscarItensOrcamento(
            orcamentoId,
            cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultado.Result);

        Assert.Equal(
            StatusCodes.Status200OK,
            okResult.StatusCode);

        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task BuscarItensOrcamento_ComFalhaEErrorInformado_Deve_RetornarNotFound()
    {
        // Arrange
        var orcamentoId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        _mediatorMock
            .Setup(x => x.Send(
                It.IsAny<GetItemServicosByOrcamentoQuery>(),
                cancellationToken))
            .ReturnsAsync(
                Result<IReadOnlyCollection<OrcamentoItemResponse>>
                    .Failure("Orçamento não encontrado."));

        // Act
        var resultado = await _controller.BuscarItensOrcamento(
            orcamentoId,
            cancellationToken);

        // Assert
        var notFound =
            Assert.IsType<NotFoundObjectResult>(resultado.Result);

        Assert.Equal(
            StatusCodes.Status404NotFound,
            notFound.StatusCode);

        Assert.NotNull(notFound.Value);
    }

    [Fact]
    public async Task BuscarItensOrcamento_ComFalhaESemError_Deve_RetornarNotFound()
    {
        // Arrange
        var orcamentoId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        _mediatorMock
            .Setup(x => x.Send(
                It.IsAny<GetItemServicosByOrcamentoQuery>(),
                cancellationToken))
            .ReturnsAsync(
                Result<IReadOnlyCollection<OrcamentoItemResponse>>
                    .Failure(new[] { "Erro de consulta." }));

        // Act
        var resultado = await _controller.BuscarItensOrcamento(
            orcamentoId,
            cancellationToken);

        // Assert
        var notFound =
            Assert.IsType<NotFoundObjectResult>(resultado.Result);

        Assert.Equal(
            StatusCodes.Status404NotFound,
            notFound.StatusCode);

        Assert.NotNull(notFound.Value);
    }


    // ============================================================
    // BUSCAR ITEM POR ID - ORÇAMENTO
    // ============================================================

    [Fact]
    public async Task BuscarItemServicoOrcamentoPorId_ComSucessoEValorNaoNulo_Deve_RetornarOk()
    {
        // Arrange
        var orcamentoId = Guid.NewGuid();
        var itemServicoId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        var item = CriarOrcamentoItemResponse();

        _mediatorMock
            .Setup(x => x.Send(
                It.IsAny<GetItemServicoByOrcamentoIdQuery>(),
                cancellationToken))
            .ReturnsAsync(
                Result<OrcamentoItemResponse>.Success(item));

        // Act
        var resultado = await _controller.BuscarItemServicoOrcamentoPorId(
            orcamentoId,
            itemServicoId,
            cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultado.Result);

        Assert.Equal(
            StatusCodes.Status200OK,
            okResult.StatusCode);

        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task BuscarItemServicoOrcamentoPorId_ComFalha_Deve_RetornarNotFound()
    {
        // Arrange
        var orcamentoId = Guid.NewGuid();
        var itemServicoId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        _mediatorMock
            .Setup(x => x.Send(
                It.IsAny<GetItemServicoByOrcamentoIdQuery>(),
                cancellationToken))
            .ReturnsAsync(
                Result<OrcamentoItemResponse>
                    .Failure("Item não encontrado."));

        // Act
        var resultado = await _controller.BuscarItemServicoOrcamentoPorId(
            orcamentoId,
            itemServicoId,
            cancellationToken);

        // Assert
        var notFound =
            Assert.IsType<NotFoundObjectResult>(resultado.Result);

        Assert.Equal(
            StatusCodes.Status404NotFound,
            notFound.StatusCode);
    }

    [Fact]
    public async Task BuscarItemServicoOrcamentoPorId_ComSucessoMasValorNulo_Deve_RetornarNotFound()
    {
        // Arrange
        var orcamentoId = Guid.NewGuid();
        var itemServicoId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        _mediatorMock
            .Setup(x => x.Send(
                It.IsAny<GetItemServicoByOrcamentoIdQuery>(),
                cancellationToken))
            .ReturnsAsync(
                Result<OrcamentoItemResponse>
                    .Success(null!));

        // Act
        var resultado = await _controller.BuscarItemServicoOrcamentoPorId(
            orcamentoId,
            itemServicoId,
            cancellationToken);

        // Assert
        var notFound =
            Assert.IsType<NotFoundObjectResult>(resultado.Result);

        Assert.Equal(
            StatusCodes.Status404NotFound,
            notFound.StatusCode);
    }

    [Fact]
    public async Task BuscarItemServicoOrcamentoPorId_ComFalhaESemError_Deve_RetornarNotFound()
    {
        // Arrange
        var orcamentoId = Guid.NewGuid();
        var itemServicoId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        _mediatorMock
            .Setup(x => x.Send(
                It.IsAny<GetItemServicoByOrcamentoIdQuery>(),
                cancellationToken))
            .ReturnsAsync(
                Result<OrcamentoItemResponse>
                    .Failure(new[] { "Erro de consulta." }));

        // Act
        var resultado = await _controller.BuscarItemServicoOrcamentoPorId(
            orcamentoId,
            itemServicoId,
            cancellationToken);

        // Assert
        var notFound =
            Assert.IsType<NotFoundObjectResult>(resultado.Result);

        Assert.Equal(
            StatusCodes.Status404NotFound,
            notFound.StatusCode);
    }

    // ============================================================
    // CRIAR ITEM ORÇAMENTO
    // ============================================================

    [Fact]
    public async Task CriarItemOrcamento_ValidacaoInvalida_Deve_RetornarBadRequest()
    {
        // Arrange
        var request = CriarRequest<CreateItemOrcamentoRequest>();
        var cancellationToken = CancellationToken.None;

        ConfigurarValidacao(
            _createItemOrcamentoValidatorMock,
            request,
            false);

        // Act
        var resultado = await _controller.CriarItemOrcamento(
            request,
            cancellationToken);

        // Assert
        Assert.IsType<BadRequestObjectResult>(resultado.Result);

        _mediatorMock.Verify(
            x => x.Send(
                It.IsAny<CreateItemOrcamentoCommand>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task CriarItemOrcamento_ComFalhaDoMediator_Deve_RetornarBadRequest()
    {
        // Arrange
        var request = CriarRequest<CreateItemOrcamentoRequest>();
        var cancellationToken = CancellationToken.None;

        ConfigurarValidacao(
            _createItemOrcamentoValidatorMock,
            request,
            true);

        _mediatorMock
            .Setup(x => x.Send(
                It.IsAny<CreateItemOrcamentoCommand>(),
                cancellationToken))
            .ReturnsAsync(
                Result.Failure("Não foi possível criar o item."));

        // Act
        var resultado = await _controller.CriarItemOrcamento(
            request,
            cancellationToken);

        // Assert
        Assert.IsType<BadRequestObjectResult>(resultado.Result);
    }

    [Fact]
    public async Task CriarItemOrcamento_ComFalhaDoMediatorSemError_Deve_RetornarBadRequest()
    {
        // Arrange
        var request = CriarRequest<CreateItemOrcamentoRequest>();
        var cancellationToken = CancellationToken.None;

        ConfigurarValidacao(
            _createItemOrcamentoValidatorMock,
            request,
            true);

        _mediatorMock
            .Setup(x => x.Send(
                It.IsAny<CreateItemOrcamentoCommand>(),
                cancellationToken))
            .ReturnsAsync(
                Result.Failure(new[] { "Erro." }));

        // Act
        var resultado = await _controller.CriarItemOrcamento(
            request,
            cancellationToken);

        // Assert
        Assert.IsType<BadRequestObjectResult>(resultado.Result);
    }

    [Fact]
    public async Task CriarItemOrcamento_ComSucesso_Deve_RetornarOk()
    {
        // Arrange
        var request = CriarRequest<CreateItemOrcamentoRequest>();
        var cancellationToken = CancellationToken.None;

        ConfigurarValidacao(
            _createItemOrcamentoValidatorMock,
            request,
            true);

        _mediatorMock
            .Setup(x => x.Send(
                It.IsAny<CreateItemOrcamentoCommand>(),
                cancellationToken))
            .ReturnsAsync(Result.Success());

        // Act
        var resultado = await _controller.CriarItemOrcamento(
            request,
            cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultado.Result);

        Assert.Equal(
            StatusCodes.Status200OK,
            okResult.StatusCode);
    }

    // ============================================================
    // CRIAR SERVIÇO ORÇAMENTO
    // ============================================================

    [Fact]
    public async Task CriarServicoOrcamento_ValidacaoInvalida_Deve_RetornarBadRequest()
    {
        // Arrange
        var request = CriarRequest<CreateServicoOrcamentoRequest>();
        var cancellationToken = CancellationToken.None;

        ConfigurarValidacao(
            _createServicoOrcamentoValidatorMock,
            request,
            false);

        // Act
        var resultado = await _controller.CriarServicoOrcamento(
            request,
            cancellationToken);

        // Assert
        Assert.IsType<BadRequestObjectResult>(resultado.Result);
    }

    [Fact]
    public async Task CriarServicoOrcamento_ComFalhaDoMediator_Deve_RetornarBadRequest()
    {
        // Arrange
        var request = CriarRequest<CreateServicoOrcamentoRequest>();
        var cancellationToken = CancellationToken.None;

        ConfigurarValidacao(
            _createServicoOrcamentoValidatorMock,
            request,
            true);

        _mediatorMock
            .Setup(x => x.Send(
                It.IsAny<CreateServicoOrcamentoCommand>(),
                cancellationToken))
            .ReturnsAsync(
                Result.Failure("Erro ao criar serviço."));

        // Act
        var resultado = await _controller.CriarServicoOrcamento(
            request,
            cancellationToken);

        // Assert
        Assert.IsType<BadRequestObjectResult>(resultado.Result);
    }

    [Fact]
    public async Task CriarServicoOrcamento_ComFalhaDoMediatorSemError_Deve_RetornarBadRequest()
    {
        // Arrange
        var request = CriarRequest<CreateServicoOrcamentoRequest>();
        var cancellationToken = CancellationToken.None;

        ConfigurarValidacao(
            _createServicoOrcamentoValidatorMock,
            request,
            true);

        _mediatorMock
            .Setup(x => x.Send(
                It.IsAny<CreateServicoOrcamentoCommand>(),
                cancellationToken))
            .ReturnsAsync(
                Result.Failure(new[] { "Erro." }));

        // Act
        var resultado = await _controller.CriarServicoOrcamento(
            request,
            cancellationToken);

        // Assert
        Assert.IsType<BadRequestObjectResult>(resultado.Result);
    }

    [Fact]
    public async Task CriarServicoOrcamento_ComSucesso_Deve_RetornarOk()
    {
        // Arrange
        var request = CriarRequest<CreateServicoOrcamentoRequest>();
        var cancellationToken = CancellationToken.None;

        ConfigurarValidacao(
            _createServicoOrcamentoValidatorMock,
            request,
            true);

        _mediatorMock
            .Setup(x => x.Send(
                It.IsAny<CreateServicoOrcamentoCommand>(),
                cancellationToken))
            .ReturnsAsync(Result.Success());

        // Act
        var resultado = await _controller.CriarServicoOrcamento(
            request,
            cancellationToken);

        // Assert
        Assert.IsType<OkObjectResult>(resultado.Result);
    }

    // ============================================================
    // ATUALIZAR ITEM ORÇAMENTO
    // ============================================================

    [Fact]
    public async Task AtualizarItemOrcamento_ValidacaoInvalida_Deve_RetornarBadRequest()
    {
        // Arrange
        var request = CriarRequest<UpdateItemOrcamentoRequest>();
        var cancellationToken = CancellationToken.None;

        ConfigurarValidacao(
            _updateOrcamentoValidatorMock,
            request,
            false);

        // Act
        var resultado = await _controller.AtualizarItemOrcamento(
            request,
            cancellationToken);

        // Assert
        Assert.IsType<BadRequestObjectResult>(resultado.Result);
    }

    [Fact]
    public async Task AtualizarItemOrcamento_ComFalhaDoMediator_Deve_RetornarBadRequest()
    {
        // Arrange
        var request = CriarRequest<UpdateItemOrcamentoRequest>();
        var cancellationToken = CancellationToken.None;

        ConfigurarValidacao(
            _updateOrcamentoValidatorMock,
            request,
            true);

        _mediatorMock
            .Setup(x => x.Send(
                It.IsAny<UpdateItemOrcamentoCommand>(),
                cancellationToken))
            .ReturnsAsync(
                Result.Failure("Erro ao atualizar item."));

        // Act
        var resultado = await _controller.AtualizarItemOrcamento(
            request,
            cancellationToken);

        // Assert
        Assert.IsType<BadRequestObjectResult>(resultado.Result);
    }

    [Fact]
    public async Task AtualizarItemOrcamento_ComFalhaDoMediatorSemError_Deve_RetornarBadRequest()
    {
        // Arrange
        var request = CriarRequest<UpdateItemOrcamentoRequest>();
        var cancellationToken = CancellationToken.None;

        ConfigurarValidacao(
            _updateOrcamentoValidatorMock,
            request,
            true);

        _mediatorMock
            .Setup(x => x.Send(
                It.IsAny<UpdateItemOrcamentoCommand>(),
                cancellationToken))
            .ReturnsAsync(
                Result.Failure(new[] { "Erro." }));

        // Act
        var resultado = await _controller.AtualizarItemOrcamento(
            request,
            cancellationToken);

        // Assert
        Assert.IsType<BadRequestObjectResult>(resultado.Result);
    }

    [Fact]
    public async Task AtualizarItemOrcamento_ComSucesso_Deve_RetornarOk()
    {
        // Arrange
        var request = CriarRequest<UpdateItemOrcamentoRequest>();
        var cancellationToken = CancellationToken.None;

        ConfigurarValidacao(
            _updateOrcamentoValidatorMock,
            request,
            true);

        _mediatorMock
            .Setup(x => x.Send(
                It.IsAny<UpdateItemOrcamentoCommand>(),
                cancellationToken))
            .ReturnsAsync(Result.Success());

        // Act
        var resultado = await _controller.AtualizarItemOrcamento(
            request,
            cancellationToken);

        // Assert
        Assert.IsType<OkObjectResult>(resultado.Result);
    }

    // ============================================================
    // ATUALIZAR SERVIÇO ORÇAMENTO
    // ============================================================

    [Fact]
    public async Task AtualizarServicoOrcamento_ValidacaoInvalida_Deve_RetornarBadRequest()
    {
        // Arrange
        var request = CriarRequest<UpdateServicoOrcamentoRequest>();
        var cancellationToken = CancellationToken.None;

        ConfigurarValidacao(
            _updateServicoOrcamentoValidatorMock,
            request,
            false);

        // Act
        var resultado = await _controller.AtualizarServicoOrcamento(
            request,
            cancellationToken);

        // Assert
        Assert.IsType<BadRequestObjectResult>(resultado.Result);
    }

    [Fact]
    public async Task AtualizarServicoOrcamento_ComFalhaDoMediator_Deve_RetornarBadRequest()
    {
        // Arrange
        var request = CriarRequest<UpdateServicoOrcamentoRequest>();
        var cancellationToken = CancellationToken.None;

        ConfigurarValidacao(
            _updateServicoOrcamentoValidatorMock,
            request,
            true);

        _mediatorMock
            .Setup(x => x.Send(
                It.IsAny<UpdateServicoOrcamentoCommand>(),
                cancellationToken))
            .ReturnsAsync(
                Result.Failure("Erro ao atualizar serviço."));

        // Act
        var resultado = await _controller.AtualizarServicoOrcamento(
            request,
            cancellationToken);

        // Assert
        Assert.IsType<BadRequestObjectResult>(resultado.Result);
    }

    [Fact]
    public async Task AtualizarServicoOrcamento_ComFalhaDoMediatorSemError_Deve_RetornarBadRequest()
    {
        // Arrange
        var request = CriarRequest<UpdateServicoOrcamentoRequest>();
        var cancellationToken = CancellationToken.None;

        ConfigurarValidacao(
            _updateServicoOrcamentoValidatorMock,
            request,
            true);

        _mediatorMock
            .Setup(x => x.Send(
                It.IsAny<UpdateServicoOrcamentoCommand>(),
                cancellationToken))
            .ReturnsAsync(
                Result.Failure(new[] { "Erro." }));

        // Act
        var resultado = await _controller.AtualizarServicoOrcamento(
            request,
            cancellationToken);

        // Assert
        Assert.IsType<BadRequestObjectResult>(resultado.Result);
    }

    [Fact]
    public async Task AtualizarServicoOrcamento_ComSucesso_Deve_RetornarOk()
    {
        // Arrange
        var request = CriarRequest<UpdateServicoOrcamentoRequest>();
        var cancellationToken = CancellationToken.None;

        ConfigurarValidacao(
            _updateServicoOrcamentoValidatorMock,
            request,
            true);

        _mediatorMock
            .Setup(x => x.Send(
                It.IsAny<UpdateServicoOrcamentoCommand>(),
                cancellationToken))
            .ReturnsAsync(Result.Success());

        // Act
        var resultado = await _controller.AtualizarServicoOrcamento(
            request,
            cancellationToken);

        // Assert
        Assert.IsType<OkObjectResult>(resultado.Result);
    }

    // ============================================================
    // HELPERS
    // ============================================================

    private static T CriarRequest<T>()
        where T : class
    {
        return (T)RuntimeHelpers.GetUninitializedObject(typeof(T));
    }

    private static TResponse CriarResponse<TResponse>()
        where TResponse : class
    {
        return (TResponse)RuntimeHelpers.GetUninitializedObject(
            typeof(TResponse));
    }

    private static OrdemServicoItensResponse CriarOrdemServicoItensResponse()
    {
        return CriarResponse<OrdemServicoItensResponse>();
    }

    private static OrcamentoItemResponse CriarOrcamentoItemResponse()
    {
        return CriarResponse<OrcamentoItemResponse>();
    }

    private static void ConfigurarValidacao<T>(
        Mock<IValidator<T>> validatorMock,
        T request,
        bool valido)
    {
        var validationResult = valido
            ? new ValidationResult()
            : new ValidationResult(
                new[]
                {
                    new ValidationFailure(
                        "Request",
                        "Dados inválidos.")
                });

        validatorMock
            .Setup(x => x.ValidateAsync(
                request,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);
    }
}