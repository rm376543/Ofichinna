using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ofichina.Application.UseCases.Veiculos.Commands;
using Ofichina.Application.UseCases.Veiculos.Queries;
using Ofichina.Contracts.Requests.Veiculo;
using Ofichina.Contracts.Responses;
using Ofichina.Contracts.Responses.Veiculo;

namespace Ofichina.Api.Controllers.Veiculo;

/// <summary>
    /// Controller responsável pelo CRUD de veículos vinculados a pessoas.
/// </summary>
[Authorize]
[ApiController]
[Route("api/veiculos")]
#pragma warning disable S6960
public sealed class VeiculoController : ControllerBase
#pragma warning restore S6960
{
    private readonly IValidator<CreateVeiculoRequest> _createValidator;
    private readonly IValidator<UpdateVeiculoRequest> _updateValidator;
    private readonly IMediator _mediator;
    private readonly ILogger<VeiculoController> _logger;

    public VeiculoController(
        IValidator<CreateVeiculoRequest> createValidator,
        IValidator<UpdateVeiculoRequest> updateValidator,
        IMediator mediator,
        ILogger<VeiculoController> logger)
    {
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Retorna todos os veículos cadastrados.
    /// </summary>
    /// <param name="clienteId">Identificador do cliente.</param>
    /// <param name="pageNumber">Número da página.</param>
    /// <param name="pageSize">Tamanho da página.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Lista de veículos.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpGet("cliente/{clienteId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<VeiculoListResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<VeiculoResponse>>>> BuscarVeiculosPorClienteId(
        Guid clienteId,
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Iniciando a obtenção de todos os veículos vinculados a uma pessoa.");

        var result = await _mediator.Send(
            new GetVeiculosByPessoaIdQuery(clienteId, pageNumber, pageSize),
            cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Falha ao obter os veículos vinculados a uma pessoa. Erro: {Error}", result.Error);
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível obter os veículos."));
        }

        _logger.LogInformation("Pesquisa de veículos do usuario {ClienteId} concluída com sucesso.", clienteId);
        return Ok(ApiResponse<PagedResponse<VeiculoListResponse>>.SuccessResponse(result.Value ?? new PagedResponse<VeiculoListResponse>()));
    }

    /// <summary>
    /// Retorna todos os veículos cadastrados.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Lista de veículos.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<VeiculoResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<VeiculoResponse>>>> BuscarVeiculos(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a obtenção de todos os veículos vinculados a pessoas.");

        var result = await _mediator.Send(new GetVeiculosQuery(), cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível obter os veículos."));

        return Ok(ApiResponse<IReadOnlyCollection<VeiculoResponse>>.SuccessResponse(result.Value ?? []));
    }

    /// <summary>
    /// Retorna um veículo pelo identificador.
    /// </summary>
    /// <param name="id">Identificador do veículo.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Veículo encontrado ou erro 404.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<VeiculoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<VeiculoResponse>>> BuscarVeiculoPorId(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a obtenção do veículo com Id: {Id}", id);

        var result = await _mediator.Send(new GetVeiculoByIdQuery(id), cancellationToken);

        if (!result.IsSuccess || result.Value is null)
            return NotFound(ApiResponse.FailureResponse(result.Error ?? "Veículo não encontrado."));

        return Ok(ApiResponse<VeiculoResponse>.SuccessResponse(result.Value));
    }

    /// <summary>
    /// Cria um novo veículo.
    /// </summary>
    /// <param name="request">Dados do veículo.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Identificador do veículo criado ou erro de validação.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<Guid>>> CriarVeiculo(
        [FromBody] CreateVeiculoRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a criação de um veículo. Placa: {Placa}", request.Placa);

        var validation = await _createValidator.ValidateAsync(request, cancellationToken);

        if (!validation.IsValid)
            return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));

        var result = await _mediator.Send(new CreateVeiculoCommand
        {
            PessoaId = request.PessoaId,
            Placa = request.Placa,
            Marca = request.Marca,
            Modelo = request.Modelo,
            AnoFabricacao = request.AnoFabricacao,
            Cor = request.Cor,
            Observacoes = request.Observacoes,
            Hodometro = request.Hodometro,
            Ativo = request.Ativo
        }, cancellationToken);

        if (!result.IsSuccess)
            return result.Error == "Pessoa não encontrada."
                ? NotFound(ApiResponse.FailureResponse(result.Error))
                : BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível criar o veículo."));

        return StatusCode(StatusCodes.Status201Created, ApiResponse<Guid>.SuccessResponse(result.Value, "Veículo criado com sucesso."));
    }

    /// <summary>
    /// Atualiza um veículo existente.
    /// </summary>
    /// <param name="request">Dados atualizados do veículo.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Mensagem de sucesso, erro de validação ou veículo não encontrado.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPut]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> AtualizarVeiculo(
        [FromBody] UpdateVeiculoRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a atualização do veículo com Id: {Id}", request.Id);

        var validation = await _updateValidator.ValidateAsync(request, cancellationToken);

        if (!validation.IsValid)
            return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));

        var result = await _mediator.Send(new UpdateVeiculoCommand
        {
            Id = request.Id,
            PessoaId = request.PessoaId,
            Placa = request.Placa,
            Marca = request.Marca,
            Modelo = request.Modelo,
            AnoFabricacao = request.AnoFabricacao,
            Cor = request.Cor,
            Observacoes = request.Observacoes,
            Hodometro = request.Hodometro,
            Ativo = request.Ativo
        }, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.Error == "Veículo não encontrado." || result.Error == "Pessoa não encontrada."
                ? NotFound(ApiResponse.FailureResponse(result.Error))
                : BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível atualizar o veículo."));
        }

        return Ok(ApiResponse.SuccessResponse("Veículo atualizado com sucesso."));
    }

    /// <summary>
    /// Remove logicamente um veículo existente.
    /// </summary>
    /// <param name="id">Identificador do veículo.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Mensagem de sucesso ou erro 404.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> RemoverVeiculo(
        Guid id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a remoção do veículo com Id: {Id}", id);

        var result = await _mediator.Send(new DeleteVeiculoCommand { Id = id }, cancellationToken);

        if (!result.IsSuccess)
            return NotFound(ApiResponse.FailureResponse(result.Error ?? "Veículo não encontrado."));

        return Ok(ApiResponse.SuccessResponse("Veículo removido com sucesso."));
    }
}


