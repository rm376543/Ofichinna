using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.OrdensServico.ItemServico.Commands;
using Ofichina.Application.UseCases.OrdensServico.ItemServico.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.OrdemServico;
using Ofichina.Contracts.Responses;
using Ofichina.Contracts.Responses.OrdemServico;

namespace Ofichina.Api.Controllers.ItemServico;

/// <summary>
/// Controller responsÃ¡vel pelo CRUD de itens de serviÃ§o vinculados Ã  ordem de serviÃ§o.
/// </summary>
[Authorize]
[ApiController]
[Route("api/ordens-servico/{ordemServicoId:guid}/itens-servico")]
public sealed class ItemServicoController : ControllerBase
{
    private readonly IValidator<CreateItemServicoRequest> _createValidator;
    private readonly IValidator<UpdateItemServicoRequest> _updateValidator;
    private readonly ILogger<ItemServicoController> _logger;

    public ItemServicoController(
        IValidator<CreateItemServicoRequest> createValidator,
        IValidator<UpdateItemServicoRequest> updateValidator,
        ILogger<ItemServicoController> logger)
    {
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _logger = logger;
    }

    /// <summary>
    /// Retorna todos os itens de serviÃ§o de uma ordem de serviÃ§o.
    /// </summary>
    /// <param name="ordemServicoId">Identificador da ordem de serviÃ§o.</param>
    /// <param name="handler">Handler de consulta dos itens de serviÃ§o.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Lista de itens de serviÃ§o da ordem.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<ItemServicoResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<ItemServicoResponse>>>> BuscarItensServico(
        Guid ordemServicoId,
        [FromServices] IQueryHandler<GetItemServicosByOrdemServicoQuery, Result<IReadOnlyCollection<ItemServicoResponse>>> handler,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a obtenÃ§Ã£o dos itens de serviÃ§o da ordem. OrdemServicoId: {OrdemServicoId}.", ordemServicoId);

