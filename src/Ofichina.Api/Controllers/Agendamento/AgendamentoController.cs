using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Agendamentos.Commands;
using Ofichina.Application.UseCases.Agendamentos.Queries;
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
    private readonly IQueryHandler<GetAgendamentosQuery, Result<IReadOnlyCollection<AgendamentoResponse>>> _getAllHandler;
    private readonly IQueryHandler<GetAgendamentoByIdQuery, Result<AgendamentoResponse>> _getByIdHandler;
    private readonly ILogger<AgendamentoController> _logger;

    public AgendamentoController(
        IValidator<CreateAgendamentoRequest> validator,
        ICommandHandler<CreateAgendamentoCommand, Result<AgendamentoResponse>> createHandler,
        IQueryHandler<GetAgendamentosQuery, Result<IReadOnlyCollection<AgendamentoResponse>>> getAllHandler,
        IQueryHandler<GetAgendamentoByIdQuery, Result<AgendamentoResponse>> getByIdHandler,
        ILogger<AgendamentoController> logger)
    {
        _validator = validator;
        _createHandler = createHandler;
        _getAllHandler = getAllHandler;
        _getByIdHandler = getByIdHandler;
        _logger = logger;
    }

    /// <summary>
    /// Lista os agendamentos de uma pessoa especÃ­fica.
    /// </summary>
    [HttpGet("pessoa/{pessoaId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<AgendamentoResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<AgendamentoResponse>>>> ListarAsync(Guid pessoaId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a listagem de agendamentos da pessoa {PessoaId}.", pessoaId);

        var result = await _getAllHandler.HandleAsync(new GetAgendamentosQuery(pessoaId));

        if (!result.IsSuccess)
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "NÃ£o foi possÃ­vel obter os agendamentos."));

        return Ok(ApiResponse<IReadOnlyCollection<AgendamentoResponse>>.SuccessResponse(result.Value ?? []));
    }

    /// <summary>
    /// ObtÃ©m um agendamento pelo identificador.
    /// </summary>
    [HttpGet("pessoa/{pessoaId:guid}/agendamento/{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<AgendamentoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<AgendamentoResponse>>> ObterPorIdAsync(Guid pessoaId, Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a obtenÃ§Ã£o do agendamento {Id} da pessoa {PessoaId}.", id, pessoaId);

        var result = await _getByIdHandler.HandleAsync(new GetAgendamentoByIdQuery(pessoaId, id));

        if (!result.IsSuccess || result.Value is null)
            return NotFound(ApiResponse.FailureResponse(result.Error ?? "Agendamento nÃ£o encontrado."));

        return Ok(ApiResponse<AgendamentoResponse>.SuccessResponse(result.Value));
    }

    /// <summary>
    /// Cria um novo agendamento para uma pessoa especÃ­fica.
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
        _logger.LogInformation("Iniciando a criaÃ§Ã£o de agendamento para a pessoa {PessoaId} com o consultor {ConsultorPessoaId} em {DataAgendamento} {HorarioAgendamento}.", pessoaId, request.ConsultorPessoaId, request.DataAgendamento, request.HorarioAgendamento);

        var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));

        var result = await _createHandler.HandleAsync(new CreateAgendamentoCommand
        {
            PessoaId = pessoaId,
            ConsultorPessoaId = request.ConsultorPessoaId,
            VeiculoId = request.VeiculoId,
            DataAgendamento = request.DataAgendamento,
            HorarioAgendamento = request.HorarioAgendamento,
            Descricao = request.Descricao
        });

        if (!result.IsSuccess || result.Value is null)
        {
            var error = result.Error ?? "NÃ£o foi possÃ­vel criar o agendamento.";

            _logger.LogWarning("Falha ao criar agendamento. PessoaId: {PessoaId}, ConsultorPessoaId: {ConsultorPessoaId}, DataAgendamento: {DataAgendamento}, HorarioAgendamento: {HorarioAgendamento}, Erro: {Erro}", pessoaId, request.ConsultorPessoaId, request.DataAgendamento, request.HorarioAgendamento, error);

            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Falha ao criar agendamento."));

        }

        return CreatedAtAction(
            nameof(ObterPorIdAsync),
            new { pessoaId, id = result.Value.Id },
            ApiResponse<AgendamentoResponse>.SuccessResponse(result.Value, "Agendamento criado com sucesso."));
    }
}
