using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ofichina.Application.UseCases.Orcamentos.Commands;
using Ofichina.Application.UseCases.Orcamentos.Queries;
using Ofichina.Contracts;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.Orcamento;
using Ofichina.Contracts.Responses;
using Ofichina.Contracts.Responses.Orcamento;

namespace Ofichina.Api.Controllers.Orcamento;

/// <summary>
/// Controller responsável pelos orçamentos e suas transições de status.
/// </summary>
[Authorize]
[ApiController]
[Route("api/orcamento")]
#pragma warning disable S6960
public sealed class OrcamentoController : ControllerBase
#pragma warning restore S6960
{
    private readonly IValidator<CreateOrcamentoRequest> _createValidator;
    private readonly IValidator<UpdateOrcamentoRequest> _updateValidator;
    private readonly IValidator<UpdateOrcamentoDescontoRequest> _updateDescontoValidator;
    private readonly IMediator _mediator;
    private readonly ILogger<OrcamentoController> _logger;

    public OrcamentoController(
        IValidator<CreateOrcamentoRequest> createValidator,
        IValidator<UpdateOrcamentoRequest> updateValidator,
        IValidator<UpdateOrcamentoDescontoRequest> updateDescontoValidator,
        IMediator mediator,
        ILogger<OrcamentoController> logger)
    {
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _updateDescontoValidator = updateDescontoValidator;
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Retorna todos os orçamentos cadastrados.
    /// </summary>
    /// <param name="pagination">Parâmetros de paginação.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Lista paginada de orçamentos.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpGet("listar")]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<OrcamentoSimplesResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<PagedResponse<OrcamentoSimplesResponse>>>> BuscarTodosOrcamentosPaginados(
        [FromQuery] Pagination pagination,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a obtenção de todos os orçamentos.");

        var result = await _mediator.Send(new GetAllOrcamentosPaginadosQuery(pagination), cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogError("Erro ao obter os orçamentos: {Erro}", result.Error);
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível obter os orçamentos."));
        }

        return Ok(ApiResponse<PagedResponse<OrcamentoSimplesResponse>>.SuccessResponse(result.Value));
    }

    /// <summary>
    /// Retorna um orçamento pelo identificador.
    /// </summary>
    /// <param name="orcamentoId">Identificador do orçamento.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Orçamento encontrado ou erro 404.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpGet("detalhar/{orcamentoId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<OrcamentoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<OrcamentoResponse>>> BuscarOrcamentoPorId(
        [FromRoute] Guid orcamentoId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a obtenção do orçamento com Id: {Id}", orcamentoId);

        var result = await _mediator.Send(new GetOrcamentoByIdQuery(orcamentoId), cancellationToken);

        if (!result.IsSuccess || result.Value is null)
        {
            _logger.LogWarning("Orçamento com Id: {Id} não encontrado.", orcamentoId);
            return NotFound(ApiResponse.FailureResponse(result.Error ?? "Orçamento não encontrado."));
        }

        return Ok(ApiResponse<OrcamentoResponse>.SuccessResponse(result.Value));
    }

    /// <summary>
    /// Cria um novo orçamento.
    /// </summary>
    /// <param name="request">Dados do orçamento.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Mensagem de sucesso ou erro de validação.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPost("adicionar")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> CriarOrcamento(
        [FromBody] CreateOrcamentoRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a criação de um orçamento. PessoaId: {PessoaId}, VeiculoId: {VeiculoId}, AgendamentoId: {AgendamentoId}.", request.PessoaId, request.VeiculoId, request.AgendamentoId);

        var validation = await _createValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));

        var result = await _mediator.Send(new CreateOrcamentoCommand(request), cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível criar o orçamento."));

        return Ok(ApiResponse.SuccessResponse("Orçamento criado com sucesso."));
    }

    /// <summary>
    /// Inicia o diagnóstico de um orçamento.
    /// </summary>
    /// <param name="request">Identificador do orçamento.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Mensagem de sucesso ou erro de validação.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPost("iniciar-diagnostico")]
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
    /// Finaliza o orçamento após diagnóstico.
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

    /// <summary>
    /// Atualiza um orçamento existente.
    /// </summary>
    /// <param name="request">Dados do orçamento.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Mensagem de sucesso ou erro de validação.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPut("atualizar")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> AtualizarOrcamento(
        [FromBody] UpdateOrcamentoRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a atualização do orçamento com Id: {Id}", request.OrcamentoId);

        var validation = await _updateValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));

        var result = await _mediator.Send(new UpdateOrcamentoCommand(request), cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível atualizar o orçamento."));

        return Ok(ApiResponse.SuccessResponse("Orçamento atualizado com sucesso."));
    }

    /// <summary>
    /// Atualiza o desconto de um orçamento.
    /// </summary>
    /// <param name="orcamentoId">Identificador do orçamento.</param>
    /// <param name="request">Dados do desconto.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Mensagem de sucesso ou erro de validação.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPut("{orcamentoId:guid}/desconto")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> AtualizarDescontoOrcamento(
        [FromRoute] Guid orcamentoId,
        [FromBody] UpdateOrcamentoDescontoRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a atualização do desconto do orçamento com Id: {Id}.", orcamentoId);

        var validation = await _updateDescontoValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));

        var result = await _mediator.Send(new UpdateOrcamentoDescontoCommand(orcamentoId, request.Desconto), cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível atualizar o desconto do orçamento."));

        return Ok(ApiResponse.SuccessResponse("Desconto do orçamento atualizado com sucesso."));
    }

    /// <summary>
    /// Envia o orçamento para o cliente.
    /// </summary>
    /// <param name="request">Identificador do orçamento.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Mensagem de sucesso ou erro 404.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPost("enviar")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> EnviarOrcamentoParaCliente(
        [FromBody] OrcamentoRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando o envio do orçamento com Id: {Id} para o cliente.", request.OrcamentoId);

        var result = await _mediator.Send(new EnviarOrcamentoParaClienteCommand(request.OrcamentoId), cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível enviar o orçamento."));

        return Ok(ApiResponse.SuccessResponse("Orçamento enviado para o cliente com sucesso."));
    }

    /// <summary>
    /// Aprova um orçamento e gera a ordem de serviço.
    /// </summary>
    /// <param name="request">Objeto contendo o identificador do orçamento.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Mensagem de sucesso ou erro de validação.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPost("aprovar")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> AprovarOrcamento(
        [FromBody] OrcamentoRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a aprovação do orçamento com Id: {Id}.", request.OrcamentoId);

        var result = await _mediator.Send(new AprovarOrcamentoCommand(request.OrcamentoId), cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível aprovar o orçamento."));

        return Ok(ApiResponse.SuccessResponse("Orçamento aprovado com sucesso."));
    }

    /// <summary>
    /// Reprova um orçamento.
    /// </summary>
    /// <param name="request">Objeto contendo o identificador do orçamento e o motivo da reprovação.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Mensagem de sucesso ou erro 404.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPost("reprovar")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> ReprovarOrcamento(
        [FromBody] ReprovarOrcamentoRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a reprovação do orçamento com Id: {Id}.", request.OrcamentoId);

        var result = await _mediator.Send(new ReprovarOrcamentoCommand(request), cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível reprovar o orçamento."));

        return Ok(ApiResponse.SuccessResponse("Orçamento reprovado com sucesso."));
    }

    /// <summary>
    /// Reenvia um orçamento após reprovação.
    /// </summary>
    /// <param name="request">Objeto contendo o identificador do orçamento.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Mensagem de sucesso ou erro 404.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPost("reenviar-para-diagnostico")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> ReenviarOrcamentoAposReprovacao(
        [FromBody] OrcamentoRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando o reenvio do orçamento com Id: {Id}.", request.OrcamentoId);

        var result = await _mediator.Send(new ReenviarOrcamentoAposReprovacaoCommand(request.OrcamentoId), cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível reenviar o orçamento."));

        return Ok(ApiResponse.SuccessResponse("Orçamento reenviado com sucesso."));
    }
}
