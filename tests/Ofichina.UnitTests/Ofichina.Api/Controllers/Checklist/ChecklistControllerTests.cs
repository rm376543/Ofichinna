using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Ofichina.Api.Controllers.Checklist;
using Ofichina.Application.UseCases.Checklists.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.Checklist;

namespace Ofichina.UnitTests.Ofichina.Api.Controllers.Checklist;

public sealed class ChecklistControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ILogger<ChecklistController>> _loggerMock;
    private readonly ChecklistController _controller;

    public ChecklistControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _loggerMock = new Mock<ILogger<ChecklistController>>();

        _controller = new ChecklistController(
            _mediatorMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public void Constructor_DependenciasValidas_Deve_CriarController()
    {
        // Arrange
        var mediator = new Mock<IMediator>();
        var logger = new Mock<ILogger<ChecklistController>>();

        // Act
        var controller = new ChecklistController(
            mediator.Object,
            logger.Object);

        // Assert
        Assert.NotNull(controller);
    }

    [Fact]
    public async Task CriarChecklist_ComSucesso_Deve_RetornarCreated()
    {
        // Arrange
        var request = new CreateChecklistRequest();
        var cancellationToken = new CancellationToken();

        _mediatorMock
            .Setup(x => x.Send(
                It.IsAny<CreateChecklistCommand>(),
                cancellationToken))
            .ReturnsAsync(Result.Success());

        // Act
        var resultado = await _controller.CriarChecklist(
            request,
            cancellationToken);

        // Assert
        var createdResult = Assert.IsType<ObjectResult>(resultado.Result);

        Assert.Equal(
            StatusCodes.Status201Created,
            createdResult.StatusCode);

        Assert.NotNull(createdResult.Value);

        _mediatorMock.Verify(
            x => x.Send(
                It.IsAny<CreateChecklistCommand>(),
                cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task CriarChecklist_ComFalha_Deve_RetornarBadRequest()
    {
        // Arrange
        var request = new CreateChecklistRequest();
        var cancellationToken = new CancellationToken();

        _mediatorMock
            .Setup(x => x.Send(
                It.IsAny<CreateChecklistCommand>(),
                cancellationToken))
            .ReturnsAsync(
                Result.Failure("Não foi possível criar o checklist."));

        // Act
        var resultado = await _controller.CriarChecklist(
            request,
            cancellationToken);

        // Assert
        var badRequestResult =
            Assert.IsType<BadRequestObjectResult>(resultado.Result);

        Assert.Equal(
            StatusCodes.Status400BadRequest,
            badRequestResult.StatusCode);

        Assert.NotNull(badRequestResult.Value);

        _mediatorMock.Verify(
            x => x.Send(
                It.IsAny<CreateChecklistCommand>(),
                cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task FinalizarChecklist_ComSucesso_Deve_RetornarOk()
    {
        // Arrange
        var request = new FinalizarChecklistRequest();
        var cancellationToken = new CancellationToken();

        _mediatorMock
            .Setup(x => x.Send(
                It.IsAny<FinalizarChecklistCommand>(),
                cancellationToken))
            .ReturnsAsync(Result.Success());

        // Act
        var resultado = await _controller.FinalizarChecklist(
            request,
            cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultado.Result);

        Assert.Equal(
            StatusCodes.Status200OK,
            okResult.StatusCode);

        Assert.NotNull(okResult.Value);

        _mediatorMock.Verify(
            x => x.Send(
                It.IsAny<FinalizarChecklistCommand>(),
                cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task FinalizarChecklist_ComFalha_Deve_RetornarBadRequest()
    {
        // Arrange
        var request = new FinalizarChecklistRequest();
        var cancellationToken = new CancellationToken();

        _mediatorMock
            .Setup(x => x.Send(
                It.IsAny<FinalizarChecklistCommand>(),
                cancellationToken))
            .ReturnsAsync(
                Result.Failure("Checklist não encontrado."));

        // Act
        var resultado = await _controller.FinalizarChecklist(
            request,
            cancellationToken);

        // Assert
        var badRequestResult =
            Assert.IsType<BadRequestObjectResult>(resultado.Result);

        Assert.Equal(
            StatusCodes.Status400BadRequest,
            badRequestResult.StatusCode);

        Assert.NotNull(badRequestResult.Value);

        _mediatorMock.Verify(
            x => x.Send(
                It.IsAny<FinalizarChecklistCommand>(),
                cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task RemoverChecklist_ComSucesso_Deve_RetornarOk()
    {
        // Arrange
        var request = new RemoveChecklistRequest();
        var cancellationToken = new CancellationToken();

        _mediatorMock
            .Setup(x => x.Send(
                It.IsAny<RemoveChecklistCommand>(),
                cancellationToken))
            .ReturnsAsync(Result.Success());

        // Act
        var resultado = await _controller.RemoverChecklist(
            request,
            cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultado.Result);

        Assert.Equal(
            StatusCodes.Status200OK,
            okResult.StatusCode);

        Assert.NotNull(okResult.Value);

        _mediatorMock.Verify(
            x => x.Send(
                It.IsAny<RemoveChecklistCommand>(),
                cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task RemoverChecklist_ComFalha_Deve_RetornarBadRequest()
    {
        // Arrange
        var request = new RemoveChecklistRequest();
        var cancellationToken = new CancellationToken();

        _mediatorMock
            .Setup(x => x.Send(
                It.IsAny<RemoveChecklistCommand>(),
                cancellationToken))
            .ReturnsAsync(
                Result.Failure("Não foi possível remover o checklist."));

        // Act
        var resultado = await _controller.RemoverChecklist(
            request,
            cancellationToken);

        // Assert
        var badRequestResult =
            Assert.IsType<BadRequestObjectResult>(resultado.Result);

        Assert.Equal(
            StatusCodes.Status400BadRequest,
            badRequestResult.StatusCode);

        Assert.NotNull(badRequestResult.Value);

        _mediatorMock.Verify(
            x => x.Send(
                It.IsAny<RemoveChecklistCommand>(),
                cancellationToken),
            Times.Once);
    }
}