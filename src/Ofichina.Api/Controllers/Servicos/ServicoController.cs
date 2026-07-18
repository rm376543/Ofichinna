using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Servicos.Commands;
using Ofichina.Application.UseCases.Servicos.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.Servicos;
using Ofichina.Contracts.Responses;
using Ofichina.Contracts.Responses.Servicos;

namespace Ofichina.Api.Controllers.Servicos;

/// <summary>
/// Controller responsÃ¡vel pelo CRUD de serviÃ§os.
/// </summary>
[Authorize]
[ApiController]
[Route("api/servicos")]
public sealed class ServicoController : ControllerBase
{
    private readonly IValidator<CreateServicoRequest> _createValidator;
    private readonly IValidator<UpdateServicoRequest> _updateValidator;
    private readonly ILogger<ServicoController> _logger;

    public ServicoController(
        IValidator<CreateServicoRequest> createValidator,
        IValidator<UpdateServicoRequest> updateValidator,
        ILogger<ServicoController> logger)
    {
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _logger = logger;
    }

    /// <summary>
    /// Retorna todos os serviÃ§os cadastrados.
    /// </summary>
    [Authorize(Roles = "ADMIN")]
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<ServicoResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<ServicoResponse>>>> BuscarServicos(
        [FromServices] IQueryHandler<GetServicosQuery, Result<IReadOnlyCollection<ServicoResponse>>> handler,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a obtenÃ§Ã£o de todos os serviÃ§os.");

        var result = await handler.HandleAsync(new GetServicosQuery());

        if (!result.IsSuccess)
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "NÃ£o foi possÃ­vel obter os serviÃ§os."));

        return Ok(ApiResponse<IReadOnlyCollection<ServicoResponse>>.SuccessResponse(result.Value ?? []));
    }

    /// <summary>
    /// Retorna um serviÃ§o pelo identificador.
    /// </summary>
    [Authorize(Roles = "ADMIN")]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ServicoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ServicoResponse>>> BuscarServicoPorId(
        Guid id,
        [FromServices] IQueryHandler<GetServicoByIdQuery, Result<ServicoResponse>> handler,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a obtenÃ§Ã£o do serviÃ§o com Id: {Id}", id);

        var result = await handler.HandleAsync(new GetServicoByIdQuery { Id = id });

        if (!result.IsSuccess || result.Value is null)
            return NotFound(ApiResponse.FailureResponse(result.Error ?? "ServiÃ§o nÃ£o encontrado."));

        return Ok(ApiResponse<ServicoResponse>.SuccessResponse(result.Value));
    }

    /// <summary>
    /// Cria um novo serviÃ§o.
    /// </summary>
    [Authorize(Roles = "ADMIN")]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<Guid>>> CriarServico(
        [FromBody] CreateServicoRequest request,
        [FromServices] ICommandHandler<CreateServicoCommand, Result<Guid>> handler,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a criaÃ§Ã£o de um serviÃ§o. Nome: {Nome}", request.Nome);

        var validation = await _createValidator.ValidateAsync(request, cancellationToken);

        if (!validation.IsValid)
            return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));

        var result = await handler.HandleAsync(new CreateServicoCommand
        {
            Nome = request.Nome,
            Descricao = request.Descricao,
            Valor = request.Valor,
            Ativo = request.Ativo
        });

        if (!result.IsSuccess)
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "NÃ£o foi possÃ­vel criar o serviÃ§o."));

        return StatusCode(StatusCodes.Status201Created, ApiResponse<Guid>.SuccessResponse(result.Value, "ServiÃ§o criado com sucesso."));
    }

    /// <summary>
    /// Atualiza um serviÃ§o existente.
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
        [FromServices] ICommandHandler<UpdateServicoCommand, Result> handler,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a atualizaÃ§Ã£o do serviÃ§o com Id: {Id}", id);

        request.Id = id;

        var validation = await _updateValidator.ValidateAsync(request, cancellationToken);

        if (!validation.IsValid)
            return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));

        var result = await handler.HandleAsync(new UpdateServicoCommand
        {
            Id = id,
            Nome = request.Nome,
            Descricao = request.Descricao,
            Valor = request.Valor,
            Ativo = request.Ativo
        });

        if (!result.IsSuccess)
            return result.Error == "ServiÃ§o nÃ£o encontrado."
                ? NotFound(ApiResponse.FailureResponse(result.Error))
                : BadRequest(ApiResponse.FailureResponse(result.Error ?? "NÃ£o foi possÃ­vel atualizar o serviÃ§o."));

        return Ok(ApiResponse.SuccessResponse("ServiÃ§o atualizado com sucesso."));
    }

    /// <summary>
    /// Remove logicamente um serviÃ§o.
    /// </summary>
    [Authorize(Roles = "ADMIN")]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> RemoverServico(
        Guid id,
        [FromServices] ICommandHandler<DeleteServicoCommand, Result> handler,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a remoÃ§Ã£o do serviÃ§o com Id: {Id}", id);

        var result = await handler.HandleAsync(new DeleteServicoCommand { Id = id });

        if (!result.IsSuccess)
            return NotFound(ApiResponse.FailureResponse(result.Error ?? "ServiÃ§o nÃ£o encontrado."));

        return Ok(ApiResponse.SuccessResponse("ServiÃ§o removido com sucesso."));
    }
}
