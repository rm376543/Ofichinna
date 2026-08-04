using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ofichina.Application.UseCases.Checklists.Commands;
using Ofichina.Contracts;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses;
using Ofichina.Contracts.Requests.Checklist;

namespace Ofichina.Api.Controllers.Checklist;

/// <summary>
/// Controller responsável pelos checklists.
/// </summary>
[Authorize]
[ApiController]
[Route("api/checklists")]
public sealed class ChecklistController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ChecklistController> _logger;

    public ChecklistController(IMediator mediator, ILogger<ChecklistController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [Authorize(Roles = "ADMIN")]
    [HttpPost]
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

    [Authorize(Roles = "ADMIN")]
    [HttpPut("{id:guid}/finalizar")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> FinalizarChecklist(
        Guid id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a finalização do checklist com Id: {Id}.", id);

        var result = await _mediator.Send(new FinalizarChecklistCommand(new FinalizarChecklistRequest { Id = id }), cancellationToken);

        if (!result.IsSuccess)
        {
            if ((result.Error ?? string.Empty).Contains("não encontrado", StringComparison.OrdinalIgnoreCase))
                return NotFound(ApiResponse.FailureResponse(result.Error ?? "Checklist não encontrado."));

            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível finalizar o checklist."));
        }

        return Ok(ApiResponse.SuccessResponse("Checklist finalizado com sucesso."));
    }
}