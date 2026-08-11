using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ofichina.Application.UseCases.Agendamentos.Commands;
using Ofichina.Application.UseCases.Agendamentos.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.Agendamento;
using Ofichina.Contracts.Responses.Agendamento;
using Ofichina.Contracts.Responses.Agendamento.Consultor;

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
    /// Busca horários disponíveis para agendamento.
    /// </summary>
    /// <param name="pagination">Parâmetros de paginação.</param>
    /// <returns>Lista de horários disponíveis para agendamento.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpGet("horarios-disponiveis")]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<HorarioResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<PagedResponse<HorarioResponse>>>> BuscarHorarios([FromQuery] Pagination pagination)
    {
        _logger.LogInformation("Iniciando a busca de horários disponíveis para agendamento.");

        var result = await _mediator.Send(new GetHorariosDisponiveisQuery(pagination));

        if (!result.IsSuccess || result.Value is null)
        {
            _logger.LogWarning("Falha ao buscar horários disponíveis. Erro: {Erro}", result.Error);
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível obter os horários disponíveis."));
        }

        return Ok(ApiResponse<PagedResponse<HorarioResponse>>.SuccessResponse(result.Value));
    }

    /// <summary>
    /// Lista todos os agendamentos de uma pessoa específica.
    /// </summary>
    /// <param name="pessoaId">Identificador da pessoa.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Lista de agendamentos da pessoa específica.</returns>
    [HttpGet("pessoa/{pessoaId:guid}/listar")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<AgendamentoUsuarioResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<AgendamentoUsuarioResponse>>>> ListarAsync(
        [FromRoute] Guid pessoaId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a listagem de agendamentos da pessoa {PessoaId}.", pessoaId);

        var result = await _mediator.Send(new GetAgendamentosQuery(pessoaId), cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível obter os agendamentos."));

        return Ok(ApiResponse<IReadOnlyCollection<AgendamentoUsuarioResponse>>.SuccessResponse(result.Value));
    }

    /// <summary>
    /// Obtém detalhes de um agendamento específico de uma pessoa.
    /// </summary>
    /// <param name="pessoaId">Identificador da pessoa.</param>
    /// <param name="agendamentoId">Identificador do agendamento.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Detalhes do agendamento específico da pessoa.</returns>
    [HttpGet("pessoa/{pessoaId:guid}/agendamento/{agendamentoId:guid}/detalhar")]
    [ProducesResponseType(typeof(ApiResponse<AgendamentoUsuarioDetalheResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<AgendamentoUsuarioDetalheResponse>>> ObterPorIdAsync(
        [FromRoute] Guid pessoaId, [FromRoute] Guid agendamentoId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a obtenção do agendamento {Id} da pessoa {PessoaId}.", agendamentoId, pessoaId);

        var result = await _mediator.Send(new GetAgendamentoByIdQuery(pessoaId, agendamentoId), cancellationToken);

        if (!result.IsSuccess || result.Value is null)
            return NotFound(ApiResponse.FailureResponse(result.Error ?? "Agendamento não encontrado."));

        return Ok(ApiResponse<AgendamentoUsuarioDetalheResponse>.SuccessResponse(result.Value));
    }

    /// <summary>
    /// Lista os dias disponíveis para agendamento em um mês e ano específicos.
    /// </summary>
    /// <param name="mes">Mês para o qual listar os dias disponíveis.</param>
    /// <param name="ano">Ano para o qual listar os dias disponíveis.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Lista de dias disponíveis.</returns>
    [HttpGet("listar/dias-disponiveis")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<DiaDisponibilidadeResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<IEnumerable<DiaDisponibilidadeResponse>>>> ListarDiasDisponiveisAsync(
    [FromQuery] int mes,
    [FromQuery] int ano,
    CancellationToken cancellationToken)
    {
        _logger.LogInformation("Listando dias disponíveis. Mês: {Mes}, Ano: {Ano}", mes, ano);

        var result = await _mediator.Send(new ListarDiasDisponiveisQuery
        {
            Mes = mes,
            Ano = ano
        }, cancellationToken);

        if (!result.IsSuccess || result.Value is null)
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível obter os dias disponíveis."));

        return Ok(ApiResponse<IEnumerable<DiaDisponibilidadeResponse>>.SuccessResponse(result.Value));
    }

    /// <summary>
    /// Lista horários disponíveis para agendamento em um dia específico.
    /// </summary>
    /// <param name="diaId">Identificador do dia para o qual listar os horários disponíveis.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Lista de horários disponíveis para o dia especificado.</returns>
    [HttpGet("listar/horarios-por-dia")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<HorarioResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<IEnumerable<HorarioResponse>>>> ListarHorariosPorDiaAsync(
    [FromQuery] Guid diaId,
    CancellationToken cancellationToken)
    {
        _logger.LogInformation("Listando horários para dia. DiaId: {DiaId}", diaId);

        var result = await _mediator.Send(new ListarHorariosPorDiaQuery
        {
            DiaDisponibilidadeId = diaId
        }, cancellationToken);

        if (!result.IsSuccess || result.Value is null)
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível obter os horários disponíveis."));

        return Ok(ApiResponse<IEnumerable<HorarioResponse>>.SuccessResponse(result.Value));
    }

    /// <summary>
    /// Lista consultores disponíveis em um dia e horário específicos.
    /// </summary>
    /// <param name="diaId">Identificador do dia para o qual listar os consultores disponíveis.</param>
    /// <param name="horarioId">Identificador do horário para o qual listar os consultores disponíveis.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Lista de consultores disponíveis.</returns>
    [HttpGet("listar/consultores-disponiveis")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ConsultorDisponibilidadeResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<IEnumerable<ConsultorDisponibilidadeResponse>>>> ListarConsultoresPorDiaHorarioAsync(
    [FromQuery] Guid diaId,
    [FromQuery] Guid horarioId,
    CancellationToken cancellationToken)
    {
        _logger.LogInformation("Listando consultores. DiaId: {DiaId}, HorarioId: {HorarioId}", diaId, horarioId);

        var result = await _mediator.Send(new ListarConsultoresPorDiaHorarioQuery
        {
            DiaDisponibilidadeId = diaId,
            HorarioDisponibilidadeId = horarioId
        }, cancellationToken);

        if (!result.IsSuccess || result.Value is null)
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível obter os consultores disponíveis."));

        return Ok(ApiResponse<IEnumerable<ConsultorDisponibilidadeResponse>>.SuccessResponse(result.Value));
    }

    /// <summary>
    /// Lista a agenda de um consultor em uma data específica.
    /// </summary>
    /// <param name="consultorId">Identificador do consultor</param>
    /// <param name="data">Data para a qual listar a agenda do consultor. Ex: YYYY-MM-DD</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Lista de compromissos do consultor na data especificada.</returns>
    [HttpGet("consultor/listar-agenda")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<AgendaConsultorResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<IEnumerable<AgendaConsultorResponse>>>> ListarAgendaPorConsultorAsync(
        [FromQuery] Guid consultorId,
        [FromQuery] DateOnly data,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Listando agenda do consultor. ConsultorId: {ConsultorId}, Data: {Data}", consultorId, data);

        var result = await _mediator.Send(new ListarAgendaPorConsultorQuery
        {
            ConsultorPessoaId = consultorId,
            Data = data
        }, cancellationToken);

        if (!result.IsSuccess || result.Value is null)
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível obter a agenda do consultor."));

        return Ok(ApiResponse<IEnumerable<AgendaConsultorResponse>>.SuccessResponse(result.Value));
    }

    /// <summary>
    /// Cadastra horários disponíveis para agendamento.
    /// </summary>
    /// <param name="horario">Horário a ser cadastrado.</param>
    /// <returns>Resultado da operação de cadastro de horário.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPost("cadastrar-horario")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> CadastrarHorarioParaAgendamento(
        [FromBody] TimeOnly horario)
    {
        _logger.LogInformation("Iniciando o cadastro de horários disponíveis para agendamento.");

        var result = await _mediator.Send(new CadastraHorarioAgendamentoCommand(horario));

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Falha ao cadastrar horários disponíveis para agendamento. Erro: {Erro}", result.Error);
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível cadastrar os horários disponíveis."));
        }

        _logger.LogInformation("Horário cadastrado com sucesso.");

        return Ok(ApiResponse.SuccessResponse("Horário cadastrado com sucesso."));
    }

    /// <summary>
    /// Cria um novo agendamento para uma pessoa.
    /// </summary>
    /// <param name="request">Objeto contendo os dados necessários para criar um novo agendamento.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Resultado da operação de criação de agendamento.</returns>
    [HttpPost("pessoa/adicionar")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<AgendamentoResponse>>> CriarAsync(
        [FromBody] CreateAgendamentoRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando criação de agendamento. PessoaId: {PessoaId}, AgendaId: {AgendaId}", request.PessoaId, request.AgendaConsultorId);

        var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));

        var result = await _mediator.Send(new CreateAgendamentoCommand(
            request.PessoaId,
            request.AgendaConsultorId,
            request.VeiculoId,
            request.Hodometro,
            request.Descricao
        ), cancellationToken);

        if (!result.IsSuccess)
        {
            var error = result.Error ?? "Não foi possível criar o agendamento.";

            _logger.LogWarning("Falha ao criar agendamento. PessoaId: {PessoaId}, AgendaId: {AgendaId}, Erro: {Erro}", request.PessoaId, request.AgendaConsultorId, error);

            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Falha ao criar agendamento."));

        }

        return Ok(ApiResponse.SuccessResponse("Agendamento criado com sucesso."));
    }

    /// <summary>
    /// Cancela um agendamento existente.
    /// </summary>
    /// <param name="request">Objeto contendo o identificador do agendamento a ser cancelado.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Resultado da operação de cancelamento.</returns>
    [HttpPost("consultor/cancelar-agendamento")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> CancelarAgendamentoAsync(
        [FromBody] CancelarAgendamentoRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Cancelando agendamento. AgendamentoId: {AgendamentoId}", request.AgendamentoId);

        var result = await _mediator.Send(new CancelarAgendamentoCommand(request), cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível cancelar o agendamento."));

        return Ok(ApiResponse.SuccessResponse("Agendamento cancelado com sucesso."));
    }

    /// <summary>
    /// Inicia um agendamento existente.
    /// </summary>
    /// <param name="agendamentoId">Identificador do agendamento a ser iniciado.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Resultado da operação de início do agendamento.</returns>
    [HttpPut("consultor/{agendamentoId:guid}/iniciar")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> IniciarAsync(
        [FromRoute] Guid agendamentoId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando agendamento. AgendamentoId: {AgendamentoId}", agendamentoId);

        var result = await _mediator.Send(new IniciarAgendamentoCommand(agendamentoId), cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível iniciar o agendamento."));

        return Ok(ApiResponse.SuccessResponse("Agendamento iniciado com sucesso."));
    }

}


