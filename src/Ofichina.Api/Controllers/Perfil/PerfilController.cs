using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Perfis.Commands;
using Ofichina.Application.UseCases.Perfis.Queries;
using Ofichina.Contracts.Enums;
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

    /// <summary>
    /// Retorna todos os perfis cadastrados.
    /// </summary>
    /// <returns>Lista de perfis.</returns>
    [Authorize(Policy = UserPolicyEnum.Ler)]
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<PerfilResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<PerfilResponse>>>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        var result = await _getAllHandler.HandleAsync(new GetPerfisQuery());

        return Ok(ApiResponse<IReadOnlyCollection<PerfilResponse>>.SuccessResponse(result));
    }

    /// <summary>
    /// Retorna um perfil pelo identificador.
    /// </summary>
    /// <param name="id">Identificador do perfil.</param>
    /// <returns>Perfil encontrado ou erro 404 quando não existir.</returns>
    [Authorize(Policy = UserPolicyEnum.Ler)]
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

    /// <summary>
    /// Cria um novo perfil.
    /// </summary>
    /// <param name="request">Dados do perfil a ser criado.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Id do perfil criado ou erro de validação.</returns>
    [Authorize(Policy = UserPolicyEnum.Escrever)]
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

    /// <summary>
    /// Atualiza um perfil existente.
    /// </summary>
    /// <param name="request">Dados atualizados do perfil.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Mensagem de sucesso, erro de validação ou perfil não encontrado.</returns>
    [Authorize(Policy = UserPolicyEnum.Atualizar)]
    [HttpPut]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> UpdateAsync(
        [FromBody] UpdatePerfilRequest request,
        CancellationToken cancellationToken)
    {
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

    /// <summary>
    /// Desativa um perfil existente.
    /// </summary>
    /// <param name="id">Identificador do perfil.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Mensagem de sucesso ou erro 404.</returns>
    [Authorize(Policy = UserPolicyEnum.Deletar)]
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