using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ofichina.Application.UseCases.Checklists.Commands;
using Ofichina.Contracts.Requests.Checklist;
using Ofichina.Contracts.Responses;

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
    [HttpPost("novo")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse>> CriarChecklist(
        [FromBody] CreateChecklistRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a criação de um checklist. VeiculoId: {VeiculoId}, PessoaId: {PessoaId}.", request.VeiculoId, request.PessoaId);

        var result = await _mediator.Send(new CreateChecklistCommand(request), cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível criar o checklist."));

        return StatusCode(StatusCodes.Status201Created, ApiResponse.SuccessResponse("Checklist criado com sucesso."));
    }

    /// <summary>
    /// Finaliza um checklist existente.
    /// </summary>
    /// <param name="request">Identificador do checklist a ser finalizado.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Mensagem de sucesso, erro de validação ou checklist não encontrado.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPut("{checklistId:guid}/finalizar")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> FinalizarChecklist(
        [FromRoute] FinalizarChecklistRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a finalização do checklist com Id: {Id}.", request.Id);

        var result = await _mediator.Send(new FinalizarChecklistCommand(request), cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível finalizar o checklist."));
        }

        return Ok(ApiResponse.SuccessResponse("Checklist finalizado com sucesso."));
    }
}