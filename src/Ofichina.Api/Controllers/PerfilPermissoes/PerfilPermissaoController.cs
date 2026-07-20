using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ofichina.Application.UseCases.PerfilPermissoes.Commands;
using Ofichina.Application.UseCases.PerfilPermissoes.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.PerfilPermissoes;
using Ofichina.Contracts.Responses;
using Ofichina.Contracts.Responses.PerfilPermissoes;

namespace Ofichina.Api.Controllers.PerfilPermissoes;

[Authorize]
[ApiController]
[Route("api/perfil/{perfilId:guid}/permissao")]
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

    [Authorize(Roles = "ADMIN")]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> VincularAsync(Guid perfilId, [FromBody] VincularPermissaoPerfilRequest request, CancellationToken cancellationToken)
    {
        request.PerfilId = perfilId;

        var validation = await _vincularValidator.ValidateAsync(request, cancellationToken);

        if (!validation.IsValid)
            return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));

        var result = await _mediator.Send(new VincularPermissaoPerfilCommand(request.PerfilId, request.PermissaoId), cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.Error is "Perfil não encontrado." or "Permissão não encontrada.")
                return NotFound(ApiResponse.FailureResponse(result.Error));

            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível vincular a permissão ao perfil."));
        }

        return Ok(ApiResponse.SuccessResponse("Permissão vinculada ao perfil com sucesso."));
    }

    [Authorize(Roles = "ADMIN")]
    [HttpDelete("{permissaoId:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> DesvincularAsync(Guid perfilId, Guid permissaoId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DesvincularPermissaoPerfilCommand(perfilId, permissaoId), cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.Error == "Vínculo entre perfil e permissão não encontrado.")
                return NotFound(ApiResponse.FailureResponse(result.Error));

            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível desvincular a permissão do perfil."));
        }

        return Ok(ApiResponse.SuccessResponse("Permissão desvinculada do perfil com sucesso."));
    }

    [Authorize(Roles = "ADMIN")]
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<PerfilPermissaoResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<PerfilPermissaoResponse>>>> GetAllAsync(Guid perfilId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetPermissoesDoPerfilQuery(perfilId), cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.Error == "Perfil não encontrado.")
                return NotFound(ApiResponse.FailureResponse(result.Error));

            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível obter as permissões do perfil."));
        }

        return Ok(ApiResponse<IReadOnlyCollection<PerfilPermissaoResponse>>.SuccessResponse(result.Value ?? []));
    }
}
