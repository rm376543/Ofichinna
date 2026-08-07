using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ofichina.Application.UseCases.PerfilUsuario.Commands;
using Ofichina.Application.UseCases.PerfilUsuario.Queries;
using Ofichina.Contracts.Requests.PerfilUsuario;
using Ofichina.Contracts.Responses;

namespace Ofichina.Api.Controllers.PerfilUsuario;

[Authorize]
[ApiController]
[Route("api/perfil-usuario")]
public sealed class PerfilUsuarioController : ControllerBase
{
    private readonly IValidator<VincularPerfilUsuarioRequest> _vincularValidator;
    private readonly IMediator _mediator;
    private readonly ILogger<PerfilUsuarioController> _logger;

    public PerfilUsuarioController(
        IValidator<VincularPerfilUsuarioRequest> vincularValidator,
        IMediator mediator,
        ILogger<PerfilUsuarioController> logger)
    {
        _vincularValidator = vincularValidator;
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Retorna todos os perfis vinculados a um usuário.
    /// </summary>
    /// <param name="usuarioId">Identificador do usuário.</param>
    /// <returns>Lista com os códigos dos perfis vinculados ao usuário.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpGet("{usuarioId:guid}/listar")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<string>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<string>>>> ObterPerfisAsync([FromRoute] Guid usuarioId)
    {
        _logger.LogInformation("Consultando perfis do usuário. UsuarioId: {UsuarioId}", usuarioId);

        var perfis = await _mediator.Send(new ObterPerfisDoUsuarioQuery(usuarioId), HttpContext.RequestAborted);

        _logger.LogInformation("Perfis obtidos com sucesso. UsuarioId: {UsuarioId}, Quantidade: {Quantidade}", usuarioId, perfis.Count);

        return Ok(ApiResponse<IReadOnlyCollection<string>>.SuccessResponse(perfis));
    }

    /// <summary>
    /// Vincula um perfil a um usuario, através do identificador
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPost("vincular")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse>> VincularAsync(
        [FromBody] VincularPerfilUsuarioRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando vinculação de perfil. UsuarioId: {UsuarioId}, PerfilId: {PerfilId}", request.UsuarioId, request.PerfilId);

        var validation = await _vincularValidator.ValidateAsync(request, cancellationToken);

        if (!validation.IsValid)
        {
            _logger.LogWarning("Validação falhou ao vincular perfil. UsuarioId: {UsuarioId}, PerfilId: {PerfilId}, Erros: {Errors}",
                request.UsuarioId, request.PerfilId, string.Join(", ", validation.Errors.Select(x => x.ErrorMessage)));
            return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));
        }

        var result = await _mediator.Send(new VincularPerfilUsuarioCommand(request), cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogError("Falha ao vincular perfil. UsuarioId: {UsuarioId}, PerfilId: {PerfilId}, Erro: {Error}",
                request.UsuarioId, request.PerfilId, result.Error);
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível vincular o perfil."));
        }

        _logger.LogInformation("Perfil vinculado com sucesso. UsuarioId: {UsuarioId}, PerfilId: {PerfilId}", request.UsuarioId, request.PerfilId);
        return Ok(ApiResponse.SuccessResponse(result.Value?.Mensagem ?? "Perfil vinculado com sucesso."));
    }
}


