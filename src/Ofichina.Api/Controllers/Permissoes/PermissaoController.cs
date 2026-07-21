using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ofichina.Application.UseCases.Permissoes.Commands;
using Ofichina.Application.UseCases.Permissoes.Queries;
using Ofichina.Contracts.Requests.Permissoes;
using Ofichina.Contracts.Responses;
using Ofichina.Contracts.Responses.Permissoes;

namespace Ofichina.Api.Controllers.Permissoes;

[Authorize]
[ApiController]
[Route("api/permissao")]
public sealed class PermissaoController : ControllerBase
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

    [Authorize(Roles = "ADMIN")]
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<PermissaoResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<PermissaoResponse>>>> GetAllAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando processo de busca de permissões");
        var result = await _mediator.Send(new GetPermissoesQuery(), cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogError("Ocorreu um erro ao buscar as permissões: {Error}", result.Error);
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível obter as permissões."));
        }

        _logger.LogInformation("Processo de busca de permissões concluído com sucesso");
        return Ok(ApiResponse<IReadOnlyCollection<PermissaoResponse>>.SuccessResponse(result.Value ?? []));
    }

    [Authorize(Roles = "ADMIN")]
    [HttpGet("{id:guid}")]
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

    [Authorize(Roles = "ADMIN")]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<Guid>>> CreateAsync([FromBody] CreatePermissaoRequest request, CancellationToken cancellationToken)
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
            
            if (result.Error == "Já existe uma permissão com este código.")
            {
                _logger.LogError("Já existe uma permissão com o código: {Codigo}", request.Codigo);
                return BadRequest(ApiResponse.FailureResponse(result.Error));
            }

            _logger.LogError("Ocorreu um erro ao criar a permissão com código: {Codigo}. Erro: {Error}", request.Codigo, result.Error);
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível criar a permissão."));
        }

        _logger.LogInformation("Processo de criação de permissão com código: {Codigo} concluído com sucesso", request.Codigo);
        return StatusCode(StatusCodes.Status201Created, ApiResponse<Guid>.SuccessResponse(result.Value, "Permissão criada com sucesso."));
    }

    [Authorize(Roles = "ADMIN")]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> UpdateAsync(Guid id, [FromBody] UpdatePermissaoRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando processo de atualização de permissão com ID: {Id}", id);
        request.Id = id;

        var validation = await _updateValidator.ValidateAsync(request, cancellationToken);

        if (!validation.IsValid)
        {
            _logger.LogWarning("Validação falhou para a atualização de permissão com ID: {Id}. Erros: {Errors}", id, string.Join(", ", validation.Errors.Select(x => x.ErrorMessage)));
            return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));
        }

        var result = await _mediator.Send(new UpdatePermissaoCommand(id, request.Codigo, request.Descricao), cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.Error == "Permissão não encontrada.")
            {
                _logger.LogInformation("Permissão com ID: {Id} não encontrada", id);
                return NotFound(ApiResponse.FailureResponse(result.Error));
            }

            _logger.LogInformation("Processo de atualização de permissão com ID: {Id} concluído com erros", id);
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível atualizar a permissão."));
        }

        _logger.LogInformation("Processo de atualização de permissão com ID: {Id} concluído com sucesso", id);
        return Ok(ApiResponse.SuccessResponse("Permissão atualizada com sucesso."));
    }

    [Authorize(Roles = "ADMIN")]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando processo de remoção de permissão com ID: {Id}", id);
        var result = await _mediator.Send(new DeletePermissaoCommand(id), cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.Error == "Permissão não encontrada.")
            {
                _logger.LogInformation("Permissão com ID: {Id} não encontrada", id);
                return NotFound(ApiResponse.FailureResponse(result.Error));
            }

            _logger.LogInformation("Processo de remoção de permissão com ID: {Id} concluído com erros", id);
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível remover a permissão."));
        }

        _logger.LogInformation("Processo de remoção de permissão com ID: {Id} concluído com sucesso", id);
        return Ok(ApiResponse.SuccessResponse("Permissão removida com sucesso."));
    }
}
