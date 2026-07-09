using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Perfis.Commands;
using Ofichina.Application.UseCases.Perfis.Queries;
using Ofichina.Contracts.Requests.Perfil;
using Ofichina.Contracts.Responses;
using Ofichina.Contracts.Responses.Perfil;

namespace Ofichina.Api.Controllers.Perfis;

[Authorize]
[ApiController]
[Route("api/[controller]")]
#pragma warning disable S6960
public sealed class PerfisController : ControllerBase
#pragma warning restore S6960
{
    private readonly IValidator<CreatePerfilRequest> _createValidator;
    private readonly IValidator<UpdatePerfilRequest> _updateValidator;
    private readonly ICommandHandler<CreatePerfilCommand, Guid> _createHandler;
    private readonly ICommandHandler<UpdatePerfilCommand> _updateHandler;
    private readonly ICommandHandler<DeletePerfilCommand> _deleteHandler;
    private readonly IQueryHandler<GetPerfisQuery, IReadOnlyCollection<PerfilResponse>> _getAllHandler;
    private readonly IQueryHandler<GetPerfilByIdQuery, PerfilResponse?> _getByIdHandler;

    public PerfisController(
        IValidator<CreatePerfilRequest> createValidator,
        IValidator<UpdatePerfilRequest> updateValidator,
        ICommandHandler<CreatePerfilCommand, Guid> createHandler,
        ICommandHandler<UpdatePerfilCommand> updateHandler,
        ICommandHandler<DeletePerfilCommand> deleteHandler,
        IQueryHandler<GetPerfisQuery, IReadOnlyCollection<PerfilResponse>> getAllHandler,
        IQueryHandler<GetPerfilByIdQuery, PerfilResponse?> getByIdHandler)
    {
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _createHandler = createHandler;
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;
        _getAllHandler = getAllHandler;
        _getByIdHandler = getByIdHandler;
    }

    [AllowAnonymous]
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<PerfilResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<PerfilResponse>>>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        var result = await _getAllHandler.HandleAsync(new GetPerfisQuery());

        return Ok(ApiResponse<IReadOnlyCollection<PerfilResponse>>.SuccessResponse(result));
    }

    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PerfilResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<PerfilResponse>>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _getByIdHandler.HandleAsync(new GetPerfilByIdQuery(id));

        if (result is null)
        {
            return NotFound(ApiResponse.FailureResponse("Perfil não encontrado."));
        }

        return Ok(ApiResponse<PerfilResponse>.SuccessResponse(result));
    }

    [AllowAnonymous]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<Guid>>> CreateAsync(
        [FromBody] CreatePerfilRequest request,
        CancellationToken cancellationToken)
    {
        var validation = await _createValidator.ValidateAsync(request, cancellationToken);

        if (!validation.IsValid)
        {
            return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));
        }

        var command = new CreatePerfilCommand(
            request.Codigo,
            request.Nome,
            request.Descricao,
            request.Ativo);

        var id = await _createHandler.HandleAsync(command);

        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<Guid>.SuccessResponse(id, "Perfil criado com sucesso."));
    }

    [AllowAnonymous]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> UpdateAsync(
        Guid id,
        [FromBody] UpdatePerfilRequest request,
        CancellationToken cancellationToken)
    {
        if (id != request.Id)
        {
            return BadRequest(ApiResponse.FailureResponse("O Id da rota deve ser igual ao Id do corpo da requisição."));
        }

        var validation = await _updateValidator.ValidateAsync(request, cancellationToken);

        if (!validation.IsValid)
        {
            return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));
        }

        var result = await _updateHandler.HandleAsync(new UpdatePerfilCommand(
            request.Id,
            request.Codigo,
            request.Nome,
            request.Descricao,
            request.Ativo));

        if (!result.IsSuccess)
        {
            return result.Error == "Perfil não encontrado."
                ? NotFound(ApiResponse.FailureResponse(result.Error))
                : BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível atualizar o perfil."));
        }

        return Ok(ApiResponse.SuccessResponse("Perfil atualizado com sucesso."));
    }

    [AllowAnonymous]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _deleteHandler.HandleAsync(new DeletePerfilCommand(id));

        if (!result.IsSuccess)
        {
            return NotFound(ApiResponse.FailureResponse(result.Error ?? "Perfil não encontrado."));
        }

        return Ok(ApiResponse.SuccessResponse("Perfil desativado com sucesso."));
    }
}