using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ofichina.Application.UseCases.OrdensServico.ItemServico.Commands;
using Ofichina.Application.UseCases.OrdensServico.ItemServico.Queries;
using Ofichina.Contracts.Requests.OrdemServico;
using Ofichina.Contracts.Responses;
using Ofichina.Contracts.Responses.OrdemServico;

namespace Ofichina.Api.Controllers.ItemServico;

/// <summary>
/// Controller responsável pelo CRUD de itens de serviço vinculados à ordem de serviço.
/// </summary>
[Authorize]
[ApiController]
[Route("api/ordens-servico/{ordemServicoId:guid}/itens-servico")]
#pragma warning disable S6960
public sealed class ItemServicoController : ControllerBase
#pragma warning restore S6960
{
    private readonly IValidator<CreateItemServicoRequest> _createValidator;
    private readonly IValidator<CreateItemServicoPecaRequest> _createPecaValidator;
    private readonly IValidator<UpdateItemServicoRequest> _updateValidator;
    private readonly IMediator _mediator;
    private readonly ILogger<ItemServicoController> _logger;

    public ItemServicoController(
        IValidator<CreateItemServicoRequest> createValidator,
        IValidator<CreateItemServicoPecaRequest> createPecaValidator,
        IValidator<UpdateItemServicoRequest> updateValidator,
        IMediator mediator,
        ILogger<ItemServicoController> logger)
    {
        _createValidator = createValidator;
        _createPecaValidator = createPecaValidator;
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
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<ItemServicoResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<ItemServicoResponse>>>> BuscarItensServico(
        Guid ordemServicoId,
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

        return Ok(ApiResponse<IReadOnlyCollection<ItemServicoResponse>>.SuccessResponse(result.Value ?? []));
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
    [ProducesResponseType(typeof(ApiResponse<ItemServicoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ItemServicoResponse>>> BuscarItemServicoPorId(
        Guid ordemServicoId,
        Guid id,
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

        return Ok(ApiResponse<ItemServicoResponse>.SuccessResponse(result.Value));
    }

    /// <summary>
    /// Cria um novo item de serviço vinculado à ordem de serviço.
    /// </summary>
    /// <param name="ordemServicoId">Identificador da ordem de serviço.</param>
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
        Guid ordemServicoId,
        [FromBody] CreateItemServicoRequest request,
        CancellationToken cancellationToken)
    {
        return await ProcessarCriacaoItemServico(ordemServicoId, request, cancellationToken);
    }

    /// <summary>
    /// Adiciona uma peça a um item de serviço existente.
    /// </summary>
    /// <param name="ordemServicoId">Identificador da ordem de serviço.</param>
    /// <param name="itemServicoId">Identificador do item de serviço.</param>
    /// <param name="request">Dados da peça.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Identificador da peça adicionada ou erro de validação.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPost("{itemServicoId:guid}/pecas")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<Guid>>> AdicionarPecaAoItemServico(
        Guid ordemServicoId,
        Guid itemServicoId,
        [FromBody] CreateItemServicoPecaRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a inclusão de peça no item de serviço. OrdemServicoId: {OrdemServicoId}, ItemServicoId: {ItemServicoId}, PecaId: {PecaId}.", ordemServicoId, itemServicoId, request.PecaId);

        var validation = await _createPecaValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            _logger.LogWarning("Falha na validação da peça do item de serviço. OrdemServicoId: {OrdemServicoId}, ItemServicoId: {ItemServicoId}. Erros: {Erros}", ordemServicoId, itemServicoId, string.Join(", ", validation.Errors.Select(x => x.ErrorMessage)));
            return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));
        }

        var result = await _mediator.Send(new CreateItemServicoPecaCommand
        {
            OrdemServicoId = ordemServicoId,
            ItemServicoId = itemServicoId,
            PecaId = request.PecaId,
            Quantidade = request.Quantidade
        }, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Falha ao adicionar peça ao item de serviço. OrdemServicoId: {OrdemServicoId}, ItemServicoId: {ItemServicoId}, PecaId: {PecaId}. Erro: {Erro}", ordemServicoId, itemServicoId, request.PecaId, result.Error);
            return result.Error == "Ordem de serviço não encontrada." || result.Error == "Item de serviço não encontrado." || result.Error == "Peça não encontrada."
                ? NotFound(ApiResponse.FailureResponse(result.Error))
                : BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível adicionar a peça ao item de serviço."));
        }

        return StatusCode(StatusCodes.Status201Created, ApiResponse<Guid>.SuccessResponse(result.Value, "Peça adicionada ao item de serviço com sucesso."));
    }

    private async Task<ActionResult<ApiResponse<Guid>>> ProcessarCriacaoItemServico(
        Guid ordemServicoId,
        CreateItemServicoRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a criação de item de serviço. OrdemServicoId: {OrdemServicoId}, ServicoId: {ServicoId}.", ordemServicoId, request.ServicoId);

        var validation = await _createValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            _logger.LogWarning("Falha na validação do item de serviço. OrdemServicoId: {OrdemServicoId}. Erros: {Erros}", ordemServicoId, string.Join(", ", validation.Errors.Select(x => x.ErrorMessage)));
            return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));
        }

        var result = await _mediator.Send(new CreateItemServicoCommand
        {
            OrdemServicoId = ordemServicoId,
            ServicoId = request.ServicoId
        }, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Falha ao criar item de serviço. OrdemServicoId: {OrdemServicoId}, ServicoId: {ServicoId}. Erro: {Erro}", ordemServicoId, request.ServicoId, result.Error);
            return result.Error == "Ordem de serviço não encontrada." || result.Error == "Serviço não encontrado."
                ? NotFound(ApiResponse.FailureResponse(result.Error))
                : BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível criar o item de serviço."));
        }

        return StatusCode(StatusCodes.Status201Created, ApiResponse<Guid>.SuccessResponse(result.Value, "Item de serviço criado com sucesso."));
    }

    /// <summary>
    /// Atualiza um item de serviço vinculado à ordem de serviço.
    /// </summary>
    /// <param name="ordemServicoId">Identificador da ordem de serviço.</param>
    /// <param name="id">Identificador do item de serviço.</param>
    /// <param name="request">Dados atualizados do item de serviço.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Mensagem de sucesso, erro de validação ou item não encontrado.</returns>
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
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a atualização do item de serviço. OrdemServicoId: {OrdemServicoId}, ItemServicoId: {ItemServicoId}.", ordemServicoId, id);

        request.Id = id;

        var validation = await _updateValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            _logger.LogWarning("Falha na validação de atualização do item de serviço. OrdemServicoId: {OrdemServicoId}, ItemServicoId: {ItemServicoId}. Erros: {Erros}", ordemServicoId, id, string.Join(", ", validation.Errors.Select(x => x.ErrorMessage)));
            return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));
        }

        var result = await _mediator.Send(new UpdateItemServicoCommand
        {
            OrdemServicoId = ordemServicoId,
            Id = id,
            Descricao = request.Descricao,
            Valor = request.Valor
        }, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Falha ao atualizar item de serviço. OrdemServicoId: {OrdemServicoId}, ItemServicoId: {ItemServicoId}. Erro: {Erro}", ordemServicoId, id, result.Error);
            return result.Error == "Ordem de serviço não encontrada." || result.Error == "Item de serviço não encontrado."
                ? NotFound(ApiResponse.FailureResponse(result.Error))
                : BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível atualizar o item de serviço."));
        }

        return Ok(ApiResponse.SuccessResponse("Item de serviço atualizado com sucesso."));
    }

    /// <summary>
    /// Remove um item de serviço da ordem de serviço.
    /// </summary>
    /// <param name="ordemServicoId">Identificador da ordem de serviço.</param>
    /// <param name="id">Identificador do item de serviço.</param>
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
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a remoção do item de serviço. OrdemServicoId: {OrdemServicoId}, ItemServicoId: {ItemServicoId}.", ordemServicoId, id);

        var result = await _mediator.Send(new DeleteItemServicoCommand
        {
            OrdemServicoId = ordemServicoId,
            Id = id
        }, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Falha ao remover item de serviço. OrdemServicoId: {OrdemServicoId}, ItemServicoId: {ItemServicoId}. Erro: {Erro}", ordemServicoId, id, result.Error);
            return result.Error == "Ordem de serviço não encontrada." || result.Error == "Item de serviço não encontrado."
                ? NotFound(ApiResponse.FailureResponse(result.Error))
                : BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível remover o item de serviço."));
        }

        return Ok(ApiResponse.SuccessResponse("Item de serviço removido com sucesso."));
    }
}



