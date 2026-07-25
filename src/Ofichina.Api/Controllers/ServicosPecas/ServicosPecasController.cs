using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ofichina.Application.UseCases.ServicosPecas.Commands;
using Ofichina.Contracts.Requests.Servicos;
using Ofichina.Contracts.Responses;

namespace Ofichina.Api.Controllers.ServicosPecas;

/// <summary>
/// Controller responsável pelo vínculo de peças com serviços.
/// </summary>
[Authorize]
[ApiController]
[Route("api/servicos-pecas")]
public sealed class ServicosPecasController : ControllerBase
{
    private readonly IValidator<CreateServicoPecaRequest> _createValidator;
    private readonly IMediator _mediator;
    private readonly ILogger<ServicosPecasController> _logger;

    public ServicosPecasController(
        IValidator<CreateServicoPecaRequest> createValidator,
        IMediator mediator,
        ILogger<ServicosPecasController> logger)
    {
        _createValidator = createValidator;
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Adiciona uma peça a um serviço.
    /// </summary>
    /// <param name="request">Dados da peça a ser adicionada ao serviço.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Identificador da peça adicionada ao serviço.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse>> AdicionarPecaAoServico(
        [FromBody] CreateServicoPecaRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a inclusão de peça no serviço. ServicoId: {ServicoId}, PecaId: {PecaId}, Quantidade: {Quantidade}.", request.ServicoId, request.PecaId, request.Quantidade);

        var validation = await _createValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            _logger.LogWarning("Validação falhou para a inclusão de peça no serviço. Erros: {Erros}", string.Join(", ", validation.Errors.Select(x => x.ErrorMessage)));
            return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));
        }

        var result = await _mediator.Send(new CreateServicoPecaCommand
        {
            ServicoId = request.ServicoId,
            PecaId = request.PecaId,
            Quantidade = request.Quantidade
        }, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Falha ao adicionar peça ao serviço. ServicoId: {ServicoId}, PecaId: {PecaId}. Erro: {Erro}", request.ServicoId, request.PecaId, result.Error);
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Ocorreu um erro ao tentar adicionar a peça"));
        }

        return Ok(ApiResponse.SuccessResponse("Peça adicionada ao serviço com sucesso."));
    }

    /// <summary>
    /// Desativa uma peça específica do serviço.
    /// </summary>
    /// <param name="servicoId">Identificador do serviço.</param>
    /// <param name="pecaId">Identificador da peça do serviço.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    [Authorize(Roles = "ADMIN")]
    [HttpDelete("{servicoId:guid}/pecas/{pecaId:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse>> DesativarPeca(
        [FromRoute] Guid servicoId,
        [FromRoute] Guid pecaId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a desativação da peça do serviço. ServicoId: {ServicoId}, PecaId: {PecaId}.", servicoId, pecaId);

        var result = await _mediator.Send(new DeleteServicoPecaCommand
        {
            ServicoId = servicoId,
            PecaId = pecaId

        }, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Falha ao desativar peça do serviço. ServicoId: {ServicoId}, PecaId: {PecaId}. Erro: {Erro}", servicoId, pecaId, result.Error);
            return BadRequest(ApiResponse.FailureResponse("Não foi possível desativar a peça do serviço."));
        }

        _logger.LogInformation("Peça do serviço desativada com sucesso");
        return Ok(ApiResponse.SuccessResponse("Peça do serviço desativada com sucesso."));
    }

    /// <summary>
    /// Desativa todas as peças vinculadas ao serviço.
    /// </summary>
    /// <param name="servicoId">Identificador do serviço.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    [Authorize(Roles = "ADMIN")]
    [HttpDelete("{servicoId:guid}/pecas")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse>> DesativarTodasAsPecas(
        [FromRoute] Guid servicoId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a desativação de todas as peças do serviço. ServicoId: {ServicoId}.", servicoId);

        var result = await _mediator.Send(new DeleteAllServicoPecasCommand
        {
            ServicoId = servicoId
        }, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Falha ao desativar peças do serviço. ServicoId: {ServicoId}. Erro: {Erro}", servicoId, result.Error);
            return BadRequest(ApiResponse.FailureResponse("Não foi possível desativar as peças do serviço."));
        }

        return Ok(ApiResponse.SuccessResponse("Peças do serviço desativadas com sucesso."));
    }
}