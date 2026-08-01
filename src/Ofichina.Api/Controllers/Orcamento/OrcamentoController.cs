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
[Route("api/orcamentos")]
#pragma warning disable S6960
public sealed class OrcamentoController : ControllerBase
#pragma warning restore S6960
{
    private readonly IValidator<CreateOrcamentoRequest> _createValidator;
    private readonly IValidator<UpdateOrcamentoRequest> _updateValidator;
    private readonly IMediator _mediator;
    private readonly ILogger<OrcamentoController> _logger;

    public OrcamentoController(
        IValidator<CreateOrcamentoRequest> createValidator,
        IValidator<UpdateOrcamentoRequest> updateValidator,
        IMediator mediator,
        ILogger<OrcamentoController> logger)
    {
        _createValidator = createValidator;
        _updateValidator = updateValidator;
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
    [HttpGet]
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
    /// <param name="id">Identificador do orçamento.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Orçamento encontrado ou erro 404.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<OrcamentoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<OrcamentoResponse>>> BuscarOrcamentoPorId(
        Guid id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a obtenção do orçamento com Id: {Id}", id);

        var result = await _mediator.Send(new GetOrcamentoByIdQuery { Id = id }, cancellationToken);

        if (!result.IsSuccess || result.Value is null)
        {
            _logger.LogWarning("Orçamento com Id: {Id} não encontrado.", id);
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
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> CriarOrcamento(
        [FromBody] CreateOrcamentoRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a criação de um orçamento. PessoaId: {PessoaId}, VeiculoId: {VeiculoId}.", request.PessoaId, request.VeiculoId);

        var validation = await _createValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));

        var result = await _mediator.Send(new CreateOrcamentoCommand(request), cancellationToken);

        if (!result.IsSuccess)
            return ResponderFalha(result.Error, "Não foi possível criar o orçamento.");

        return Ok(ApiResponse.SuccessResponse("Orçamento criado com sucesso."));
    }

    /// <summary>
    /// Atualiza um orçamento existente.
    /// </summary>
    /// <param name="request">Dados do orçamento.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Mensagem de sucesso ou erro de validação.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPut]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> AtualizarOrcamento(
        [FromBody] UpdateOrcamentoRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a atualização do orçamento com Id: {Id}", request.Id);

        var validation = await _updateValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));

        var result = await _mediator.Send(new UpdateOrcamentoCommand(request), cancellationToken);

        if (!result.IsSuccess)
            return ResponderFalha(result.Error, "Não foi possível atualizar o orçamento.");

        return Ok(ApiResponse.SuccessResponse("Orçamento atualizado com sucesso."));
    }

    /// <summary>
    /// Envia o orçamento para o cliente.
    /// </summary>
    /// <param name="id">Identificador do orçamento.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Mensagem de sucesso ou erro 404.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPut("{id:guid}/enviar")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> EnviarOrcamentoParaCliente(
        Guid id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando o envio do orçamento com Id: {Id} para o cliente.", id);

        var result = await _mediator.Send(new EnviarOrcamentoParaClienteCommand { Id = id }, cancellationToken);

        if (!result.IsSuccess)
            return ResponderFalha(result.Error, "Não foi possível enviar o orçamento.");

        return Ok(ApiResponse.SuccessResponse("Orçamento enviado para o cliente com sucesso."));
    }

    /// <summary>
    /// Aprova um orçamento e gera a ordem de serviço.
    /// </summary>
    /// <param name="id">Identificador do orçamento.</param>
    /// <param name="mecanicoReparoId">Identificador do mecânico responsável pelo reparo.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Mensagem de sucesso ou erro de validação.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPut("{id:guid}/aprovar/{mecanicoReparoId:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> AprovarOrcamento(
        Guid id,
        Guid mecanicoReparoId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a aprovação do orçamento com Id: {Id}.", id);

        var result = await _mediator.Send(new AprovarOrcamentoCommand
        {
            Id = id,
            MecanicoReparoId = mecanicoReparoId
        }, cancellationToken);

        if (!result.IsSuccess)
            return ResponderFalha(result.Error, "Não foi possível aprovar o orçamento.");

        return Ok(ApiResponse.SuccessResponse("Orçamento aprovado com sucesso."));
    }

    /// <summary>
    /// Reprova um orçamento.
    /// </summary>
    /// <param name="id">Identificador do orçamento.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Mensagem de sucesso ou erro 404.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPut("{id:guid}/reprovar")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> ReprovarOrcamento(
        Guid id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a reprovação do orçamento com Id: {Id}.", id);

        var result = await _mediator.Send(new ReprovarOrcamentoCommand { Id = id }, cancellationToken);

        if (!result.IsSuccess)
            return ResponderFalha(result.Error, "Não foi possível reprovar o orçamento.");

        return Ok(ApiResponse.SuccessResponse("Orçamento reprovado com sucesso."));
    }

    private ActionResult<ApiResponse> ResponderFalha(string? erro, string mensagemPadrao)
    {
        if (!string.IsNullOrWhiteSpace(erro) && erro.Contains("não encontrado", StringComparison.OrdinalIgnoreCase))
            return NotFound(ApiResponse.FailureResponse(erro));

        return BadRequest(ApiResponse.FailureResponse(erro ?? mensagemPadrao));
    }
}
