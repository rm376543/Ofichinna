using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ofichina.Application.UseCases.ItensServico.Commands;
using Ofichina.Application.UseCases.ItensServico.Queries;
using Ofichina.Contracts.Requests.ItensServico;
using Ofichina.Contracts.Responses;
using Ofichina.Contracts.Responses.Orcamento;
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
    private readonly IValidator<CreateItemOrcamentoRequest> _createOrcamentoItemValidator;
    private readonly IValidator<UpdateItemOrcamentoRequest> _updateOrcamentoValidator;
    private readonly IValidator<UpdateItemServicoRequest> _updateValidator;
    private readonly IMediator _mediator;
    private readonly ILogger<ItemServicoController> _logger;

    public ItemServicoController(
        IValidator<CreateItemServicoRequest> createValidator,
        IValidator<CreateItemOrcamentoRequest> createOrcamentoItemValidator,
        IValidator<UpdateItemOrcamentoRequest> updateOrcamentoValidator,
        IValidator<UpdateItemServicoRequest> updateValidator,
        IMediator mediator,
        ILogger<ItemServicoController> logger)
    {
        _createValidator = createValidator;
        _createOrcamentoItemValidator = createOrcamentoItemValidator;
        _updateOrcamentoValidator = updateOrcamentoValidator;
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
    [HttpGet("buscar-por-ordem-servico/{ordemServicoId}")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<OrdemServicoItensResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<OrdemServicoItensResponse>>>> BuscarItensServico(
        [FromRoute] Guid ordemServicoId,
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
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível obter os itens de serviço."));
        }

        return Ok(ApiResponse<IReadOnlyCollection<OrdemServicoItensResponse>>.SuccessResponse(result.Value ?? []));
    }

    /// <summary>
    /// Retorna todos os itens de serviço de um orçamento.
    /// </summary>
    /// <param name="orcamentoId">Identificador do orçamento.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Lista de itens de serviço do orçamento.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpGet("buscar-por-orcamento/{orcamentoId}")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<OrcamentoItemResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<OrcamentoItemResponse>>>> BuscarItensOrcamento(
        [FromRoute] Guid orcamentoId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a obtenção dos itens de serviço do orçamento. OrcamentoId: {OrcamentoId}.", orcamentoId);

        var result = await _mediator.Send(new GetItemServicosByOrcamentoQuery
        {
            OrcamentoId = orcamentoId
        }, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Falha ao obter itens de serviço do orçamento. OrcamentoId: {OrcamentoId}. Erro: {Erro}", orcamentoId, result.Error);
            return NotFound(ApiResponse.FailureResponse(result.Error ?? "Não foi possível obter os itens de serviço do orçamento."));
        }

        return Ok(ApiResponse<IReadOnlyCollection<OrcamentoItemResponse>>.SuccessResponse(result.Value ?? []));
    }

    /// <summary>
    /// Retorna um item de serviço específico de uma ordem de serviço.
    /// </summary>
    /// <param name="ordemServicoId">Identificador da ordem de serviço.</param>
    /// <param name="itemServicoId">Identificador do item de serviço.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Item de serviço encontrado ou erro 404.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpGet("buscar-por-ordem-servico/{ordemServicoId}/{itemServicoId}")]
    [ProducesResponseType(typeof(ApiResponse<OrdemServicoItensResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<OrdemServicoItensResponse>>> BuscarItemServicoPorId(
        [FromRoute] Guid ordemServicoId,
        [FromRoute] Guid itemServicoId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a obtenção do item de serviço. OrdemServicoId: {OrdemServicoId}, ItemServicoId: {ItemServicoId}.", ordemServicoId, itemServicoId);

        var result = await _mediator.Send(new GetItemServicoByIdQuery(
            ordemServicoId, itemServicoId
        ), cancellationToken);

        if (!result.IsSuccess || result.Value is null)
        {
            _logger.LogWarning("Item de serviço não encontrado. OrdemServicoId: {OrdemServicoId}, ItemServicoId: {ItemServicoId}.", ordemServicoId, itemServicoId);
            return NotFound(ApiResponse.FailureResponse(result.Error ?? "Item de serviço não encontrado."));
        }

        return Ok(ApiResponse<OrdemServicoItensResponse>.SuccessResponse(result.Value));
    }

    /// <summary>
    /// Retorna um item de serviço específico de um orçamento.
    /// </summary>
    /// <param name="orcamentoId">Identificador do orçamento.</param>
    /// <param name="itemServicoId">Identificador do item de serviço.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Item de serviço encontrado ou erro 404.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpGet("buscar-por-orcamento/{orcamentoId}/{itemServicoId}")]
    [ProducesResponseType(typeof(ApiResponse<OrcamentoItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<OrcamentoItemResponse>>> BuscarItemServicoOrcamentoPorId(
        [FromRoute] Guid orcamentoId,
        [FromRoute] Guid itemServicoId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a obtenção do item de serviço do orçamento. OrcamentoId: {OrcamentoId}, ItemServicoId: {ItemServicoId}.", orcamentoId, itemServicoId);

        var result = await _mediator.Send(new GetItemServicoByOrcamentoIdQuery(
            orcamentoId, itemServicoId
        ), cancellationToken);

        if (!result.IsSuccess || result.Value is null)
        {
            _logger.LogWarning("Item de serviço do orçamento não encontrado. OrcamentoId: {OrcamentoId}, ItemServicoId: {ItemServicoId}.", orcamentoId, itemServicoId);
            return NotFound(ApiResponse.FailureResponse(result.Error ?? "Item de serviço do orçamento não encontrado."));
        }

        return Ok(ApiResponse<OrcamentoItemResponse>.SuccessResponse(result.Value));
    }

    /// <summary>
    /// Cria um novo item de serviço vinculado à ordem de serviço.
    /// </summary>
    /// <param name="request">Dados do item de serviço.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Identificador do item criado ou erro de validação.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPost("adicionar/para-ordem-servico")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> CriarItemServico(
        [FromBody] CreateItemServicoRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a criação de item de serviço. OrdemServicoId: {OrdemServicoId}.", request.OrdemServicoId);

        var validation = await _createValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            _logger.LogWarning("Falha na validação do item de serviço. OrdemServicoId: {OrdemServicoId}. Erros: {Erros}", request.OrdemServicoId, string.Join(", ", validation.Errors.Select(x => x.ErrorMessage)));
            return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));
        }

        var result = await _mediator.Send(new CreateItemServicoCommand(request), cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Falha ao criar item de serviço. OrdemServicoId: {OrdemServicoId}. Erro: {Erro}", request.OrdemServicoId, result.Error);
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível criar o item de serviço."));
        }

        return Ok(ApiResponse.SuccessResponse("Item de serviço criado com sucesso."));
    }

    /// <summary>
    /// Cria um novo item de serviço vinculado ao orçamento.
    /// </summary>
    /// <param name="request">Dados do item de serviço para o orçamento.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Identificador do item criado ou erro de validação.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPost("adicionar/para-orcamento")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> CriarItemOrcamento(
        [FromBody] CreateItemOrcamentoRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a criação de item de serviço no orçamento. OrcamentoId: {OrcamentoId}.", request.OrcamentoId);

        var validation = await _createOrcamentoItemValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            _logger.LogWarning("Falha na validação do item de serviço do orçamento. OrcamentoId: {OrcamentoId}. Erros: {Erros}", request.OrcamentoId, string.Join(", ", validation.Errors.Select(x => x.ErrorMessage)));
            return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));
        }

        var result = await _mediator.Send(new CreateItemOrcamentoCommand(request), cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Falha ao criar item de serviço no orçamento. OrcamentoId: {OrcamentoId}. Erro: {Erro}", request.OrcamentoId, result.Error);
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Falha ao tentar criar itens de servico vinculado ao orcamento."));
        }

        return Ok(ApiResponse.SuccessResponse("Item de serviço criado com sucesso no orçamento."));
    }

    /// <summary>
    /// Atualiza um item de serviço vinculado ao orçamento.
    /// </summary>
    /// <param name="request">Dados atualizados do item de serviço para o orçamento.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Mensagem de sucesso, erro de validação ou item não encontrado.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPut("atualizar/para-orcamento")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse>> AtualizarItemOrcamento(
        [FromBody] UpdateItemOrcamentoRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a atualização do item de serviço do orçamento. OrcamentoId: {OrcamentoId}, ItemServicoId: {ItemServicoId}.", request.OrcamentoId, request.ItemServicoId);

        var validation = await _updateOrcamentoValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            _logger.LogWarning("Falha na validação de atualização do item de serviço do orçamento. OrcamentoId: {OrcamentoId}, ItemServicoId: {ItemServicoId}. Erros: {Erros}", request.OrcamentoId, request.ItemServicoId, string.Join(", ", validation.Errors.Select(x => x.ErrorMessage)));
            return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));
        }

        var result = await _mediator.Send(new UpdateItemOrcamentoCommand(request), cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Falha ao atualizar item de serviço do orçamento. OrcamentoId: {OrcamentoId}, ItemServicoId: {ItemServicoId}. Erro: {Erro}", request.OrcamentoId, request.ItemServicoId, result.Error);
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível atualizar o item de serviço do orçamento."));
        }

        return Ok(ApiResponse.SuccessResponse("Item de serviço do orçamento atualizado com sucesso."));
    }

    /// <summary>
    /// Atualiza um item de serviço vinculado à ordem de serviço.
    /// </summary>
    /// <param name="request">Dados atualizados do item de serviço.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Mensagem de sucesso, erro de validação ou item não encontrado.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPut("atualizar")]
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
        _logger.LogInformation("Iniciando a atualização do item de serviço. OrdemServicoId: {OrdemServicoId}, ItemServicoId: {ItemServicoId}.", request.OrdemServicoId, request.ItemServicoId);

        var validation = await _updateValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            _logger.LogWarning("Falha na validação de atualização do item de serviço. OrdemServicoId: {OrdemServicoId}, ItemServicoId: {ItemServicoId}. Erros: {Erros}", request.OrdemServicoId, request.ItemServicoId, string.Join(", ", validation.Errors.Select(x => x.ErrorMessage)));
            return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));
        }

        var result = await _mediator.Send(new UpdateItemServicoCommand(request), cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Falha ao atualizar item de serviço. OrdemServicoId: {OrdemServicoId}, ItemServicoId: {ItemServicoId}. Erro: {Erro}", request.OrdemServicoId, request.ItemServicoId, result.Error);

            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível atualizar o item de serviço."));
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
    [HttpDelete("remover")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> RemoverItemServico(
        [FromBody] DeleteItemServicoRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a remoção do item de serviço. OrdemServicoId: {OrdemServicoId}, ItemServicoId: {ItemServicoId}.", request.OrdemServicoId, request.ItemServicoId);

        var result = await _mediator.Send(new DeleteItemServicoCommand(request), cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Falha ao remover item de serviço. OrdemServicoId: {OrdemServicoId}, ItemServicoId: {ItemServicoId}. Erro: {Erro}", request.OrdemServicoId, request.ItemServicoId, result.Error);
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível remover o item de serviço."));
        }

        return Ok(ApiResponse.SuccessResponse("Item de serviço removido com sucesso."));
    }
}





