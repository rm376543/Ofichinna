using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ofichina.Application.UseCases.ItensServico.Commands;
using Ofichina.Application.UseCases.ItensServico.Queries;
using Ofichina.Contracts.Requests.ItensServico;
using Ofichina.Contracts.Responses;
using Ofichina.Contracts.Responses.OrdemServico;

namespace Ofichina.Api.Controllers.ItensServico;

/// <summary>
/// Controller responsável pelo CRUD de itens de serviço vinculados à ordem de serviço.
/// </summary>
[Authorize]
[ApiController]
[Route("api/item-servico")]
#pragma warning disable S6960
public sealed class ItemServicoController : ControllerBase
#pragma warning restore S6960
{
    private readonly IValidator<CreateItemServicoRequest> _createValidator;
    private readonly IValidator<UpdateItemServicoRequest> _updateValidator;
    private readonly IMediator _mediator;
    private readonly ILogger<ItemServicoController> _logger;

    public ItemServicoController(
        IValidator<CreateItemServicoRequest> createValidator,
        IValidator<UpdateItemServicoRequest> updateValidator,
        IMediator mediator,
        ILogger<ItemServicoController> logger)
    {
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Retorna todos os itens de serviço de uma ordem de serviço.
    /// </summary>
    /// <param name="ordemServicoId">Identificador da ordem de serviço.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Lista de itens de serviço da ordem.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<OrdemServicoItensResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<OrdemServicoItensResponse>>>> BuscarItensServico(
        [FromQuery] Guid ordemServicoId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a obtenção dos itens de serviço da ordem. OrdemServicoId: {OrdemServicoId}.", ordemServicoId);

        var result = await _mediator.Send(new GetItemServicosByOrdemServicoQuery
        {
            OrdemServicoId = ordemServicoId
        }, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Falha ao obter itens de serviço da ordem. OrdemServicoId: {OrdemServicoId}. Erro: {Erro}", ordemServicoId, result.Error);
            return NotFound(ApiResponse.FailureResponse(result.Error ?? "Não foi possível obter os itens de serviço."));
        }

        return Ok(ApiResponse<IReadOnlyCollection<OrdemServicoItensResponse>>.SuccessResponse(result.Value ?? []));
    }

    /// <summary>
    /// Retorna um item de serviço específico de uma ordem de serviço.
    /// </summary>
    /// <param name="ordemServicoId">Identificador da ordem de serviço.</param>
    /// <param name="id">Identificador do item de serviço.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Item de serviço encontrado ou erro 404.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<OrdemServicoItensResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<OrdemServicoItensResponse>>> BuscarItemServicoPorId(
        [FromQuery] Guid ordemServicoId,
        [FromQuery] Guid id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a obtenção do item de serviço. OrdemServicoId: {OrdemServicoId}, ItemServicoId: {ItemServicoId}.", ordemServicoId, id);

        var result = await _mediator.Send(new GetItemServicoByIdQuery
        {
            OrdemServicoId = ordemServicoId,
            Id = id
        }, cancellationToken);

        if (!result.IsSuccess || result.Value is null)
        {
            _logger.LogWarning("Item de serviço não encontrado. OrdemServicoId: {OrdemServicoId}, ItemServicoId: {ItemServicoId}.", ordemServicoId, id);
            return NotFound(ApiResponse.FailureResponse(result.Error ?? "Item de serviço não encontrado."));
        }

        return Ok(ApiResponse<OrdemServicoItensResponse>.SuccessResponse(result.Value));
    }

    /// <summary>
    /// Cria um novo item de serviço vinculado à ordem de serviço.
    /// </summary>
    /// <param name="request">Dados do item de serviço.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Identificador do item criado ou erro de validação.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<Guid>>> CriarItemServico(
        [FromBody] CreateItemServicoRequest request,
        CancellationToken cancellationToken)
    {
        return await ProcessarCriacaoItemServico(request, cancellationToken);
    }

    private async Task<ActionResult<ApiResponse<Guid>>> ProcessarCriacaoItemServico(
        CreateItemServicoRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a criação de item de serviço. OrdemServicoId: {OrdemServicoId}.", request.OrdemServicoId);

        var validation = await _createValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            _logger.LogWarning("Falha na validação do item de serviço. OrdemServicoId: {OrdemServicoId}. Erros: {Erros}", request.OrdemServicoId, string.Join(", ", validation.Errors.Select(x => x.ErrorMessage)));
            return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));
        }

        var result = await _mediator.Send(new CreateItemServicoCommand
        {
            OrdemServicoId = request.OrdemServicoId,
            ServicoId = request.ServicoId,
            PecaId = request.PecaId,
            Quantidade = request.Quantidade
        }, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Falha ao criar item de serviço. OrdemServicoId: {OrdemServicoId}. Erro: {Erro}", request.OrdemServicoId, result.Error);
            return result.Error == "Ordem de serviço não encontrada." || result.Error == "Serviço não encontrado." || result.Error == "Peça não encontrada."
                ? NotFound(ApiResponse.FailureResponse(result.Error))
                : BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível criar o item de serviço."));
        }

        return StatusCode(StatusCodes.Status201Created, ApiResponse<Guid>.SuccessResponse(result.Value, "Item de serviço criado com sucesso."));
    }

    /// <summary>
    /// Atualiza um item de serviço vinculado à ordem de serviço.
    /// </summary>
    /// <param name="request">Dados atualizados do item de serviço.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Mensagem de sucesso, erro de validação ou item não encontrado.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPut]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse>> AtualizarItemServico(
        [FromBody] UpdateItemServicoRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a atualização do item de serviço. OrdemServicoId: {OrdemServicoId}, ItemServicoId: {ItemServicoId}.", request.OrdemServicoId, request.Id);

        var validation = await _updateValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            _logger.LogWarning("Falha na validação de atualização do item de serviço. OrdemServicoId: {OrdemServicoId}, ItemServicoId: {ItemServicoId}. Erros: {Erros}", request.OrdemServicoId, request.Id, string.Join(", ", validation.Errors.Select(x => x.ErrorMessage)));
            return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));
        }

        var result = await _mediator.Send(new UpdateItemServicoCommand
        {
            Id = request.Id,
            OrdemServicoId = request.OrdemServicoId,
            ServicoId = request.ServicoId,
            PecaId = request.PecaId,
            Quantidade = request.Quantidade
        }, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Falha ao atualizar item de serviço. OrdemServicoId: {OrdemServicoId}, ItemServicoId: {ItemServicoId}. Erro: {Erro}", request.OrdemServicoId, request.Id, result.Error);
#pragma warning disable S3358
            return result.Error == "Ordem de serviço não encontrada." || result.Error == "Item de serviço não encontrado." || result.Error == "Serviço não encontrado." || result.Error == "Peça não encontrada."
                ? NotFound(ApiResponse.FailureResponse(result.Error))
                : BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível atualizar o item de serviço."));
#pragma warning restore S3358
        }

        return Ok(ApiResponse.SuccessResponse("Item de serviço atualizado com sucesso."));
    }

    /// <summary>
    /// Remove um item de serviço da ordem de serviço.
    /// </summary>
    /// <param name="request">Dados necessários para exclusão do item de serviço.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Mensagem de sucesso ou erro 404.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpDelete]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> RemoverItemServico(
        [FromBody] DeleteItemServicoRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a remoção do item de serviço. OrdemServicoId: {OrdemServicoId}, ItemServicoId: {ItemServicoId}.", request.OrdemServicoId, request.Id);

        var result = await _mediator.Send(new DeleteItemServicoCommand
        {
            Id = request.Id,
            OrdemServicoId = request.OrdemServicoId
        }, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Falha ao remover item de serviço. OrdemServicoId: {OrdemServicoId}, ItemServicoId: {ItemServicoId}. Erro: {Erro}", request.OrdemServicoId, request.Id, result.Error);
            return result.Error == "Ordem de serviço não encontrada." || result.Error == "Item de serviço não encontrado."
                ? NotFound(ApiResponse.FailureResponse(result.Error))
                : BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível remover o item de serviço."));
        }

        return Ok(ApiResponse.SuccessResponse("Item de serviço removido com sucesso."));
    }
}



