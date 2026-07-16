using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Veiculos.Commands;
using Ofichina.Application.UseCases.Veiculos.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses;
using Ofichina.Contracts.Requests.Veiculo;
using Ofichina.Contracts.Responses.Veiculo;

namespace Ofichina.Api.Controllers.Veiculo;

/// <summary>
    /// Controller responsável pelo CRUD de veículos vinculados a pessoas.
/// </summary>
[Authorize]
[ApiController]
[Route("api/veiculos")]
public sealed class VeiculoController : ControllerBase
{
    private readonly IValidator<CreateVeiculoRequest> _createValidator;
    private readonly IValidator<UpdateVeiculoRequest> _updateValidator;
    private readonly IQueryHandler<GetVeiculosQuery, Result<IReadOnlyCollection<VeiculoResponse>>> _getAllHandler;
    private readonly IQueryHandler<GetVeiculoByIdQuery, Result<VeiculoResponse>> _getByIdHandler;
    private readonly ILogger<VeiculoController> _logger;

#pragma warning disable S107
    public VeiculoController(
        IValidator<CreateVeiculoRequest> createValidator,
        IValidator<UpdateVeiculoRequest> updateValidator,
        IQueryHandler<GetVeiculosQuery, Result<IReadOnlyCollection<VeiculoResponse>>> getAllHandler,
        IQueryHandler<GetVeiculoByIdQuery, Result<VeiculoResponse>> getByIdHandler,
        ILogger<VeiculoController> logger)
#pragma warning restore S107
    {
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _getAllHandler = getAllHandler;
        _getByIdHandler = getByIdHandler;
        _logger = logger;
    }

    /// <summary>
    /// Retorna todos os veículos cadastrados.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Lista de veículos.</returns>
    [AllowAnonymous]
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<VeiculoResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<VeiculoResponse>>>> BuscarVeiculos(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a obtenção de todos os veículos vinculados a pessoas.");

        var result = await _getAllHandler.HandleAsync(new GetVeiculosQuery());

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
    [Authorize(Policy = "usuario.ler")]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<VeiculoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<VeiculoResponse>>> BuscarVeiculoPorId(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a obtenção do veículo com Id: {Id}", id);

        var result = await _getByIdHandler.HandleAsync(new GetVeiculoByIdQuery(id));

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
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<Guid>>> CriarVeiculo(
        [FromBody] CreateVeiculoRequest request,
        [FromServices] ICommandHandler<CreateVeiculoCommand, Result<Guid>> createHandler,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a criação de um veículo. Placa: {Placa}", request.Placa);

        var validation = await _createValidator.ValidateAsync(request, cancellationToken);

        if (!validation.IsValid)
            return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));

        var result = await createHandler.HandleAsync(new CreateVeiculoCommand
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
        });

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
    [HttpPut]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> AtualizarVeiculo(
        [FromBody] UpdateVeiculoRequest request,
        [FromServices] ICommandHandler<UpdateVeiculoCommand, Result> updateHandler,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a atualização do veículo com Id: {Id}", request.Id);

        var validation = await _updateValidator.ValidateAsync(request, cancellationToken);

        if (!validation.IsValid)
            return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));

        var result = await updateHandler.HandleAsync(new UpdateVeiculoCommand
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
        });

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
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> RemoverVeiculo(
        Guid id,
        [FromServices] ICommandHandler<DeleteVeiculoCommand, Result> deleteHandler,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a remoção do veículo com Id: {Id}", id);

        var result = await deleteHandler.HandleAsync(new DeleteVeiculoCommand { Id = id });

        if (!result.IsSuccess)
            return NotFound(ApiResponse.FailureResponse(result.Error ?? "Veículo não encontrado."));

        return Ok(ApiResponse.SuccessResponse("Veículo removido com sucesso."));
    }
}