using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Ofichina.Api.Controllers.Autenticacao;
using Ofichina.Application.UseCases.Autenticacao.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.Autenticacao;
using Ofichina.Contracts.Requests.Usuario;
using Ofichina.Contracts.Responses.Authentication;
using System.Runtime.CompilerServices;

namespace Ofichina.UnitTests.Ofichina.Api.Controllers.Autenticacao;

public sealed class AuthControllerTests
{
    private readonly Mock<IValidator<AutenticacaoRequest>> _loginValidatorMock;
    private readonly Mock<IValidator<CadastrarUsuarioRequest>> _registerValidatorMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ILogger<AuthController>> _loggerMock;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _loginValidatorMock = new Mock<IValidator<AutenticacaoRequest>>();
        _registerValidatorMock = new Mock<IValidator<CadastrarUsuarioRequest>>();
        _mediatorMock = new Mock<IMediator>();
        _loggerMock = new Mock<ILogger<AuthController>>();

        _controller = new AuthController(
            _loginValidatorMock.Object,
            _registerValidatorMock.Object,
            _mediatorMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public void Constructor_DependenciasValidas_Deve_CriarController()
    {
        // Act
        var controller = new AuthController(
            _loginValidatorMock.Object,
            _registerValidatorMock.Object,
            _mediatorMock.Object,
            _loggerMock.Object);

        // Assert
        Assert.NotNull(controller);
    }

    [Fact]
    public async Task LoginAsync_ValidacaoInvalida_Deve_RetornarBadRequest()
    {
        // Arrange
        var request = new AutenticacaoRequest
        {
            Email = "email-invalido",
            Senha = ""
        };

        var erros = new[]
        {
            new ValidationFailure("Email", "Email inválido."),
            new ValidationFailure("Senha", "Senha é obrigatória.")
        };

        ConfigurarValidacaoLogin(new ValidationResult(erros));

        // Act
        var resultado = await _controller.LoginAsync(request, CancellationToken.None);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(resultado.Result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequest.StatusCode);
        Assert.NotNull(badRequest.Value);

        _mediatorMock.Verify(
            x => x.Send(
                It.IsAny<AutenticarCommand>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task LoginAsync_CredenciaisValidasEAutenticacaoComSucesso_Deve_RetornarOk()
    {
        // Arrange
        var request = new AutenticacaoRequest
        {
            Email = "usuario@teste.com",
            Senha = "Senha@123"
        };

        var authenticationResponse = CriarAuthenticationResponse();

        ConfigurarValidacaoLoginValida();

        _mediatorMock
            .Setup(x => x.Send(
                It.Is<AutenticarCommand>(command =>
                    command.Email == request.Email &&
                    command.Senha == request.Senha),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AuthenticationResponse>.Success(authenticationResponse));

        // Act
        var resultado = await _controller.LoginAsync(request, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.NotNull(okResult.Value);

        _mediatorMock.Verify(
            x => x.Send(
                It.Is<AutenticarCommand>(command =>
                    command.Email == request.Email &&
                    command.Senha == request.Senha),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task LoginAsync_AutenticacaoComFalhaEErroInformado_Deve_RetornarUnauthorizedComErro()
    {
        // Arrange
        var request = new AutenticacaoRequest
        {
            Email = "usuario@teste.com",
            Senha = "SenhaErrada"
        };

        ConfigurarValidacaoLoginValida();

        _mediatorMock
            .Setup(x => x.Send(
                It.IsAny<AutenticarCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                Result<AuthenticationResponse>.Failure("Usuário ou senha inválidos."));

        // Act
        var resultado = await _controller.LoginAsync(request, CancellationToken.None);

        // Assert
        var unauthorized = Assert.IsType<UnauthorizedObjectResult>(resultado.Result);
        Assert.Equal(StatusCodes.Status401Unauthorized, unauthorized.StatusCode);
        Assert.NotNull(unauthorized.Value);
    }

    [Fact]
    public async Task LoginAsync_AutenticacaoComFalhaESemErro_Deve_RetornarUnauthorizedComMensagemPadrao()
    {
        // Arrange
        var request = new AutenticacaoRequest
        {
            Email = "usuario@teste.com",
            Senha = "SenhaErrada"
        };

        ConfigurarValidacaoLoginValida();

        var result = new Result<AuthenticationResponse>(
            false,
            CriarAuthenticationResponse(),
            null);

        _mediatorMock
            .Setup(x => x.Send(
                It.IsAny<AutenticarCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var resultado = await _controller.LoginAsync(request, CancellationToken.None);

        // Assert
        var unauthorized = Assert.IsType<UnauthorizedObjectResult>(resultado.Result);
        Assert.Equal(StatusCodes.Status401Unauthorized, unauthorized.StatusCode);
        Assert.NotNull(unauthorized.Value);
    }

    [Fact]
    public async Task LoginAsync_AutenticacaoComSucessoMasValueNulo_Deve_RetornarUnauthorized()
    {
        // Arrange
        var request = new AutenticacaoRequest
        {
            Email = "usuario@teste.com",
            Senha = "Senha@123"
        };

        ConfigurarValidacaoLoginValida();

        var result = new Result<AuthenticationResponse>(
            true,
            null!,
            "Resposta de autenticação não encontrada.");

        _mediatorMock
            .Setup(x => x.Send(
                It.IsAny<AutenticarCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var resultado = await _controller.LoginAsync(request, CancellationToken.None);

        // Assert
        var unauthorized = Assert.IsType<UnauthorizedObjectResult>(resultado.Result);
        Assert.Equal(StatusCodes.Status401Unauthorized, unauthorized.StatusCode);
        Assert.NotNull(unauthorized.Value);
    }

    [Fact]
    public async Task RegisterAsync_ValidacaoInvalida_Deve_RetornarBadRequest()
    {
        // Arrange
        var request = new CadastrarUsuarioRequest
        {
            Email = "email-invalido",
            Senha = ""
        };

        var erros = new[]
        {
            new ValidationFailure("Email", "Email inválido."),
            new ValidationFailure("Senha", "Senha é obrigatória.")
        };

        ConfigurarValidacaoCadastro(new ValidationResult(erros));

        // Act
        var resultado = await _controller.RegisterAsync(request, CancellationToken.None);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(resultado.Result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequest.StatusCode);
        Assert.NotNull(badRequest.Value);

        _mediatorMock.Verify(
            x => x.Send(
                It.IsAny<CadastrarUsuarioCommand>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_CadastroComSucesso_Deve_RetornarCreated()
    {
        // Arrange
        var request = new CadastrarUsuarioRequest
        {
            Email = "novo.usuario@teste.com",
            Senha = "Senha@123"
        };

        var authenticationResponse = CriarAuthenticationResponse();

        ConfigurarValidacaoCadastroValida();

        _mediatorMock
            .Setup(x => x.Send(
                It.Is<CadastrarUsuarioCommand>(command =>
                    command.Email == request.Email &&
                    command.Senha == request.Senha),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AuthenticationResponse>.Success(authenticationResponse));

        // Act
        var resultado = await _controller.RegisterAsync(request, CancellationToken.None);

        // Assert
        var created = Assert.IsType<ObjectResult>(resultado.Result);
        Assert.Equal(StatusCodes.Status201Created, created.StatusCode);
        Assert.NotNull(created.Value);

        _mediatorMock.Verify(
            x => x.Send(
                It.Is<CadastrarUsuarioCommand>(command =>
                    command.Email == request.Email &&
                    command.Senha == request.Senha),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_CadastroComFalhaEErroInformado_Deve_RetornarBadRequest()
    {
        // Arrange
        var request = new CadastrarUsuarioRequest
        {
            Email = "usuario@teste.com",
            Senha = "Senha@123"
        };

        ConfigurarValidacaoCadastroValida();

        _mediatorMock
            .Setup(x => x.Send(
                It.IsAny<CadastrarUsuarioCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                Result<AuthenticationResponse>.Failure("Usuário já cadastrado."));

        // Act
        var resultado = await _controller.RegisterAsync(request, CancellationToken.None);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(resultado.Result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequest.StatusCode);
        Assert.NotNull(badRequest.Value);
    }

    [Fact]
    public async Task RegisterAsync_CadastroComFalhaESemErro_Deve_RetornarBadRequestComMensagemPadrao()
    {
        // Arrange
        var request = new CadastrarUsuarioRequest
        {
            Email = "usuario@teste.com",
            Senha = "Senha@123"
        };

        ConfigurarValidacaoCadastroValida();

        var result = new Result<AuthenticationResponse>(
            false,
            CriarAuthenticationResponse(),
            null);

        _mediatorMock
            .Setup(x => x.Send(
                It.IsAny<CadastrarUsuarioCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var resultado = await _controller.RegisterAsync(
            request,
            CancellationToken.None);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(resultado.Result);

        Assert.Equal(
            StatusCodes.Status400BadRequest,
            badRequest.StatusCode);

        Assert.NotNull(badRequest.Value);
    }

    [Fact]
    public async Task RegisterAsync_CadastroComSucessoMasValueNulo_Deve_RetornarBadRequest()
    {
        // Arrange
        var request = new CadastrarUsuarioRequest
        {
            Email = "usuario@teste.com",
            Senha = "Senha@123"
        };

        ConfigurarValidacaoCadastroValida();

        var result = new Result<AuthenticationResponse>(
            true,
            null!,
            "Resposta de autenticação não encontrada.");

        _mediatorMock
            .Setup(x => x.Send(
                It.IsAny<CadastrarUsuarioCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var resultado = await _controller.RegisterAsync(request, CancellationToken.None);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(resultado.Result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequest.StatusCode);
        Assert.NotNull(badRequest.Value);
    }

    private void ConfigurarValidacaoLogin(ValidationResult resultado)
    {
        _loginValidatorMock
            .Setup(x => x.ValidateAsync(
                It.IsAny<AutenticacaoRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(resultado);
    }

    private void ConfigurarValidacaoLoginValida()
    {
        ConfigurarValidacaoLogin(new ValidationResult());
    }

    private void ConfigurarValidacaoCadastro(ValidationResult resultado)
    {
        _registerValidatorMock
            .Setup(x => x.ValidateAsync(
                It.IsAny<CadastrarUsuarioRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(resultado);
    }

    private void ConfigurarValidacaoCadastroValida()
    {
        ConfigurarValidacaoCadastro(new ValidationResult());
    }

    private static AuthenticationResponse CriarAuthenticationResponse()
    {
        return (AuthenticationResponse)RuntimeHelpers.GetUninitializedObject(
            typeof(AuthenticationResponse));
    }
}