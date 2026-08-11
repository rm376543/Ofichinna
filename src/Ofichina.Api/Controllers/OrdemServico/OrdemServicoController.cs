using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ofichina.Application.UseCases.OrdensServico.Commands;
using Ofichina.Application.UseCases.OrdensServico.Queries;
using Ofichina.Contracts;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses;
using Ofichina.Contracts.Responses.OrdemServico;
using Ofichina.Contracts.Responses.OrdensServico;

namespace Ofichina.Api.Controllers.OrdensServico;

/// <summary>
/// Controller responsável pelas consultas e transições de status de ordens de serviço.
/// </summary>
[Authorize]
[ApiController]
[Route("api/ordem-servico")]
public sealed class OrdemServicoController : ControllerBase
{
    private const string StatusEmExecucao = "EmExecucao";
    private const string StatusFinalizada = "Finalizada";
    private const string StatusEntregue = "Entregue";
    private const string StatusCancelada = "Cancelada";

    private readonly IMediator _mediator;
    private readonly ILogger<OrdemServicoController> _logger;

    public OrdemServicoController(
        IMediator mediator,
        ILogger<OrdemServicoController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Retorna todas as ordens de serviço cadastradas.
    /// </summary>
    /// <param name="pagination">Parâmetros de paginação.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Lista de ordens de serviço.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpGet("listar")]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<OrdemServicoSimplesResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<PagedResponse<OrdemServicoSimplesResponse>>>> BuscarTodasOrdensServicoPaginadas(
        [FromQuery] Pagination pagination,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a obtenção de todas as ordens de serviço.");

        var result = await _mediator.Send(new GetAllOrdensServicoPaginadasQuery(pagination), cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogError("Erro ao obter as ordens de serviço: {Erro}", result.Error);
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível obter as ordens de serviço."));
        }

        return Ok(ApiResponse<PagedResponse<OrdemServicoSimplesResponse>>.SuccessResponse(result.Value));
    }

    /// <summary>
    /// Retorna uma ordem de serviço pelo identificador.
    /// </summary>
    /// <param name="ordemServicoId">Identificador da ordem de serviço.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Ordem de serviço encontrada ou erro 404.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpGet("detalhar/{ordemServicoId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<OrdemServicoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<OrdemServicoResponse>>> BuscarOrdemServicoPorId(
        [FromRoute] Guid ordemServicoId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a obtenção da ordem de serviço com Id: {Id}", ordemServicoId);

        var result = await _mediator.Send(new GetOrdemServicoByIdQuery(ordemServicoId), cancellationToken);

        if (!result.IsSuccess || result.Value is null)
        {
            _logger.LogError("Ordem de serviço com Id: {Id} não encontrada.", ordemServicoId);
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Ordem de serviço não encontrada."));
        }

        return Ok(ApiResponse<OrdemServicoResponse>.SuccessResponse(result.Value));
    }

    /// <summary>
    /// Inicia a execução da ordem de serviço.
    /// </summary>
    /// <param name="request">Identificador da ordem de serviço.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Mensagem de sucesso ou erro de status.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPost("iniciar-execucao")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public Task<ActionResult<ApiResponse>> IniciarExecucaoOrdemServico(
        [FromBody] OrdemServicoRequest request,
        CancellationToken cancellationToken)
        => AlterarStatusAsync(request, StatusEmExecucao, "Execução da ordem de serviço iniciada com sucesso.", cancellationToken);

    /// <summary>
    /// Finaliza a ordem de serviço.
    /// </summary>
    /// <param name="request">Identificador da ordem de serviço.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Mensagem de sucesso ou erro de status.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPut("finalizar")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public Task<ActionResult<ApiResponse>> FinalizarOrdemServico(
        [FromBody] OrdemServicoRequest request,
        CancellationToken cancellationToken)
        => AlterarStatusAsync(request, StatusFinalizada, "Ordem de serviço finalizada com sucesso.", cancellationToken);

    /// <summary>
    /// Marca a ordem de serviço como entregue.
    /// </summary>
    /// <param name="request">Identificador da ordem de serviço.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Mensagem de sucesso ou erro de status.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPut("entregar")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public Task<ActionResult<ApiResponse>> EntregarOrdemServico(
        [FromBody] OrdemServicoRequest request,
        CancellationToken cancellationToken)
        => AlterarStatusAsync(request, StatusEntregue, "Ordem de serviço entregue com sucesso.", cancellationToken);

    /// <summary>
    /// Cancela a ordem de serviço.
    /// </summary>
    /// <param name="request">Identificador da ordem de serviço.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Mensagem de sucesso ou erro de status.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPut("cancelar")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public Task<ActionResult<ApiResponse>> CancelarOrdemServico(
        [FromBody] OrdemServicoRequest request,
        CancellationToken cancellationToken)
        => AlterarStatusAsync(request, StatusCancelada, "Ordem de serviço cancelada com sucesso.", cancellationToken);

    private async Task<ActionResult<ApiResponse>> AlterarStatusAsync(
        OrdemServicoRequest request,
        string statusDestino,
        string mensagemSucesso,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a alteração de status da ordem de serviço com Id: {Id} para {StatusDestino}.", request.OrdemServicoId, statusDestino);

        var result = await _mediator.Send(new AlterarStatusOrdemServicoCommand(request.OrdemServicoId, statusDestino), cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogError("Erro ao alterar status da ordem de serviço com Id: {Id}. Erro: {Erro}", request.OrdemServicoId, result.Error);
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível alterar o status da ordem de serviço."));
        }

        return Ok(ApiResponse.SuccessResponse(mensagemSucesso));
    }

}



