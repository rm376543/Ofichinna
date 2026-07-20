using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ofichina.Application.UseCases.Autenticacao.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.Autenticacao;
using Ofichina.Contracts.Requests.Usuario;
using Ofichina.Contracts.Responses;

namespace Ofichina.Api.Controllers.Autenticacao;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IValidator<AutenticacaoRequest> _loginValidator;
    private readonly IValidator<CadastrarUsuarioRequest> _registerValidator;
    private readonly IMediator _mediator;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IValidator<AutenticacaoRequest> loginValidator,
        IValidator<CadastrarUsuarioRequest> registerValidator,
        IMediator mediator,
        ILogger<AuthController> logger)
    {
        _loginValidator = loginValidator;
        _registerValidator = registerValidator;
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Realiza o login do cliente.
    /// </summary>
    /// <param name="request">Dados para criar o usuario do cliente.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Mensagem de sucesso, erro de validação ou erro ao concluir cadastro.</returns>
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AutenticacaoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AutenticacaoResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<AutenticacaoResponse>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<AutenticacaoResponse>>> LoginAsync(
        [FromBody] AutenticacaoRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando login. Email: {Email}", request.Email);

        var validation = await _loginValidator.ValidateAsync(request, cancellationToken);

        if (!validation.IsValid)
        {
            _logger.LogWarning(
                "Validação inválida no login. Email: {Email}, Erros: {Errors}",
                request.Email,
                string.Join(", ", validation.Errors.Select(x => x.ErrorMessage)));

            return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));
        }

        var result = await _mediator.Send(new AutenticarCommand(request.Email, request.Senha), cancellationToken);

        if (!result.IsSuccess || result.Value is null)
        {
            _logger.LogWarning(
                "Login negado. Email: {Email}, Motivo: {Reason}",
                request.Email,
                result.Error ?? "Credenciais inválidas.");

            return Unauthorized(ApiResponse.FailureResponse(result.Error ?? "Credenciais inválidas."));
        }

        _logger.LogInformation("Login realizado com sucesso. Email: {Email}", request.Email);

        return Ok(ApiResponse<AutenticacaoResponse>.SuccessResponse(result.Value, "Autenticação realizada com sucesso."));
    }

    /// <summary>
    /// Cria um login para o cliente.
    /// </summary>
    /// <param name="request">Dados para criar o usuario do cliente.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Mensagem de sucesso, erro de validação ou erro ao concluir cadastro.</returns>
    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<AutenticacaoResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<AutenticacaoResponse>>> RegisterAsync(
        [FromBody] CadastrarUsuarioRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando cadastro de usuário. Email: {Email}", request.Email);

        var validation = await _registerValidator.ValidateAsync(request, cancellationToken);

        if (!validation.IsValid)
        {
            _logger.LogWarning(
                "Validação inválida no cadastro. Email: {Email}, Erros: {Errors}",
                request.Email,
                string.Join(", ", validation.Errors.Select(x => x.ErrorMessage)));

            return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));
        }

        var result = await _mediator.Send(
            new CadastrarUsuarioCommand(request.Email, request.Senha),
            cancellationToken);

        if (!result.IsSuccess || result.Value is null)
        {
            _logger.LogWarning(
                "Cadastro não concluído. Email: {Email}, Motivo: {Reason}",
                request.Email,
                result.Error ?? "Não foi possível concluir o cadastro.");

            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível concluir o cadastro."));
        }

        _logger.LogInformation("Cadastro realizado com sucesso. Email: {Email}", request.Email);

        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<AutenticacaoResponse>.SuccessResponse(result.Value, "Cadastro realizado com sucesso."));
    }
}


