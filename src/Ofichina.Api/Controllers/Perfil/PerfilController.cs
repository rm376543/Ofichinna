using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Perfis.Commands;
using Ofichina.Application.UseCases.Perfis.Queries;
using Ofichina.Contracts.Requests.Perfil;
using Ofichina.Contracts.Responses;
using Ofichina.Contracts.Responses.Perfil;
using Ofichina.Contracts.Common;

namespace Ofichina.Api.Controllers.Perfis;

[Authorize]
[ApiController]
[Route("api/perfil")]
#pragma warning disable S6960
public sealed class PerfisController : ControllerBase
#pragma warning restore S6960
{
    private readonly IValidator<CreatePerfilRequest> _createValidator;
    private readonly IValidator<UpdatePerfilRequest> _updateValidator;
    private readonly ICommandHandler<CreatePerfilCommand, Result> _createHandler;
    private readonly ICommandHandler<UpdatePerfilCommand, Result> _updateHandler;
    private readonly ICommandHandler<DeletePerfilCommand, Result> _deleteHandler;
    private readonly IQueryHandler<GetPerfisQuery, Result<IReadOnlyCollection<PerfilResponse>>> _getAllHandler;
    private readonly IQueryHandler<GetPerfilByIdQuery, Result<PerfilResponse>> _getByIdHandler;
    private readonly ILogger<PerfisController> _logger;

#pragma warning disable S107
    public PerfisController(
        IValidator<CreatePerfilRequest> createValidator,
        IValidator<UpdatePerfilRequest> updateValidator,
        ICommandHandler<CreatePerfilCommand, Result> createHandler,
        ICommandHandler<UpdatePerfilCommand, Result> updateHandler,
        ICommandHandler<DeletePerfilCommand, Result> deleteHandler,
        IQueryHandler<GetPerfisQuery, Result<IReadOnlyCollection<PerfilResponse>>> getAllHandler,
        IQueryHandler<GetPerfilByIdQuery, Result<PerfilResponse>> getByIdHandler,
        ILogger<PerfisController> logger)
#pragma warning restore S107
    {
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _createHandler = createHandler;
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;
        _getAllHandler = getAllHandler;
        _getByIdHandler = getByIdHandler;
        _logger = logger;
    }

    /// <summary>
    /// Retorna todos os perfis cadastrados.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>Lista de perfis.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<PerfilResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<PerfilResponse>>>> GetAllAsync(
    CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a obtenção de todos os perfis.");
        var result = await _getAllHandler.HandleAsync(new GetPerfisQuery());

        if (!result.IsSuccess)
        {
            _logger.LogError("Erro ao obter os perfis: {Error}", result.Error);
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível obter os perfis."));
        }

        _logger.LogInformation("Perfis obtidos com sucesso. Total de perfis: {Count}", result.Value?.Count ?? 0);
        return Ok(ApiResponse<IReadOnlyCollection<PerfilResponse>>.SuccessResponse(result.Value ?? []));
    }

    /// <summary>
    /// Retorna um perfil pelo identificador.
    /// </summary>
    /// <param name="id">Identificador do perfil.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Perfil encontrado ou erro 404 quando não existir.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PerfilResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<PerfilResponse>>> GetByIdAsync(
    Guid id,
    CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a obtenção do perfil com Id: {Id}", id);
        var result = await _getByIdHandler.HandleAsync(new GetPerfilByIdQuery(id));

        if (!result.IsSuccess || result.Value is null)
        {
            _logger.LogError("Perfil com Id: {Id} não encontrado.", id);
            return NotFound(ApiResponse.FailureResponse(result.Error ?? "Perfil não encontrado."));
        }

        _logger.LogInformation("Perfil com Id: {Id} obtido com sucesso.", id);
        return Ok(ApiResponse<PerfilResponse>.SuccessResponse(result.Value));
    }

    /// <summary>
    /// Cria um novo perfil.
    /// </summary>
    /// <param name="request">Dados do perfil a ser criado.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Id do perfil criado ou erro de validação.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse>> CreateAsync(
        [FromBody] CreatePerfilRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a criação de um novo perfil com Nome: {NomePerfil}", request.NomePerfil);
        var validation = await _createValidator.ValidateAsync(request, cancellationToken);

        if (!validation.IsValid)
        {
            _logger.LogError("Erro ao validar a criação do perfil com Nome: {NomePerfil}. Erros: {Erros}", request.NomePerfil, string.Join(", ", validation.Errors.Select(x => x.ErrorMessage)));
            return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));
        }

        var command = new CreatePerfilCommand(
            request.NomePerfil,
            request.Descricao);

        await _createHandler.HandleAsync(command);

        _logger.LogInformation("Perfil criado com sucesso, Nome: {NomePerfil}", request.NomePerfil);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse.SuccessResponse($"Perfil criado com sucesso, Nome: {request.NomePerfil}"));
    }

    /// <summary>
    /// Atualiza um perfil existente.
    /// </summary>
    /// <param name="request">Dados atualizados do perfil.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Mensagem de sucesso, erro de validação ou perfil não encontrado.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPut]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse>> UpdateAsync(
        [FromBody] UpdatePerfilRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a atualização do perfil com Id: {Id}", request.Id);
        var validation = await _updateValidator.ValidateAsync(request, cancellationToken);

        if (!validation.IsValid)
        {
            _logger.LogError("Erro ao validar a atualização do perfil com Id: {Id}. Erros: {Erros}", request.Id, string.Join(", ", validation.Errors.Select(x => x.ErrorMessage)));
            return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));
        }

        var result = await _updateHandler.HandleAsync(new UpdatePerfilCommand(
            request.Id,
            request.NomePerfil,
            request.Descricao));

        if (!result.IsSuccess)
        {
            _logger.LogError("Erro ao atualizar o perfil com Id: {Id}. Erro: {Erro}", request.Id, result.Error);
            return result.Error == "Perfil não encontrado."
                ? NotFound(ApiResponse.FailureResponse(result.Error))
                : BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível atualizar o perfil."));
        }

        _logger.LogInformation("Perfil com Id: {Id} atualizado com sucesso.", request.Id);
        return Ok(ApiResponse.SuccessResponse("Perfil atualizado com sucesso."));
    }

    /// <summary>
    /// Desativa um perfil existente.
    /// </summary>
    /// <param name="id">Identificador do perfil.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Mensagem de sucesso ou erro 404.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a desativação do perfil com Id: {Id}", id);
        var result = await _deleteHandler.HandleAsync(new DeletePerfilCommand(id));

        if (!result.IsSuccess)
        {
            _logger.LogError("Erro ao desativar o perfil com Id: {Id}. Erro: {Erro}", id, result.Error);
            return NotFound(ApiResponse.FailureResponse(result.Error ?? "Perfil não encontrado."));
        }

        _logger.LogInformation("Perfil com Id: {Id} desativado com sucesso.", id);
        return Ok(ApiResponse.SuccessResponse("Perfil desativado com sucesso."));
    }
}