using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ofichina.Application.UseCases.Servicos.Commands;
using Ofichina.Application.UseCases.Servicos.Queries;
using Ofichina.Contracts;
using Ofichina.Contracts.Common;
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
#pragma warning disable S6960
public sealed class ServicoController : ControllerBase
#pragma warning restore S6960
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
    /// <returns>Uma lista paginada de serviços.</returns>
    /// <param name="pagination">Parâmetros de paginação.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    [Authorize(Roles = "ADMIN")]
    [HttpGet("listar")]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<ServicoResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<PagedResponse<ServicoResponse>>>> BuscarTodosServicosPaginados(
        [FromQuery] Pagination pagination,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a obtenção de todos os serviços.");

        var result = await _mediator.Send(new GetAllServicosPaginadosQuery(pagination), cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível obter os serviços."));

        return Ok(ApiResponse<PagedResponse<ServicoResponse>>.SuccessResponse(result.Value));
    }

    /// <summary>
    /// Retorna um serviço pelo identificador.
    /// </summary>
    /// <returns>O serviço correspondente ao identificador fornecido.</returns>
    /// <param name="servicoId">Identificador do serviço.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    [Authorize(Roles = "ADMIN")]
    [HttpGet("detalhar/{servicoId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ServicoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ServicoResponse>>> BuscarServicoPorId(
        [FromRoute] Guid servicoId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a obtenção do serviço com Id: {Id}", servicoId);

        var result = await _mediator.Send(new GetServicoByIdQuery(servicoId), cancellationToken);

        if (!result.IsSuccess || result.Value is null)
            return NotFound(ApiResponse.FailureResponse(result.Error ?? "Serviço não encontrado."));

        return Ok(ApiResponse<ServicoResponse>.SuccessResponse(result.Value));
    }

    /// <summary>
    /// Cria um novo serviço.
    /// </summary>
    /// <returns>O identificador do serviço criado.</returns>
    /// <param name="request">Dados do serviço a ser criado.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    [Authorize(Roles = "ADMIN")]
    [HttpPost("novo")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse>> CriarServico(
        [FromBody] CreateServicoRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a criação de um serviço. Nome: {Nome}", request.Nome);

        var validation = await _createValidator.ValidateAsync(request, cancellationToken);

        if (!validation.IsValid)
            return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));

        var result = await _mediator.Send(new CreateServicoCommand(request), cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível criar o serviço."));

        return ApiResponse.SuccessResponse("Serviço criado com sucesso.");
    }

    /// <summary>
    /// Atualiza um serviço existente.
    /// </summary>
    /// <param name="request">Dados do serviço a ser atualizado.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Resultado da operação de atualização.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPut("atualizar")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> AtualizarServico(
        [FromBody] UpdateServicoRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a atualização do serviço com Id: {Id}", request.Id);

        var validation = await _updateValidator.ValidateAsync(request, cancellationToken);

        if (!validation.IsValid)
            return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));

        var result = await _mediator.Send(new UpdateServicoCommand(request), cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível atualizar o serviço."));

        return Ok(ApiResponse.SuccessResponse("Serviço atualizado com sucesso."));
    }

    /// <summary>
    /// Remove logicamente um serviço.
    /// </summary>
    /// <param name="request">Identificador do serviço.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Resultado da operação de remoção.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpDelete("remover")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> RemoverServico(
        [FromBody] RemoveServicoRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a remoção do serviço com Id: {Id}", request.ServicoId);

        var result = await _mediator.Send(new DeleteServicoCommand(request), cancellationToken);

        if (!result.IsSuccess)
            return NotFound(ApiResponse.FailureResponse(result.Error ?? "Serviço não encontrado."));

        return Ok(ApiResponse.SuccessResponse("Serviço removido com sucesso."));
    }
}


