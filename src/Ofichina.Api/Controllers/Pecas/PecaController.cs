using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Pecas.Commands;
using Ofichina.Application.UseCases.Pecas.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.Pecas;
using Ofichina.Contracts.Responses;
using Ofichina.Contracts.Responses.Pecas;

namespace Ofichina.Api.Controllers.Pecas;

/// <summary>
/// Controller responsÃ¡vel pelo CRUD de peÃ§as.
/// </summary>
[Authorize]
[ApiController]
[Route("api/pecas")]
public sealed class PecaController : ControllerBase
{
    private readonly IValidator<CreatePecaRequest> _createValidator;
    private readonly IValidator<UpdatePecaRequest> _updateValidator;
    private readonly ICommandHandler<CreatePecaCommand, Result<Guid>> _createHandler;
    private readonly ICommandHandler<UpdatePecaCommand, Result> _updateHandler;
    private readonly ICommandHandler<DeletePecaCommand, Result> _deleteHandler;
    private readonly IQueryHandler<GetPecasQuery, Result<IReadOnlyCollection<PecaResponse>>> _getAllHandler;
    private readonly IQueryHandler<GetPecaByIdQuery, Result<PecaResponse>> _getByIdHandler;
    private readonly ILogger<PecaController> _logger;

#pragma warning disable S107
    /// <summary>
    /// Inicializa uma nova instÃ¢ncia do controller de peÃ§as.
    /// </summary>
    public PecaController(
        IValidator<CreatePecaRequest> createValidator,
        IValidator<UpdatePecaRequest> updateValidator,
        ICommandHandler<CreatePecaCommand, Result<Guid>> createHandler,
        ICommandHandler<UpdatePecaCommand, Result> updateHandler,
        ICommandHandler<DeletePecaCommand, Result> deleteHandler,
        IQueryHandler<GetPecasQuery, Result<IReadOnlyCollection<PecaResponse>>> getAllHandler,
        IQueryHandler<GetPecaByIdQuery, Result<PecaResponse>> getByIdHandler,
        ILogger<PecaController> logger)
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
    /// Retorna todas as peÃ§as cadastradas.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Lista de peÃ§as.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<PecaResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<PecaResponse>>>> BuscarPecas(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a obtenÃ§Ã£o de todas as peÃ§as.");

        var result = await _getAllHandler.HandleAsync(new GetPecasQuery());

        if (!result.IsSuccess)
        {
            _logger.LogError("Erro ao obter as peÃ§as: {Erro}", result.Error);
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "NÃ£o foi possÃ­vel obter as peÃ§as."));
        }

        return Ok(ApiResponse<IReadOnlyCollection<PecaResponse>>.SuccessResponse(result.Value ?? []));
    }

    /// <summary>
    /// Retorna uma peÃ§a pelo identificador.
    /// </summary>
    /// <param name="id">Identificador da peÃ§a.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>PeÃ§a encontrada ou erro 404.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PecaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<PecaResponse>>> BuscarPecaPorId(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a obtenÃ§Ã£o da peÃ§a com Id: {Id}", id);

        var result = await _getByIdHandler.HandleAsync(new GetPecaByIdQuery { Id = id });

        if (!result.IsSuccess || result.Value is null)
        {
            _logger.LogError("PeÃ§a com Id: {Id} nÃ£o encontrada.", id);
            return NotFound(ApiResponse.FailureResponse(result.Error ?? "PeÃ§a nÃ£o encontrada."));
        }

        return Ok(ApiResponse<PecaResponse>.SuccessResponse(result.Value));
    }

    /// <summary>
    /// Cria uma nova peÃ§a.
    /// </summary>
    /// <param name="request">Dados da peÃ§a.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Id da peÃ§a criada ou erros de validaÃ§Ã£o.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<Guid>>> CriarPeca([FromBody] CreatePecaRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a criaÃ§Ã£o de uma nova peÃ§a. Nome: {Nome}", request.Nome);

        var validation = await _createValidator.ValidateAsync(request, cancellationToken);

        if (!validation.IsValid)
        {
            _logger.LogError("Erro ao validar a criaÃ§Ã£o da peÃ§a. Erros: {Erros}", string.Join(", ", validation.Errors.Select(x => x.ErrorMessage)));
            return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));
        }

        var result = await _createHandler.HandleAsync(new CreatePecaCommand
        {
            Nome = request.Nome,
            Descricao = request.Descricao,
            Codigo = request.Codigo,
            Valor = request.Valor,
            QuantidadeEstoque = request.QuantidadeEstoque,
            Ativo = request.Ativo
        });

        if (!result.IsSuccess)
        {
            _logger.LogError("Erro ao criar a peÃ§a. Erro: {Erro}", result.Error);
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "NÃ£o foi possÃ­vel criar a peÃ§a."));
        }

        return StatusCode(StatusCodes.Status201Created, ApiResponse<Guid>.SuccessResponse(result.Value, "PeÃ§a criada com sucesso."));
    }

    /// <summary>
    /// Atualiza uma peÃ§a existente.
    /// </summary>
    /// <param name="id">Identificador da peÃ§a.</param>
    /// <param name="request">Dados atualizados da peÃ§a.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Mensagem de sucesso, erro de validaÃ§Ã£o ou peÃ§a nÃ£o encontrada.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> AtualizarPeca(Guid id, [FromBody] UpdatePecaRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a atualizaÃ§Ã£o da peÃ§a com Id: {Id}", id);

        request.Id = id;

        var validation = await _updateValidator.ValidateAsync(request, cancellationToken);

        if (!validation.IsValid)
        {
            _logger.LogError("Erro ao validar a atualizaÃ§Ã£o da peÃ§a com Id: {Id}. Erros: {Erros}", id, string.Join(", ", validation.Errors.Select(x => x.ErrorMessage)));
            return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));
        }

        var result = await _updateHandler.HandleAsync(new UpdatePecaCommand
        {
            Id = id,
            Nome = request.Nome,
            Descricao = request.Descricao,
            Codigo = request.Codigo,
            Valor = request.Valor,
            QuantidadeEstoque = request.QuantidadeEstoque,
            Ativo = request.Ativo
        });

        if (!result.IsSuccess)
        {
            _logger.LogError("Erro ao atualizar a peÃ§a com Id: {Id}. Erro: {Erro}", id, result.Error);
            return result.Error == "PeÃ§a nÃ£o encontrada."
                ? NotFound(ApiResponse.FailureResponse(result.Error))
                : BadRequest(ApiResponse.FailureResponse(result.Error ?? "NÃ£o foi possÃ­vel atualizar a peÃ§a."));
        }

        return Ok(ApiResponse.SuccessResponse("PeÃ§a atualizada com sucesso."));
    }

    /// <summary>
    /// Desativa e remove logicamente uma peÃ§a existente.
    /// </summary>
    /// <param name="id">Identificador da peÃ§a.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Mensagem de sucesso ou erro 404.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> DeletarPeca(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a desativaÃ§Ã£o da peÃ§a com Id: {Id}", id);

        var result = await _deleteHandler.HandleAsync(new DeletePecaCommand { Id = id });

        if (!result.IsSuccess)
        {
            _logger.LogError("Erro ao desativar a peÃ§a com Id: {Id}. Erro: {Erro}", id, result.Error);
            return NotFound(ApiResponse.FailureResponse(result.Error ?? "PeÃ§a nÃ£o encontrada."));
        }

        return Ok(ApiResponse.SuccessResponse("PeÃ§a removida com sucesso."));
    }
}
