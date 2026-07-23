using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ofichina.Application.UseCases.ServicosPecas.Commands;
using Ofichina.Contracts.Requests.Servicos;
using Ofichina.Contracts.Responses;

namespace Ofichina.Api.Controllers.ServicosPecas;

/// <summary>
/// Controller responsável pelo vínculo de peças com serviços.
/// </summary>
[Authorize]
[ApiController]
[Route("api/servicos/pecas")]
public sealed class ServicosPecasController : ControllerBase
{
    private readonly IValidator<CreateServicoPecaRequest> _createValidator;
    private readonly IMediator _mediator;
    private readonly ILogger<ServicosPecasController> _logger;

    public ServicosPecasController(
        IValidator<CreateServicoPecaRequest> createValidator,
        IMediator mediator,
        ILogger<ServicosPecasController> logger)
    {
        _createValidator = createValidator;
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Adiciona uma peça a um serviço.
    /// </summary>
    [Authorize(Roles = "ADMIN")]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<Guid>>> AdicionarPecaAoServico(
        [FromBody] CreateServicoPecaRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a inclusão de peça no serviço. ServicoId: {ServicoId}, PecaId: {PecaId}, Quantidade: {Quantidade}.", request.ServicoId, request.PecaId, request.Quantidade);

        var validation = await _createValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));

        var result = await _mediator.Send(new CreateServicoPecaCommand
        {
            ServicoId = request.ServicoId,
            PecaId = request.PecaId,
            Quantidade = request.Quantidade
        }, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Falha ao adicionar peça ao serviço. ServicoId: {ServicoId}, PecaId: {PecaId}. Erro: {Erro}", request.ServicoId, request.PecaId, result.Error);
            return MapCreateError(result.Error, "Não foi possível adicionar a peça ao serviço.");
        }

        return StatusCode(StatusCodes.Status201Created, ApiResponse<Guid>.SuccessResponse(result.Value, "Peça adicionada ao serviço com sucesso."));
    }

    /// <summary>
    /// Desativa uma peça específica do serviço.
    /// </summary>
    [Authorize(Roles = "ADMIN")]
    [HttpDelete("~/api/servicos/{servicoId:guid}/pecas/{pecaServicoId:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse>> DesativarPeca(
        [FromRoute] Guid servicoId,
        [FromRoute] Guid pecaServicoId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a desativação da peça do serviço. ServicoId: {ServicoId}, ServicoPecaId: {ServicoPecaId}.", servicoId, pecaServicoId);

        var result = await _mediator.Send(new DeleteServicoPecaCommand
        {
            ServicoId = servicoId,
            ServicoPecaId = pecaServicoId

        }, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Falha ao desativar peça do serviço. ServicoId: {ServicoId}, ServicoPecaId: {ServicoPecaId}. Erro: {Erro}", servicoId, pecaServicoId, result.Error);
            return MapError(result.Error, "Não foi possível desativar a peça do serviço.");
        }

        return Ok(ApiResponse.SuccessResponse("Peça do serviço desativada com sucesso."));
    }

    /// <summary>
    /// Desativa todas as peças vinculadas ao serviço.
    /// </summary>
    [Authorize(Roles = "ADMIN")]
    [HttpDelete("~/api/servicos/{servicoId:guid}/pecas")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse>> DesativarTodasAsPecas(
        [FromRoute] Guid servicoId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a desativação de todas as peças do serviço. ServicoId: {ServicoId}.", servicoId);

        var result = await _mediator.Send(new DeleteAllServicoPecasCommand
        {
            ServicoId = servicoId
        }, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Falha ao desativar peças do serviço. ServicoId: {ServicoId}. Erro: {Erro}", servicoId, result.Error);
            return MapError(result.Error, "Não foi possível desativar as peças do serviço.");
        }

        return Ok(ApiResponse.SuccessResponse("Peças do serviço desativadas com sucesso."));
    }

    private ActionResult<ApiResponse> MapError(string? error, string defaultMessage)
    {
        return error switch
        {
            "Serviço não encontrado." or "Peça não encontrada." => NotFound(ApiResponse.FailureResponse(error)),
            "Não é possível remover uma peça já utilizada." => Conflict(ApiResponse.FailureResponse(error)),
            _ => BadRequest(ApiResponse.FailureResponse(error ?? defaultMessage))
        };
    }

    private ActionResult<ApiResponse<Guid>> MapCreateError(string? error, string defaultMessage)
    {
        return error switch
        {
            "Serviço não encontrado." or "Peça não encontrada." => NotFound(ApiResponse.FailureResponse(error)),
            "A peça já foi adicionada ao serviço." => Conflict(ApiResponse.FailureResponse(error)),
            _ => BadRequest(ApiResponse.FailureResponse(error ?? defaultMessage))
        };
    }
}