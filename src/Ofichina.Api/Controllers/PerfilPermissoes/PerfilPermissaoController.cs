using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ofichina.Application.UseCases.PerfilPermissoes.Commands;
using Ofichina.Application.UseCases.PerfilPermissoes.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.PerfilPermissoes;
using Ofichina.Contracts.Responses.PerfilPermissoes;

namespace Ofichina.Api.Controllers.PerfilPermissoes;

[Authorize]
[ApiController]
[Route("api/perfil-permissao")]
public sealed class PerfilPermissaoController : ControllerBase
{
    private readonly IValidator<VincularPermissaoPerfilRequest> _vincularValidator;
    private readonly IMediator _mediator;
    private readonly ILogger<PerfilPermissaoController> _logger;

    public PerfilPermissaoController(
        IValidator<VincularPermissaoPerfilRequest> vincularValidator,
        IMediator mediator,
        ILogger<PerfilPermissaoController> logger)
    {
        _vincularValidator = vincularValidator;
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Retorna todas as permissões de um perfil específico.
    /// </summary>
    /// <param name="pagination">Parâmetros de paginação.</param>
    /// <param name="perfilId">Identificador do perfil.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Lista de permissões do perfil.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpGet("listar")]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<PerfilPermissaoResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<PagedResponse<PerfilPermissaoResponse>>>> GetAllPerfisPermissoesPaginadas(
        [FromQuery] Pagination pagination, Guid perfilId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando o processo de obtenção das permissões do perfil {PerfilId}.", perfilId);
        var result = await _mediator.Send(new GetAllPerfisPermissoesPaginadasQuery(perfilId, pagination), cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Falha ao obter as permissões do perfil {PerfilId}. Erro: {Erro}", perfilId, result.Error);
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível obter as permissões do perfil."));
        }

        return Ok(ApiResponse<PagedResponse<PerfilPermissaoResponse>>.SuccessResponse(result.Value));
    }

    /// <summary>
    /// Vincula uma permissão a um perfil específico.
    /// </summary>
    /// <param name="request">Dados da permissão a ser vinculada.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Resultado da operação.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPost("vincular")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> VincularAsync([FromBody] VincularPermissaoPerfilRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando o processo de vinculação da permissão {PermissaoId} ao perfil {PerfilId}.", request.PermissaoId, request.PerfilId);

        var validation = await _vincularValidator.ValidateAsync(request, cancellationToken);

        if (!validation.IsValid)
        {
            _logger.LogWarning("Validação falhou para a requisição de vinculação da permissão {PermissaoId} ao perfil {PerfilId}. Erros: {Errors}", request.PermissaoId, request.PerfilId, validation.Errors.Select(x => x.ErrorMessage));
            return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));
        }

        var result = await _mediator.Send(new VincularPermissaoPerfilCommand(request.PerfilId, request.PermissaoId), cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Falha ao vincular a permissão {PermissaoId} ao perfil {PerfilId}. Erro: {Erro}", request.PermissaoId, request.PerfilId, result.Error);
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível vincular a permissão ao perfil."));
        }

        return Ok(ApiResponse.SuccessResponse("Permissão vinculada ao perfil com sucesso."));
    }

    /// <summary>
    /// Desvincula uma permissão de um perfil específico.
    /// </summary>
    /// <param name="request">Dados para desvincular a permissão do perfil.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Resultado da operação.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpDelete("remover")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> DesvincularAsync(
        [FromBody] DesvincularPerfilPermissao request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando o processo de desvinculação da permissão {PermissaoId} do perfil {PerfilId}.", request.PermissaoId, request.PerfilId);
        var result = await _mediator.Send(new DesvincularPermissaoPerfilCommand(request.PerfilId, request.PermissaoId), cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Falha ao desvincular a permissão {PermissaoId} do perfil {PerfilId}. Erro: {Erro}", request.PermissaoId, request.PerfilId, result.Error);
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível desvincular a permissão do perfil."));
        }

        _logger.LogInformation("Permissão {PermissaoId} desvinculada do perfil {PerfilId} com sucesso.", request.PermissaoId, request.PerfilId);
        return Ok(ApiResponse.SuccessResponse("Permissão desvinculada do perfil com sucesso."));
    }

}
