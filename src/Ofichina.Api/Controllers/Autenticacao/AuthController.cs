using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ofichina.Application.Abstractions;
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
    private readonly IValidator<CreateClienteRequest> _registerValidator;
    private readonly ICommandHandler<AutenticarCommand, Result<AutenticacaoResponse>> _loginHandler;
    private readonly ICommandHandler<CadastrarClienteCommand, Result<AutenticacaoResponse>> _registerHandler;

    public AuthController(
        IValidator<AutenticacaoRequest> loginValidator,
        IValidator<CreateClienteRequest> registerValidator,
        ICommandHandler<AutenticarCommand, Result<AutenticacaoResponse>> loginHandler,
        ICommandHandler<CadastrarClienteCommand, Result<AutenticacaoResponse>> registerHandler)
    {
        _loginValidator = loginValidator;
        _registerValidator = registerValidator;
        _loginHandler = loginHandler;
        _registerHandler = registerHandler;
    }

    /// <summary>
    /// Realiza o login do cliente.
    /// </summary>
    /// <param name="request">Dados de autenticacao do cliente.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Mensagem de sucesso, erro de validação ou acesso negado.</returns>
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AutenticacaoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<AutenticacaoResponse>>> LoginAsync(
        [FromBody] AutenticacaoRequest request,
        CancellationToken cancellationToken)
    {
        var validation = await _loginValidator.ValidateAsync(request, cancellationToken);

        if (!validation.IsValid)
        {
            return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));
        }

        var result = await _loginHandler.HandleAsync(new AutenticarCommand(request.Email, request.Senha));

        if (!result.IsSuccess || result.Value is null)
        {
            return Unauthorized(ApiResponse.FailureResponse(result.Error ?? "Credenciais inválidas."));
        }

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
        [FromBody] CreateClienteRequest request,
        CancellationToken cancellationToken)
    {
        var validation = await _registerValidator.ValidateAsync(request, cancellationToken);

        if (!validation.IsValid)
        {
            return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));
        }

        var result = await _registerHandler.HandleAsync(
            new CadastrarClienteCommand(request.Nome, request.Email, request.Senha));

        if (!result.IsSuccess || result.Value is null)
        {
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível concluir o cadastro."));
        }

        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<AutenticacaoResponse>.SuccessResponse(result.Value, "Cadastro realizado com sucesso."));
    }
}