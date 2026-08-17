using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ofichina.Application.UseCases.Checklists.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.Checklist;

namespace Ofichina.Api.Controllers.Checklist;

/// <summary>
/// Controller responsável pelos checklists.
/// </summary>
[Authorize]
[ApiController]
[Route("api/checklist")]
public sealed class ChecklistController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ChecklistController> _logger;

    public ChecklistController(IMediator mediator, ILogger<ChecklistController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Cria um novo checklist.
    /// </summary>
    /// <param name="request">Dados necessários para criar o checklist.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Mensagem de sucesso ou erro de validação.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPost("adicionar")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse>> CriarChecklist(
        [FromBody] CreateChecklistRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a criação de um checklist. AgendamentoId: {AgendamentoId}.", request.AgendamentoId);

        var result = await _mediator.Send(new CreateChecklistCommand(request), cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível criar o checklist."));

        return StatusCode(StatusCodes.Status201Created, ApiResponse.SuccessResponse("Checklist criado com sucesso."));
    }

    /// <summary>
    /// Finaliza um checklist existente.
    /// </summary>
    /// <param name="request">Dados necessários para finalizar o checklist.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Mensagem de sucesso ou erro de validação.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPut("finalizar")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> FinalizarChecklist(
        [FromBody] FinalizarChecklistRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a finalização do checklist com Id: {Id}.", request.AgendamentoId);

        var result = await _mediator.Send(new FinalizarChecklistCommand(request), cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível finalizar o checklist."));
        }

        return Ok(ApiResponse.SuccessResponse("Checklist finalizado com sucesso."));
    }


    /// <summary>
    /// Remove logicamente um checklist vinculado ao agendamento existente.
    /// </summary>
    /// <param name="request">Dados necessários para remoção de um checklist cadastrado errado.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Resultado da operação.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpDelete("remover")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> RemoverChecklist(
        [FromBody] RemoveChecklistRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a remoção do Checklist com Id: {Id}", request.ChecklistId);

        var result = await _mediator.Send(new RemoveChecklistCommand(request), cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogError("Erro ao remover o Checklist com Id: {Id}. Erro: {Erro}", request.ChecklistId, result.Error);
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível remover o Checklist."));
        }

        _logger.LogInformation("Checklist com Id: {Id} removido com sucesso.", request.ChecklistId);

        return Ok(ApiResponse.SuccessResponse("Checklist removido com sucesso."));
    }

}