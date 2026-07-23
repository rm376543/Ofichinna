using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ofichina.Application.UseCases.Agendamentos.Commands;
using Ofichina.Application.UseCases.Agendamentos.Queries;
using Ofichina.Contracts.Requests.Agendamento;
using Ofichina.Contracts.Responses;
using Ofichina.Contracts.Responses.Agendamento;

namespace Ofichina.Api.Controllers.Agendamento;

[Authorize]
[ApiController]
[Route("api/agendamento")]
public sealed class AgendamentoController : ControllerBase
{
    private readonly IValidator<CreateAgendamentoRequest> _validator;
    private readonly IMediator _mediator;
    private readonly ILogger<AgendamentoController> _logger;

    public AgendamentoController(
        IValidator<CreateAgendamentoRequest> validator,
        IMediator mediator,
        ILogger<AgendamentoController> logger)
    {
        _validator = validator;
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Lista os agendamentos de uma pessoa específica.
    /// </summary>
    [HttpGet("pessoa/{pessoaId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<AgendamentoResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<AgendamentoResponse>>>> ListarAsync(Guid pessoaId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a listagem de agendamentos da pessoa {PessoaId}.", pessoaId);

        var result = await _mediator.Send(new GetAgendamentosQuery(pessoaId), cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível obter os agendamentos."));

        return Ok(ApiResponse<IReadOnlyCollection<AgendamentoResponse>>.SuccessResponse(result.Value ?? []));
    }

    /// <summary>
    /// Obtém um agendamento pelo identificador.
    /// </summary>
    [HttpGet("pessoa/{pessoaId:guid}/agendamento/{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<AgendamentoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<AgendamentoResponse>>> ObterPorIdAsync(Guid pessoaId, Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a obtenção do agendamento {Id} da pessoa {PessoaId}.", id, pessoaId);

        var result = await _mediator.Send(new GetAgendamentoByIdQuery(pessoaId, id), cancellationToken);

        if (!result.IsSuccess || result.Value is null)
            return NotFound(ApiResponse.FailureResponse(result.Error ?? "Agendamento não encontrado."));

        return Ok(ApiResponse<AgendamentoResponse>.SuccessResponse(result.Value));
    }

    /// <summary>
    /// Cria um novo agendamento para uma pessoa específica.
    /// </summary>
    [HttpPost("pessoa/{pessoaId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<AgendamentoResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<AgendamentoResponse>>> CriarAsync(
        Guid pessoaId,
        [FromBody] CreateAgendamentoRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a criação de agendamento para a pessoa {PessoaId} no dia {DiaDisponibilidadeId} e horário {HorarioConsultorId}.", pessoaId, request.DiaDisponibilidadeId, request.HorarioConsultorId);

        var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));

        var result = await _mediator.Send(new CreateAgendamentoCommand
        {
            PessoaId = pessoaId,
            DiaDisponibilidadeId = request.DiaDisponibilidadeId,
            HorarioConsultorId = request.HorarioConsultorId,
            VeiculoId = request.VeiculoId,
            Descricao = request.Descricao
        }, cancellationToken);

        if (!result.IsSuccess || result.Value is null)
        {
            var error = result.Error ?? "Não foi possível criar o agendamento.";

            _logger.LogWarning("Falha ao criar agendamento. PessoaId: {PessoaId}, DiaDisponibilidadeId: {DiaDisponibilidadeId}, HorarioConsultorId: {HorarioConsultorId}, Erro: {Erro}", pessoaId, request.DiaDisponibilidadeId, request.HorarioConsultorId, error);

            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Falha ao criar agendamento."));

        }

        return CreatedAtAction(
            nameof(ObterPorIdAsync),
            new { pessoaId, id = result.Value.Id },
            ApiResponse<AgendamentoResponse>.SuccessResponse(result.Value, "Agendamento criado com sucesso."));
    }
}


