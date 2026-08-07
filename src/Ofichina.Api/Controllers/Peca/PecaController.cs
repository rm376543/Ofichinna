using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ofichina.Application.UseCases.Pecas.Commands;
using Ofichina.Application.UseCases.Pecas.Queries;
using Ofichina.Contracts;
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
[Route("api/peca")]
#pragma warning disable S6960
public sealed class PecaController : ControllerBase
#pragma warning restore S6960
{
    private readonly IValidator<CreatePecaRequest> _createValidator;
    private readonly IValidator<UpdatePecaRequest> _updateValidator;
    private readonly IMediator _mediator;
    private readonly ILogger<PecaController> _logger;

    /// <summary>
    /// Inicializa uma nova instância do controller de peças.
    /// </summary>
    public PecaController(
        IValidator<CreatePecaRequest> createValidator,
        IValidator<UpdatePecaRequest> updateValidator,
        IMediator mediator,
        ILogger<PecaController> logger)
    {
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Retorna todas as peças cadastradas.
    /// </summary>
    /// <param name="pagination">Parâmetros de paginação.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Lista de peças.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpGet("listar")]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<PecaResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<PagedResponse<PecaResponse>>>> BuscarTodasPecasPaginadas(
        [FromQuery] Pagination pagination,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a obtenção de todas as peças.");

        var result = await _mediator.Send(new GetAllPecasPaginadasQuery(pagination), cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogError("Erro ao obter as peças: {Erro}", result.Error);
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível obter as peças."));
        }

        return Ok(ApiResponse<PagedResponse<PecaResponse>>.SuccessResponse(result.Value));
    }

    /// <summary>
    /// Retorna uma peça pelo identificador.
    /// </summary>
    /// <param name="pecaId">Identificador da peça.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Peça encontrada ou erro 404.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpGet("detalhar/{pecaId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PecaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<PecaResponse>>> BuscarPecaPorId([FromRoute] Guid pecaId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a obtenção da peça com Id: {Id}", pecaId);

        var result = await _mediator.Send(new GetPecaByIdQuery(pecaId), cancellationToken);

        if (!result.IsSuccess || result.Value is null)
        {
            _logger.LogError("Peça com Id: {Id} não encontrada.", pecaId);
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
    [HttpPost("nova")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse>> CriarPeca([FromBody] CreatePecaRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a criação de uma nova peça. Nome: {Nome}", request.Nome);

        var validation = await _createValidator.ValidateAsync(request, cancellationToken);

        if (!validation.IsValid)
        {
            _logger.LogError("Erro ao validar a criação da peça. Erros: {Erros}", string.Join(", ", validation.Errors.Select(x => x.ErrorMessage)));
            return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));
        }

        var result = await _mediator.Send(new CreatePecaCommand
        {
            Nome = request.Nome,
            Descricao = request.Descricao,
            Codigo = request.Codigo,
            Valor = request.Valor,
            QuantidadeEstoque = request.QuantidadeEstoque
        }, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogError("Erro ao criar a peça. Erro: {Erro}", result.Error);
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível criar a peça."));
        }

        return Ok(ApiResponse.SuccessResponse("Peça criada com sucesso."));
    }

    /// <summary>
    /// Atualiza uma peça existente.
    /// </summary>
    /// <param name="request">Dados atualizados da peça.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Mensagem de sucesso, erro de validação ou peça não encontrada.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPut("atualizar")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> AtualizarPeca(
        [FromBody] UpdatePecaRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a atualização da peça com Id: {Id}", request.Id);

        var validation = await _updateValidator.ValidateAsync(request, cancellationToken);

        if (!validation.IsValid)
        {
            _logger.LogError("Erro ao validar a atualização da peça com Id: {Id}. Erros: {Erros}", request.Id, string.Join(", ", validation.Errors.Select(x => x.ErrorMessage)));
            return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));
        }

        var result = await _mediator.Send(new UpdatePecaCommand
        {
            Id = request.Id,
            Nome = request.Nome,
            Descricao = request.Descricao,
            Codigo = request.Codigo,
            Valor = request.Valor,
            QuantidadeEstoque = request.QuantidadeEstoque
        }, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogError("Erro ao atualizar a peça com Id: {Id}. Erro: {Erro}", request.Id, result.Error);
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível atualizar a peça."));
        }

        return Ok(ApiResponse.SuccessResponse("Peça atualizada com sucesso."));
    }

    /// <summary>
    /// Desativa e remove logicamente uma peça existente.
    /// </summary>
    /// <param name="request">Identificador da peça.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Mensagem de sucesso ou erro 404.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpDelete("remover")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> DeletarPeca([FromBody] RemovePecaRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a desativação da peça com Id: {Id}", request.Id);

        var result = await _mediator.Send(new DeletePecaCommand(request.Id), cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogError("Erro ao desativar a peça com Id: {Id}. Erro: {Erro}", request.Id, result.Error);
            return NotFound(ApiResponse.FailureResponse(result.Error ?? "Peça não encontrada."));
        }

        return Ok(ApiResponse.SuccessResponse("Peça removida com sucesso."));
    }
}


