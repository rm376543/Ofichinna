using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ofichina.Application.Abstractions.Common;
using Ofichina.Application.UseCases.Agendamentos.Commands;
using Ofichina.Application.UseCases.Agendamentos.Queries;
using Ofichina.Contracts;
using Ofichina.Contracts.Common;
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
    /// Busca os horários disponíveis para agendamento.
    /// </summary>
    /// <param name="pagination"></param>
    /// <returns></returns>
    [Authorize(Roles = "ADMIN")]
    [HttpGet("horarios-disponiveis")]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<HorarioDisponivelResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<PagedResponse<HorarioDisponivelResponse>>>> BuscarHorarios([FromQuery] Pagination pagination)
    {
        _logger.LogInformation("Iniciando a busca de horários disponíveis para agendamento.");

        var result = await _mediator.Send(new GetHorariosDisponiveisQuery(pagination));

        if (!result.IsSuccess || result.Value is null)
        {
            _logger.LogWarning("Falha ao buscar horários disponíveis. Erro: {Erro}", result.Error);
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível obter os horários disponíveis."));
        }

        return Ok(ApiResponse<PagedResponse<HorarioDisponivelResponse>>.SuccessResponse(result.Value));
    }

    /// <summary>
    /// Cadastra horários disponíveis para agendamento.
    /// </summary>
    /// <param name="horario"></param>
    /// <returns></returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPost("cadastrar-horario")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> CadastrarHorarioParaAgendamento([FromBody] TimeOnly horario)
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

        return Ok(ApiResponse<IReadOnlyCollection<AgendamentoResponse>>.SuccessResponse(result.Value));
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
        _logger.LogInformation("Iniciando criação de agendamento. PessoaId: {PessoaId}, SlotId: {SlotId}", pessoaId, request.HorarioConsultorDisponibilidadeId);

        var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));

        var result = await _mediator.Send(new CreateAgendamentoCommand
        {
            PessoaId = pessoaId,
            HorarioConsultorDisponibilidadeId = request.HorarioConsultorDisponibilidadeId,
            VeiculoId = request.VeiculoId,
            Descricao = request.Descricao
        }, cancellationToken);

        if (!result.IsSuccess || result.Value is null)
        {
            var error = result.Error ?? "Não foi possível criar o agendamento.";

            _logger.LogWarning("Falha ao criar agendamento. PessoaId: {PessoaId}, SlotId: {SlotId}, Erro: {Erro}", pessoaId, request.HorarioConsultorDisponibilidadeId, error);

            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Falha ao criar agendamento."));

        }

        return CreatedAtAction(
            nameof(ObterPorIdAsync),
            new { pessoaId, id = result.Value.Id },
            ApiResponse<AgendamentoResponse>.SuccessResponse(result.Value, "Agendamento criado com sucesso."));
    }

    /// <summary>
    /// Lista dias disponíveis para agendamento em um mês/ano específico.
    /// </summary>
    [HttpGet("dias-disponiveis")]
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

        var mapped = result.Select(d => new DiaDisponibilidadeResponse
        {
            Id = d.Id,
            Data = DateOnly.ParseExact(d.Data, "yyyy-MM-dd")
        });

        return Ok(ApiResponse<IEnumerable<DiaDisponibilidadeResponse>>.SuccessResponse(mapped));
    }

    /// <summary>
    /// Lista horários disponíveis de um dia específico.
    /// </summary>
    [HttpGet("dias/{diaId:guid}/horarios")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<HorarioDisponivelResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<IEnumerable<HorarioDisponivelResponse>>>> ListarHorariosPorDiaAsync(
        Guid diaId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Listando horários para dia. DiaId: {DiaId}", diaId);

        var result = await _mediator.Send(new ListarHorariosPorDiaQuery
        {
            DiaDisponibilidadeId = diaId
        }, cancellationToken);

        var mapped = result.Select(h => new HorarioDisponivelResponse
        {
            Id = h.Id,
            Horario = TimeOnly.ParseExact(h.Hora, "HH:mm"),
            Disponivel = h.Disponivel
        });

        return Ok(ApiResponse<IEnumerable<HorarioDisponivelResponse>>.SuccessResponse(mapped));
    }

    /// <summary>
    /// Lista consultores disponíveis para uma combinação dia + horário.
    /// </summary>
    [HttpGet("dias/{diaId:guid}/horarios/{horarioId:guid}/consultores")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ConsultorDisponibilidadeResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<IEnumerable<ConsultorDisponibilidadeResponse>>>> ListarConsultoresPorDiaHorarioAsync(
        Guid diaId,
        Guid horarioId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Listando consultores. DiaId: {DiaId}, HorarioId: {HorarioId}", diaId, horarioId);

        var result = await _mediator.Send(new ListarConsultoresPorDiaHorarioQuery
        {
            DiaDisponibilidadeId = diaId,
            HorarioDisponibilidadeId = horarioId
        }, cancellationToken);

        var mapped = result.Select(c => new ConsultorDisponibilidadeResponse
        {
            Id = c.PessoaId,
            Nome = c.Nome,
            Documento = c.Documento
        });

        return Ok(ApiResponse<IEnumerable<ConsultorDisponibilidadeResponse>>.SuccessResponse(mapped));
    }

    /// <summary>
    /// Lista agenda de um consultor em uma data específica.
    /// </summary>
    [HttpGet("consultores/{consultorId:guid}/agenda")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<AgendaConsultorResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<IEnumerable<AgendaConsultorResponse>>>> ListarAgendaPorConsultorAsync(
        Guid consultorId,
        [FromQuery] DateOnly data,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Listando agenda do consultor. ConsultorId: {ConsultorId}, Data: {Data}", consultorId, data);

        var result = await _mediator.Send(new ListarAgendaPorConsultorQuery
        {
            ConsultorPessoaId = consultorId,
            Data = data
        }, cancellationToken);

        var mapped = result.Select(a => new AgendaConsultorResponse
        {
            SlotId = a.SlotId,
            Hora = a.Hora,
            Status = a.Status,
            ClienteNome = a.ClienteNome,
            Veiculo = a.Veiculo
        });

        return Ok(ApiResponse<IEnumerable<AgendaConsultorResponse>>.SuccessResponse(mapped));
    }


    /// <summary>
    /// Cancela um agendamento existente.
    /// </summary>
    [HttpPost("{id:guid}/cancelar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> CancelarAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Cancelando agendamento. AgendamentoId: {AgendamentoId}", id);

        // TODO: Obter PessoaId do contexto (claims do JWT)
        var pessoaId = Guid.Empty; // Temporariamente vazio, deve vir do contexto

        var result = await _mediator.Send(new CancelarAgendamentoCommand(pessoaId, id), cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível cancelar o agendamento."));

        return Ok(ApiResponse.SuccessResponse("Agendamento cancelado com sucesso."));
    }
}


