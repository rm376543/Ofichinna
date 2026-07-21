using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ofichina.Application.UseCases.Servicos.Commands;
using Ofichina.Application.UseCases.Servicos.Queries;
using Ofichina.Contracts.Requests.Servicos;
using Ofichina.Contracts.Responses;
using Ofichina.Contracts.Responses.Servicos;

namespace Ofichina.Api.Controllers.Servicos;

/// <summary>
/// Controller responsável pelo CRUD de serviços.
/// </summary>
[Authorize]
[ApiController]
[Route("api/servicos")]
public sealed class ServicoController : ControllerBase
{
    private readonly IValidator<CreateServicoRequest> _createValidator;
    private readonly IValidator<UpdateServicoRequest> _updateValidator;
    private readonly IMediator _mediator;
    private readonly ILogger<ServicoController> _logger;

    public ServicoController(
        IValidator<CreateServicoRequest> createValidator,
        IValidator<UpdateServicoRequest> updateValidator,
        IMediator mediator,
        ILogger<ServicoController> logger)
    {
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Retorna todos os serviços cadastrados.
    /// </summary>
    [Authorize(Roles = "ADMIN")]
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<ServicoResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<ServicoResponse>>>> BuscarServicos(
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a obtenção de todos os serviços.");

        var result = await _mediator.Send(new GetServicosQuery(), cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível obter os serviços."));

        return Ok(ApiResponse<IReadOnlyCollection<ServicoResponse>>.SuccessResponse(result.Value ?? []));
    }

    /// <summary>
    /// Retorna um serviço pelo identificador.
    /// </summary>
    [Authorize(Roles = "ADMIN")]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ServicoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ServicoResponse>>> BuscarServicoPorId(
        Guid id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a obtenção do serviço com Id: {Id}", id);

        var result = await _mediator.Send(new GetServicoByIdQuery { Id = id }, cancellationToken);

        if (!result.IsSuccess || result.Value is null)
            return NotFound(ApiResponse.FailureResponse(result.Error ?? "Serviço não encontrado."));

        return Ok(ApiResponse<ServicoResponse>.SuccessResponse(result.Value));
    }

    /// <summary>
    /// Cria um novo serviço.
    /// </summary>
    [Authorize(Roles = "ADMIN")]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<Guid>>> CriarServico(
        [FromBody] CreateServicoRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a criação de um serviço. Nome: {Nome}", request.Nome);

        var validation = await _createValidator.ValidateAsync(request, cancellationToken);

        if (!validation.IsValid)
            return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));

        var result = await _mediator.Send(new CreateServicoCommand
        {
            Nome = request.Nome,
            Descricao = request.Descricao,
            Valor = request.Valor,
            Ativo = request.Ativo
        }, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível criar o serviço."));

        return StatusCode(StatusCodes.Status201Created, ApiResponse<Guid>.SuccessResponse(result.Value, "Serviço criado com sucesso."));
    }

    /// <summary>
    /// Atualiza um serviço existente.
    /// </summary>
    [Authorize(Roles = "ADMIN")]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> AtualizarServico(
        Guid id,
        [FromBody] UpdateServicoRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a atualização do serviço com Id: {Id}", id);

        request.Id = id;

        var validation = await _updateValidator.ValidateAsync(request, cancellationToken);

        if (!validation.IsValid)
            return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));

        var result = await _mediator.Send(new UpdateServicoCommand
        {
            Id = id,
            Nome = request.Nome,
            Descricao = request.Descricao,
            Valor = request.Valor,
            Ativo = request.Ativo
        }, cancellationToken);

        if (!result.IsSuccess)
            return result.Error == "Serviço não encontrado."
                ? NotFound(ApiResponse.FailureResponse(result.Error))
                : BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível atualizar o serviço."));

        return Ok(ApiResponse.SuccessResponse("Serviço atualizado com sucesso."));
    }

    /// <summary>
    /// Remove logicamente um serviço.
    /// </summary>
    [Authorize(Roles = "ADMIN")]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> RemoverServico(
        Guid id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a remoção do serviço com Id: {Id}", id);

        var result = await _mediator.Send(new DeleteServicoCommand { Id = id }, cancellationToken);

        if (!result.IsSuccess)
            return NotFound(ApiResponse.FailureResponse(result.Error ?? "Serviço não encontrado."));

        return Ok(ApiResponse.SuccessResponse("Serviço removido com sucesso."));
    }
}


