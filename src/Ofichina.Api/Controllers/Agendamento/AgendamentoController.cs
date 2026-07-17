using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Agendamentos.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.Agendamento;
using Ofichina.Contracts.Responses;
using Ofichina.Contracts.Responses.Agendamento;

namespace Ofichina.Api.Controllers.Agendamento;

[Authorize]
[ApiController]
[Route("api/agendamentos")]
public sealed class AgendamentoController : ControllerBase
{
    private readonly IValidator<CreateAgendamentoRequest> _validator;
    private readonly ICommandHandler<CreateAgendamentoCommand, Result<AgendamentoResponse>> _createHandler;
    private readonly ILogger<AgendamentoController> _logger;

    public AgendamentoController(
        IValidator<CreateAgendamentoRequest> validator,
        ICommandHandler<CreateAgendamentoCommand, Result<AgendamentoResponse>> createHandler,
        ILogger<AgendamentoController> logger)
    {
        _validator = validator;
        _createHandler = createHandler;
        _logger = logger;
    }

    /// <summary>
    /// Cria um novo agendamento para o usuário autenticado.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<AgendamentoResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<AgendamentoResponse>>> CriarAsync(
        [FromBody] CreateAgendamentoRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a criação de agendamento para o veículo {VeiculoId}.", request.VeiculoId);

        var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));

        var result = await _createHandler.HandleAsync(new CreateAgendamentoCommand
        {
            VeiculoId = request.VeiculoId,
            DataHoraPreferida = request.DataHoraPreferida,
            Motivo = request.Motivo,
            Observacoes = request.Observacoes
        });

        if (!result.IsSuccess || result.Value is null)
        {
            var error = result.Error ?? "Não foi possível criar o agendamento.";

            _logger.LogWarning("Falha ao criar agendamento. VeiculoId: {VeiculoId}, Erro: {Erro}", request.VeiculoId, error);

            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Falha ao criar agendamento."));

        }

        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<AgendamentoResponse>.SuccessResponse(result.Value, "Agendamento criado com sucesso."));
    }
}