        var result = await handler.HandleAsync(new GetItemServicosByOrdemServicoQuery
        {
            OrdemServicoId = ordemServicoId
        });

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Falha ao obter itens de serviÃ§o da ordem. OrdemServicoId: {OrdemServicoId}. Erro: {Erro}", ordemServicoId, result.Error);
            return NotFound(ApiResponse.FailureResponse(result.Error ?? "NÃ£o foi possÃ­vel obter os itens de serviÃ§o."));
        }

        return Ok(ApiResponse<IReadOnlyCollection<ItemServicoResponse>>.SuccessResponse(result.Value ?? []));
    }

    /// <summary>
    /// Retorna um item de serviÃ§o especÃ­fico de uma ordem de serviÃ§o.
    /// </summary>
    /// <param name="ordemServicoId">Identificador da ordem de serviÃ§o.</param>
    /// <param name="id">Identificador do item de serviÃ§o.</param>
    /// <param name="handler">Handler de consulta do item de serviÃ§o.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Item de serviÃ§o encontrado ou erro 404.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ItemServicoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ItemServicoResponse>>> BuscarItemServicoPorId(
        Guid ordemServicoId,
        Guid id,
        [FromServices] IQueryHandler<GetItemServicoByIdQuery, Result<ItemServicoResponse>> handler,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a obtenÃ§Ã£o do item de serviÃ§o. OrdemServicoId: {OrdemServicoId}, ItemServicoId: {ItemServicoId}.", ordemServicoId, id);

        var result = await handler.HandleAsync(new GetItemServicoByIdQuery
        {
            OrdemServicoId = ordemServicoId,
            Id = id
        });

        if (!result.IsSuccess || result.Value is null)
        {
            _logger.LogWarning("Item de serviÃ§o nÃ£o encontrado. OrdemServicoId: {OrdemServicoId}, ItemServicoId: {ItemServicoId}.", ordemServicoId, id);
            return NotFound(ApiResponse.FailureResponse(result.Error ?? "Item de serviÃ§o nÃ£o encontrado."));
        }

        return Ok(ApiResponse<ItemServicoResponse>.SuccessResponse(result.Value));
    }

    /// <summary>
    /// Cria um novo item de serviÃ§o vinculado Ã  ordem de serviÃ§o.
    /// </summary>
    /// <param name="ordemServicoId">Identificador da ordem de serviÃ§o.</param>
    /// <param name="request">Dados do item de serviÃ§o.</param>
    /// <param name="handler">Handler de criaÃ§Ã£o do item de serviÃ§o.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Identificador do item criado ou erro de validaÃ§Ã£o.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<Guid>>> CriarItemServico(
        Guid ordemServicoId,
        [FromBody] CreateItemServicoRequest request,
        [FromServices] ICommandHandler<CreateItemServicoCommand, Result<Guid>> handler,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a criaÃ§Ã£o de item de serviÃ§o. OrdemServicoId: {OrdemServicoId}, ServicoId: {ServicoId}.", ordemServicoId, request.ServicoId);

        var validation = await _createValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            _logger.LogWarning("Falha na validaÃ§Ã£o do item de serviÃ§o. OrdemServicoId: {OrdemServicoId}. Erros: {Erros}", ordemServicoId, string.Join(", ", validation.Errors.Select(x => x.ErrorMessage)));
            return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));
        }

        var result = await handler.HandleAsync(new CreateItemServicoCommand
        {
            OrdemServicoId = ordemServicoId,
            ServicoId = request.ServicoId
        });

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Falha ao criar item de serviÃ§o. OrdemServicoId: {OrdemServicoId}, ServicoId: {ServicoId}. Erro: {Erro}", ordemServicoId, request.ServicoId, result.Error);
            return result.Error == "Ordem de serviÃ§o nÃ£o encontrada." || result.Error == "ServiÃ§o nÃ£o encontrado."
                ? NotFound(ApiResponse.FailureResponse(result.Error))
                : BadRequest(ApiResponse.FailureResponse(result.Error ?? "NÃ£o foi possÃ­vel criar o item de serviÃ§o."));
        }

        return StatusCode(StatusCodes.Status201Created, ApiResponse<Guid>.SuccessResponse(result.Value, "Item de serviÃ§o criado com sucesso."));
    }

    /// <summary>
    /// Atualiza um item de serviÃ§o vinculado Ã  ordem de serviÃ§o.
    /// </summary>
    /// <param name="ordemServicoId">Identificador da ordem de serviÃ§o.</param>
    /// <param name="id">Identificador do item de serviÃ§o.</param>
    /// <param name="request">Dados atualizados do item de serviÃ§o.</param>
    /// <param name="handler">Handler de atualizaÃ§Ã£o do item de serviÃ§o.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Mensagem de sucesso, erro de validaÃ§Ã£o ou item nÃ£o encontrado.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> AtualizarItemServico(
        Guid ordemServicoId,
        Guid id,
        [FromBody] UpdateItemServicoRequest request,
        [FromServices] ICommandHandler<UpdateItemServicoCommand, Result> handler,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a atualizaÃ§Ã£o do item de serviÃ§o. OrdemServicoId: {OrdemServicoId}, ItemServicoId: {ItemServicoId}.", ordemServicoId, id);

        request.Id = id;

        var validation = await _updateValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            _logger.LogWarning("Falha na validaÃ§Ã£o de atualizaÃ§Ã£o do item de serviÃ§o. OrdemServicoId: {OrdemServicoId}, ItemServicoId: {ItemServicoId}. Erros: {Erros}", ordemServicoId, id, string.Join(", ", validation.Errors.Select(x => x.ErrorMessage)));
            return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));
        }

        var result = await handler.HandleAsync(new UpdateItemServicoCommand
        {
            OrdemServicoId = ordemServicoId,
            Id = id,
            Descricao = request.Descricao,
            Valor = request.Valor
        });

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Falha ao atualizar item de serviÃ§o. OrdemServicoId: {OrdemServicoId}, ItemServicoId: {ItemServicoId}. Erro: {Erro}", ordemServicoId, id, result.Error);
            return result.Error == "Ordem de serviÃ§o nÃ£o encontrada." || result.Error == "Item de serviÃ§o nÃ£o encontrado."
                ? NotFound(ApiResponse.FailureResponse(result.Error))
                : BadRequest(ApiResponse.FailureResponse(result.Error ?? "NÃ£o foi possÃ­vel atualizar o item de serviÃ§o."));
        }

        return Ok(ApiResponse.SuccessResponse("Item de serviÃ§o atualizado com sucesso."));
    }

    /// <summary>
    /// Remove um item de serviÃ§o da ordem de serviÃ§o.
    /// </summary>
    /// <param name="ordemServicoId">Identificador da ordem de serviÃ§o.</param>
    /// <param name="id">Identificador do item de serviÃ§o.</param>
    /// <param name="handler">Handler de remoÃ§Ã£o do item de serviÃ§o.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Mensagem de sucesso ou erro 404.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> RemoverItemServico(
        Guid ordemServicoId,
        Guid id,
        [FromServices] ICommandHandler<DeleteItemServicoCommand, Result> handler,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a remoÃ§Ã£o do item de serviÃ§o. OrdemServicoId: {OrdemServicoId}, ItemServicoId: {ItemServicoId}.", ordemServicoId, id);

        var result = await handler.HandleAsync(new DeleteItemServicoCommand
        {
            OrdemServicoId = ordemServicoId,
            Id = id
        });

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Falha ao remover item de serviÃ§o. OrdemServicoId: {OrdemServicoId}, ItemServicoId: {ItemServicoId}. Erro: {Erro}", ordemServicoId, id, result.Error);
            return result.Error == "Ordem de serviÃ§o nÃ£o encontrada." || result.Error == "Item de serviÃ§o nÃ£o encontrado."
                ? NotFound(ApiResponse.FailureResponse(result.Error))
                : BadRequest(ApiResponse.FailureResponse(result.Error ?? "NÃ£o foi possÃ­vel remover o item de serviÃ§o."));
        }

        return Ok(ApiResponse.SuccessResponse("Item de serviÃ§o removido com sucesso."));
    }
}

