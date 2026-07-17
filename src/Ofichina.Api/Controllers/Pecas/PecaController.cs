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
/// Controller responsável pelo CRUD de peças.
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
    /// Inicializa uma nova instância do controller de peças.
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
    /// Retorna todas as peças cadastradas.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Lista de peças.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<PecaResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<PecaResponse>>>> BuscarPecas(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a obtenção de todas as peças.");

        var result = await _getAllHandler.HandleAsync(new GetPecasQuery());

        if (!result.IsSuccess)
        {
            _logger.LogError("Erro ao obter as peças: {Erro}", result.Error);
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível obter as peças."));
        }

        return Ok(ApiResponse<IReadOnlyCollection<PecaResponse>>.SuccessResponse(result.Value ?? []));
    }

    /// <summary>
    /// Retorna uma peça pelo identificador.
    /// </summary>
    /// <param name="id">Identificador da peça.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Peça encontrada ou erro 404.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PecaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<PecaResponse>>> BuscarPecaPorId(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a obtenção da peça com Id: {Id}", id);

        var result = await _getByIdHandler.HandleAsync(new GetPecaByIdQuery { Id = id });

        if (!result.IsSuccess || result.Value is null)
        {
            _logger.LogError("Peça com Id: {Id} não encontrada.", id);
            return NotFound(ApiResponse.FailureResponse(result.Error ?? "Peça não encontrada."));
        }

        return Ok(ApiResponse<PecaResponse>.SuccessResponse(result.Value));
    }

    /// <summary>
    /// Cria uma nova peça.
    /// </summary>
    /// <param name="request">Dados da peça.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Id da peça criada ou erros de validação.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<Guid>>> CriarPeca([FromBody] CreatePecaRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a criação de uma nova peça. Nome: {Nome}", request.Nome);

        var validation = await _createValidator.ValidateAsync(request, cancellationToken);

        if (!validation.IsValid)
        {
            _logger.LogError("Erro ao validar a criação da peça. Erros: {Erros}", string.Join(", ", validation.Errors.Select(x => x.ErrorMessage)));
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
            _logger.LogError("Erro ao criar a peça. Erro: {Erro}", result.Error);
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível criar a peça."));
        }

        return StatusCode(StatusCodes.Status201Created, ApiResponse<Guid>.SuccessResponse(result.Value, "Peça criada com sucesso."));
    }

    /// <summary>
    /// Atualiza uma peça existente.
    /// </summary>
    /// <param name="id">Identificador da peça.</param>
    /// <param name="request">Dados atualizados da peça.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Mensagem de sucesso, erro de validação ou peça não encontrada.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> AtualizarPeca(Guid id, [FromBody] UpdatePecaRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a atualização da peça com Id: {Id}", id);

        request.Id = id;

        var validation = await _updateValidator.ValidateAsync(request, cancellationToken);

        if (!validation.IsValid)
        {
            _logger.LogError("Erro ao validar a atualização da peça com Id: {Id}. Erros: {Erros}", id, string.Join(", ", validation.Errors.Select(x => x.ErrorMessage)));
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
            _logger.LogError("Erro ao atualizar a peça com Id: {Id}. Erro: {Erro}", id, result.Error);
            return result.Error == "Peça não encontrada."
                ? NotFound(ApiResponse.FailureResponse(result.Error))
                : BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível atualizar a peça."));
        }

        return Ok(ApiResponse.SuccessResponse("Peça atualizada com sucesso."));
    }

    /// <summary>
    /// Desativa e remove logicamente uma peça existente.
    /// </summary>
    /// <param name="id">Identificador da peça.</param>
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
        _logger.LogInformation("Iniciando a desativação da peça com Id: {Id}", id);

        var result = await _deleteHandler.HandleAsync(new DeletePecaCommand { Id = id });

        if (!result.IsSuccess)
        {
            _logger.LogError("Erro ao desativar a peça com Id: {Id}. Erro: {Erro}", id, result.Error);
            return NotFound(ApiResponse.FailureResponse(result.Error ?? "Peça não encontrada."));
        }

        return Ok(ApiResponse.SuccessResponse("Peça removida com sucesso."));
    }
}