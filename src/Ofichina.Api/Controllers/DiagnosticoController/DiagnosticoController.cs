using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ofichina.Application.UseCases.Orcamentos.Commands;
using Ofichina.Application.UseCases.OrdensServico.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.Orcamento;
using Ofichina.Contracts.Requests.Pecas;

namespace Ofichina.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/diagnostico-orcamento")]
public sealed class DiagnosticoController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<DiagnosticoController> _logger;

    public DiagnosticoController(
        IMediator mediator,
        ILogger<DiagnosticoController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Inicia o diagnóstico de um orçamento.
    /// </summary>
    /// <param name="request">Identificador do orçamento.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Mensagem de sucesso ou erro de validação.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPost("iniciar")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> IniciarDiagnosticoOrcamento(
        [FromBody] OrcamentoRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando o diagnóstico do orçamento com Id: {Id}.", request.OrcamentoId);

        var result = await _mediator.Send(new IniciarDiagnosticoOrcamentoCommand(request.OrcamentoId), cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível iniciar o diagnóstico do orçamento."));

        return Ok(ApiResponse.SuccessResponse("Diagnóstico do orçamento iniciado com sucesso."));
    }

    /// <summary>  
    /// Sinaliza que uma peça vinculada a um item foi utilizada, dando baixa no estoque.  
    /// </summary>  
    /// <param name="request">Ordem de serviço, item de serviço e peça.</param>  
    /// <param name="cancellationToken">Token de cancelamento.</param>  
    /// <returns>Mensagem de sucesso ou erro.</returns>  
    [Authorize(Roles = "ADMIN")]
    [HttpPost("utilizar-peca")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> UtilizarPeca(
        [FromBody] UtilizarPecaRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Iniciando utilização de peça. OrdemServicoId: {OrdemServicoId}, ItemServicoId: {ItemServicoId}, PecaId: {PecaId}.",
            request.OrdemServicoId, request.ItemServicoId, request.PecaId);

        var result = await _mediator.Send(new UtilizarPecaCommand(request), cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogError(
                "Erro ao utilizar peça. OrdemServicoId: {OrdemServicoId}, PecaId: {PecaId}. Erro: {Erro}",
                request.OrdemServicoId, request.PecaId, result.Error);
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível utilizar a peça."));
        }

        return Ok(ApiResponse.SuccessResponse("Peça utilizada com sucesso."));
    }

    /// <summary>
    /// Finaliza o diagnóstico do orçamento.
    /// </summary>
    /// <param name="request">Identificador do orçamento.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Mensagem de sucesso ou erro de validação.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPost("finalizar")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> FinalizarOrcamento(
        [FromBody] OrcamentoRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Finalizando o orçamento com Id: {Id}.", request.OrcamentoId);

        var result = await _mediator.Send(new FinalizarOrcamentoCommand(request.OrcamentoId), cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível finalizar o orçamento."));

        return Ok(ApiResponse.SuccessResponse("Orçamento finalizado com sucesso."));
    }
}

