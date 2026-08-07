using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ofichina.Application.UseCases.Veiculos.Commands;
using Ofichina.Application.UseCases.Veiculos.Queries;
using Ofichina.Contracts;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.Veiculo;
using Ofichina.Contracts.Responses;
using Ofichina.Contracts.Responses.Pessoa;
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
    /// <param name="pessoaId">Identificador do cliente.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Dados da pessoa com todos os veículos vinculados.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpGet("pessoa/{pessoaId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PessoaVeiculoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<PessoaVeiculoResponse>>> BuscarVeiculosPorPessoaId(
        Guid pessoaId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Iniciando a obtenção de todos os veículos vinculados a uma pessoa.");

        var result = await _mediator.Send(new GetVeiculosByPessoaIdQuery(pessoaId), cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Falha ao obter os veículos vinculados a uma pessoa. Erro: {Error}", result.Error);
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível obter os veículos."));
        }

        _logger.LogInformation("Pesquisa de veículos da pessoa {PessoaId} concluída com sucesso.", pessoaId);

        return Ok(ApiResponse<PessoaVeiculoResponse>.SuccessResponse(result.Value));
    }

    /// <summary>
    /// Retorna todos os veículos cadastrados.
    /// </summary>
    /// <param name="pagination" >Parâmetros de paginação.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Lista de veículos.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpGet("listar")]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<VeiculoResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<PagedResponse<VeiculoResponse>>>> BuscarTodosVeiculosPaginado([FromQuery] Pagination pagination, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a obtenção de todos os veículos vinculados a pessoas.");

        var result = await _mediator.Send(new GetAllVeiculosPaginadosQuery(pagination), cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Falha ao obter os veículos vinculados a pessoas. Erro: {Error}", result.Error);
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível obter os veículos."));
        }

        _logger.LogInformation("Pesquisa de veículos concluída com sucesso.");
        return Ok(ApiResponse<PagedResponse<VeiculoResponse>>.SuccessResponse(result.Value));
    }

    /// <summary>
    /// Retorna um veículo pelo identificador.
    /// </summary>
    /// <param name="veiculoId">Identificador do veículo.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Veículo encontrado ou erro 404.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpGet("detalhar/{veiculoId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<VeiculoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<VeiculoResponse>>> BuscarVeiculoPorId([FromRoute] Guid veiculoId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a obtenção do veículo com Id: {Id}", veiculoId);

        var result = await _mediator.Send(new GetVeiculoByIdQuery(veiculoId), cancellationToken);

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
    [HttpPost("novo")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> CriarVeiculo(
        [FromBody] CreateVeiculoRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a criação de um veículo. Placa: {Placa}", request.Placa);

        var validation = await _createValidator.ValidateAsync(request, cancellationToken);

        if (!validation.IsValid)
        {
            _logger.LogWarning("Falha na validação do veículo. Erros: {Errors}", string.Join(", ", validation.Errors.Select(x => x.ErrorMessage)));
            return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));
        }

        var result = await _mediator.Send(new CreateVeiculoCommand(request), cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Falha ao criar o veículo. Erro: {Error}", result.Error);
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível criar o veículo."));
        }

        _logger.LogInformation("Veículo criado com sucesso.");
        return Ok(ApiResponse.SuccessResponse("Veículo cadastrado com sucesso."));
    }

    /// <summary>
    /// Atualiza um veículo existente.
    /// </summary>
    /// <param name="request">Dados atualizados do veículo.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Mensagem de sucesso, erro de validação ou veículo não encontrado.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPut("atualizar")]
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

        var result = await _mediator.Send(new UpdateVeiculoCommand(request), cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Falha ao atualizar o veículo. Erro: {Error}", result.Error);
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível atualizar o veículo."));
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
    [HttpDelete("remove")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> RemoverVeiculo(
        [FromBody] RemoveVeiculoRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a remoção do veículo com Id: {Id}", request.Id);

        var result = await _mediator.Send(new DeleteVeiculoCommand(request), cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Falha ao remover o veículo. Erro: {Error}", result.Error);
            return NotFound(ApiResponse.FailureResponse(result.Error ?? "Veículo não encontrado."));
        }

        return Ok(ApiResponse.SuccessResponse("Veículo removido com sucesso."));
    }
}


