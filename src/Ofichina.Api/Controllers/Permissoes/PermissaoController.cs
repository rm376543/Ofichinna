using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ofichina.Application.UseCases.Permissoes.Commands;
using Ofichina.Application.UseCases.Permissoes.Queries;
using Ofichina.Contracts;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.Permissoes;
using Ofichina.Contracts.Responses;
using Ofichina.Contracts.Responses.Permissoes;

namespace Ofichina.Api.Controllers.Permissoes;

[Authorize]
[ApiController]
[Route("api/permissao")]
#pragma warning disable S6960
public sealed class PermissaoController : ControllerBase
#pragma warning restore S6960
{
    private readonly IValidator<CreatePermissaoRequest> _createValidator;
    private readonly IValidator<UpdatePermissaoRequest> _updateValidator;
    private readonly IMediator _mediator;
    private readonly ILogger<PermissaoController> _logger;

    public PermissaoController(
        IValidator<CreatePermissaoRequest> createValidator,
        IValidator<UpdatePermissaoRequest> updateValidator,
        IMediator mediator,
        ILogger<PermissaoController> logger)
    {
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Retorna todas as permissões cadastradas.
    /// </summary>
    /// <param name="pagination">Parâmetros de paginação.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Lista de permissões.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpGet("listar")]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<PermissaoResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<PagedResponse<PermissaoResponse>>>> BuscarTodasPermissoesPaginadas(
        [FromQuery] Pagination pagination,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando processo de busca de permissões");
        var result = await _mediator.Send(new GetAllPermissoesPaginadasQuery(pagination), cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogError("Ocorreu um erro ao buscar as permissões: {Error}", result.Error);
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível obter as permissões."));
        }

        _logger.LogInformation("Processo de busca de permissões concluído com sucesso");
        return Ok(ApiResponse<PagedResponse<PermissaoResponse>>.SuccessResponse(result.Value));
    }

    /// <summary>
    /// Retorna uma permissão pelo identificador.
    /// </summary>
    /// <param name="id">Identificador da permissão.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Permissão encontrada ou erro 404 quando não existir.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpGet("detalhar/{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PermissaoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<PermissaoResponse>>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando processo de busca de permissão por ID: {Id}", id);
        var result = await _mediator.Send(new GetPermissaoByIdQuery(id), cancellationToken);

        if (!result.IsSuccess || result.Value is null)
        {
            _logger.LogError("Ocorreu um erro ao buscar a permissão por ID: {Id}. Erro: {Error}", id, result.Error);
            return NotFound(ApiResponse.FailureResponse(result.Error ?? "Permissão não encontrada."));
        }

        _logger.LogInformation("Processo de busca de permissão por ID: {Id} concluído com sucesso", id);
        return Ok(ApiResponse<PermissaoResponse>.SuccessResponse(result.Value));
    }

    /// <summary>
    /// Cria uma nova permissão.
    /// </summary>
    /// <param name="request">Dados da permissão a ser criada.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Id da permissão criada ou erro de validação.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPost("nova")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse>> CreateAsync([FromBody] CreatePermissaoRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando processo de criação de permissão com código: {Codigo}", request.Codigo);
        var validation = await _createValidator.ValidateAsync(request, cancellationToken);

        if (!validation.IsValid)
        {
            _logger.LogWarning("Validação falhou para a criação de permissão com código: {Codigo}. Erros: {Errors}", request.Codigo, string.Join(", ", validation.Errors.Select(x => x.ErrorMessage)));
            return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));
        }

        var result = await _mediator.Send(new CreatePermissaoCommand(request.Codigo, request.Descricao), cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogInformation("Processo de criação de permissão com código: {Codigo} concluído com erros", request.Codigo);
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível criar a permissão."));
        }

        _logger.LogInformation("Processo de criação de permissão com código: {Codigo} concluído com sucesso", request.Codigo);
        return Ok(ApiResponse.SuccessResponse("Permissão criada com sucesso."));
    }

    /// <summary>
    /// Retorna uma permissão pelo identificador.
    /// </summary>
    /// <param name="request">Dados da permissão a ser atualizada.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Permissão encontrada ou erro 404 quando não existir.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPut("atualizar")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> UpdateAsync([FromBody] UpdatePermissaoRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando processo de atualização de permissão com ID: {Id}", request.Id);

        var validation = await _updateValidator.ValidateAsync(request, cancellationToken);

        if (!validation.IsValid)
        {
            _logger.LogWarning("Validação falhou para a atualização de permissão com ID: {Id}. Erros: {Errors}", request.Id, string.Join(", ", validation.Errors.Select(x => x.ErrorMessage)));
            return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));
        }

        var result = await _mediator.Send(new UpdatePermissaoCommand(request.Id, request.Codigo, request.Descricao), cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogInformation("Processo de atualização de permissão com ID: {Id} concluído com erros", request.Id);
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível atualizar a permissão."));
        }

        _logger.LogInformation("Processo de atualização de permissão com ID: {Id} concluído com sucesso", request.Id);
        return Ok(ApiResponse.SuccessResponse("Permissão atualizada com sucesso."));
    }

    /// <summary>
    /// Remove uma permissão pelo identificador.
    /// </summary>
    /// <param name="request">Identificador da permissão.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Permissão encontrada ou erro 404 quando não existir.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpDelete("remover")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> DeleteAsync([FromBody] RemovePermissaoRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando processo de remoção de permissão com ID: {Id}", request.Id);
        var result = await _mediator.Send(new DeletePermissaoCommand(request.Id), cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogInformation("Processo de remoção de permissão com ID: {Id} concluído com erros", request.Id);
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível remover a permissão."));
        }

        _logger.LogInformation("Processo de remoção de permissão com ID: {Id} concluído com sucesso", request.Id);
        return Ok(ApiResponse.SuccessResponse("Permissão removida com sucesso."));
    }
}
