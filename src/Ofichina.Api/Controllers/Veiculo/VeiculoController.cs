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
    /// Controller responsÃ¡vel pelo CRUD de veÃ­culos vinculados a pessoas.
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
    /// Retorna todos os veÃ­culos cadastrados.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Lista de veÃ­culos.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<VeiculoResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<VeiculoResponse>>>> BuscarVeiculos(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a obtenÃ§Ã£o de todos os veÃ­culos vinculados a pessoas.");

        var result = await _getAllHandler.HandleAsync(new GetVeiculosQuery(), cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "NÃ£o foi possÃ­vel obter os veÃ­culos."));

        return Ok(ApiResponse<IReadOnlyCollection<VeiculoResponse>>.SuccessResponse(result.Value ?? []));
    }

    /// <summary>
    /// Retorna um veÃ­culo pelo identificador.
    /// </summary>
    /// <param name="id">Identificador do veÃ­culo.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>VeÃ­culo encontrado ou erro 404.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<VeiculoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<VeiculoResponse>>> BuscarVeiculoPorId(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a obtenÃ§Ã£o do veÃ­culo com Id: {Id}", id);

        var result = await _getByIdHandler.HandleAsync(new GetVeiculoByIdQuery(id), cancellationToken);

        if (!result.IsSuccess || result.Value is null)
            return NotFound(ApiResponse.FailureResponse(result.Error ?? "VeÃ­culo nÃ£o encontrado."));

        return Ok(ApiResponse<VeiculoResponse>.SuccessResponse(result.Value));
    }

    /// <summary>
    /// Cria um novo veÃ­culo.
    /// </summary>
    /// <param name="request">Dados do veÃ­culo.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Identificador do veÃ­culo criado ou erro de validaÃ§Ã£o.</returns>
    [Authorize(Roles = "ADMIN")]
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
        _logger.LogInformation("Iniciando a criaÃ§Ã£o de um veÃ­culo. Placa: {Placa}", request.Placa);

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
        }, cancellationToken);

        if (!result.IsSuccess)
            return result.Error == "Pessoa nÃ£o encontrada."
                ? NotFound(ApiResponse.FailureResponse(result.Error))
                : BadRequest(ApiResponse.FailureResponse(result.Error ?? "NÃ£o foi possÃ­vel criar o veÃ­culo."));

        return StatusCode(StatusCodes.Status201Created, ApiResponse<Guid>.SuccessResponse(result.Value, "VeÃ­culo criado com sucesso."));
    }

    /// <summary>
    /// Atualiza um veÃ­culo existente.
    /// </summary>
    /// <param name="request">Dados atualizados do veÃ­culo.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Mensagem de sucesso, erro de validaÃ§Ã£o ou veÃ­culo nÃ£o encontrado.</returns>
    [Authorize(Roles = "ADMIN")]
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
        _logger.LogInformation("Iniciando a atualizaÃ§Ã£o do veÃ­culo com Id: {Id}", request.Id);

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
        }, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.Error == "VeÃ­culo nÃ£o encontrado." || result.Error == "Pessoa nÃ£o encontrada."
                ? NotFound(ApiResponse.FailureResponse(result.Error))
                : BadRequest(ApiResponse.FailureResponse(result.Error ?? "NÃ£o foi possÃ­vel atualizar o veÃ­culo."));
        }

        return Ok(ApiResponse.SuccessResponse("VeÃ­culo atualizado com sucesso."));
    }

    /// <summary>
    /// Remove logicamente um veÃ­culo existente.
    /// </summary>
    /// <param name="id">Identificador do veÃ­culo.</param>
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
        [FromServices] ICommandHandler<DeleteVeiculoCommand, Result> deleteHandler,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a remoÃ§Ã£o do veÃ­culo com Id: {Id}", id);

        var result = await deleteHandler.HandleAsync(new DeleteVeiculoCommand { Id = id }, cancellationToken);

        if (!result.IsSuccess)
            return NotFound(ApiResponse.FailureResponse(result.Error ?? "VeÃ­culo nÃ£o encontrado."));

        return Ok(ApiResponse.SuccessResponse("VeÃ­culo removido com sucesso."));
    }
}
