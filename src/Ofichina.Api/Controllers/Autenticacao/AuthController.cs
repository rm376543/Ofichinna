using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Ofichina.Contracts.Responses;
using Ofichina.Authentication.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Ofichina.Contracts.Enums;
using Ofichina.Contracts.Requests.Autenticacao;
using Ofichina.Contracts.Requests.Cliente;
using Ofichina.Contracts.Requests.Usuario;

namespace Ofichina.Api.Controllers.Autenticacao;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly IAutenticacaoService _autenticacaoService;
    private readonly IValidator<AutenticacaoRequest> _validator;
    private readonly IValidator<CreateClienteRequest> _cadastroValidator;

    public AuthController(
        IAutenticacaoService autenticacaoService,
        IValidator<AutenticacaoRequest> validator,
        IValidator<CreateClienteRequest> cadastroValidator)
    {
        _autenticacaoService = autenticacaoService;
        _validator = validator;
        _cadastroValidator = cadastroValidator;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AutenticacaoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<AutenticacaoResponse>>> LoginAsync(
        [FromBody] AutenticacaoRequest request,
        CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(request, cancellationToken);

        if (!validation.IsValid)
        {
            return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));
        }

        var result = await _autenticacaoService.AutenticarAsync(request, cancellationToken);

        if (!result.IsSuccess || result.Value is null)
        {
            return Unauthorized(ApiResponse.FailureResponse(result.Error ?? "Credenciais inválidas."));
        }

        return Ok(ApiResponse<AutenticacaoResponse>.SuccessResponse(result.Value, "Autenticação realizada com sucesso."));
    }

    [AllowAnonymous]
    //[Authorize(Roles = PerfilSistemaEnum.Administrador)]
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<AutenticacaoResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<AutenticacaoResponse>>> RegisterAsync(
        [FromBody] CreateClienteRequest request,
        CancellationToken cancellationToken)
    {
        var validation = await _cadastroValidator.ValidateAsync(request, cancellationToken);

        if (!validation.IsValid)
        {
            return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));
        }

        var result = await _autenticacaoService.CadastrarAsync(request, cancellationToken);

        if (!result.IsSuccess || result.Value is null)
        {
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível concluir o cadastro."));
        }

        return StatusCode(StatusCodes.Status201Created, ApiResponse<AutenticacaoResponse>.SuccessResponse(result.Value, "Cadastro realizado com sucesso."));
    }
}