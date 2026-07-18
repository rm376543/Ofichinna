using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.OrdensServico.Commands;
using Ofichina.Application.UseCases.OrdensServico.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Enums;
using Ofichina.Contracts.Requests.OrdemServico;
using Ofichina.Contracts.Responses;
using Ofichina.Contracts.Responses.OrdemServico;

namespace Ofichina.Api.Controllers.OrdemServico;

/// <summary>
/// Controller responsÃ¡vel pelo CRUD de ordens de serviÃ§o e pelas transiÃ§Ãµes de status.
/// </summary>
[Authorize]
[ApiController]
[Route("api/ordens-servico")]
public sealed class OrdemServicoController : ControllerBase
{
    private readonly IValidator<CreateOrdemServicoRequest> _createValidator;
    private readonly IValidator<UpdateOrdemServicoRequest> _updateValidator;
    private readonly ILogger<OrdemServicoController> _logger;

    public OrdemServicoController(
        IValidator<CreateOrdemServicoRequest> createValidator,
        IValidator<UpdateOrdemServicoRequest> updateValidator,
        ILogger<OrdemServicoController> logger)
    {
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _logger = logger;
    }

    /// <summary>
    /// Retorna todas as ordens de serviÃ§o cadastradas.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <param name="getAllHandler">Handler de consulta das ordens de serviÃ§o.</param>
    /// <returns>Lista de ordens de serviÃ§o.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<OrdemServicoResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<OrdemServicoResponse>>>> BuscarOrdensServico(
        [FromServices] IQueryHandler<GetOrdensServicoQuery, Result<IReadOnlyCollection<OrdemServicoResponse>>> getAllHandler,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a obtenÃ§Ã£o de todas as ordens de serviÃ§o.");

        var result = await getAllHandler.HandleAsync(new GetOrdensServicoQuery());

        if (!result.IsSuccess)
        {
            _logger.LogError("Erro ao obter as ordens de serviÃ§o: {Erro}", result.Error);
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "NÃ£o foi possÃ­vel obter as ordens de serviÃ§o."));
        }

        return Ok(ApiResponse<IReadOnlyCollection<OrdemServicoResponse>>.SuccessResponse(result.Value ?? []));
    }

    /// <summary>
    /// Retorna uma ordem de serviÃ§o pelo identificador.
    /// </summary>
    /// <param name="id">Identificador da ordem de serviÃ§o.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <param name="getByIdHandler">Handler de consulta por identificador.</param>
    /// <returns>Ordem de serviÃ§o encontrada ou erro 404.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<OrdemServicoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<OrdemServicoResponse>>> BuscarOrdemServicoPorId(
        Guid id,
        [FromServices] IQueryHandler<GetOrdemServicoByIdQuery, Result<OrdemServicoResponse>> getByIdHandler,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a obtenÃ§Ã£o da ordem de serviÃ§o com Id: {Id}", id);

        var result = await getByIdHandler.HandleAsync(new GetOrdemServicoByIdQuery { Id = id });

        if (!result.IsSuccess || result.Value is null)
        {
            _logger.LogError("Ordem de serviÃ§o com Id: {Id} nÃ£o encontrada.", id);
            return NotFound(ApiResponse.FailureResponse(result.Error ?? "Ordem de serviÃ§o nÃ£o encontrada."));
        }

        return Ok(ApiResponse<OrdemServicoResponse>.SuccessResponse(result.Value));
    }

    /// <summary>
    /// Cria uma nova ordem de serviÃ§o.
    /// </summary>
    /// <param name="request">Dados da ordem de serviÃ§o.</param>
    /// <param name="createHandler">Handler de criaÃ§Ã£o da ordem de serviÃ§o.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Identificador da ordem de serviÃ§o criada ou erro de validaÃ§Ã£o.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<Guid>>> CriarOrdemServico(
        [FromBody] CreateOrdemServicoRequest request,
        [FromServices] ICommandHandler<CreateOrdemServicoCommand, Result<Guid>> createHandler,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a criaÃ§Ã£o de uma nova ordem de serviÃ§o. PessoaId: {PessoaId}, VeiculoId: {VeiculoId}.", request.PessoaId, request.VeiculoId);

        var validation = await _createValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            _logger.LogError("Erro ao validar a criaÃ§Ã£o da ordem de serviÃ§o. Erros: {Erros}", string.Join(", ", validation.Errors.Select(x => x.ErrorMessage)));
            return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));
        }

        var result = await createHandler.HandleAsync(new CreateOrdemServicoCommand
        {
            PessoaId = request.PessoaId,
            VeiculoId = request.VeiculoId,
            FuncionarioId = request.FuncionarioId,
            HodometroEntrada = request.HodometroEntrada,
            ProblemaRelatado = request.ProblemaRelatado,
            Observacoes = request.Observacoes,
            Servicos = request.Servicos,
            Pecas = request.Pecas
        });

        if (!result.IsSuccess)
        {
            _logger.LogError("Erro ao criar a ordem de serviÃ§o. Erro: {Erro}", result.Error);
            return result.Error is "Pessoa nÃ£o encontrada." or "FuncionÃ¡rio nÃ£o encontrado." or "VeÃ­culo nÃ£o encontrado."
                ? NotFound(ApiResponse.FailureResponse(result.Error ?? "NÃ£o foi possÃ­vel criar a ordem de serviÃ§o."))
                : BadRequest(ApiResponse.FailureResponse(result.Error ?? "NÃ£o foi possÃ­vel criar a ordem de serviÃ§o."));
        }

        return StatusCode(StatusCodes.Status201Created, ApiResponse<Guid>.SuccessResponse(result.Value, "Ordem de serviÃ§o criada com sucesso."));
    }

    /// <summary>
    /// Atualiza uma ordem de serviÃ§o existente.
    /// </summary>
    /// <param name="request">Dados atualizados da ordem de serviÃ§o.</param>
    /// <param name="updateHandler">Handler de atualizaÃ§Ã£o da ordem de serviÃ§o.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Mensagem de sucesso, erro de validaÃ§Ã£o ou ordem de serviÃ§o nÃ£o encontrada.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPut]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> AtualizarOrdemServico(
        [FromBody] UpdateOrdemServicoRequest request,
        [FromServices] ICommandHandler<UpdateOrdemServicoCommand, Result> updateHandler,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a atualizaÃ§Ã£o da ordem de serviÃ§o com Id: {Id}", request.Id);

        var validation = await _updateValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            _logger.LogError("Erro ao validar a atualizaÃ§Ã£o da ordem de serviÃ§o com Id: {Id}. Erros: {Erros}", request.Id, string.Join(", ", validation.Errors.Select(x => x.ErrorMessage)));
            return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));
        }

        var result = await updateHandler.HandleAsync(new UpdateOrdemServicoCommand
        {
            Id = request.Id,
            FuncionarioId = request.FuncionarioId,
            ProblemaRelatado = request.ProblemaRelatado,
            Observacoes = request.Observacoes,
            Servicos = request.Servicos,
            Pecas = request.Pecas
        });

        if (!result.IsSuccess)
        {
            _logger.LogError("Erro ao atualizar a ordem de serviÃ§o com Id: {Id}. Erro: {Erro}", request.Id, result.Error);
            return result.Error == "Ordem de serviÃ§o nÃ£o encontrada."
                ? NotFound(ApiResponse.FailureResponse(result.Error))
                : BadRequest(ApiResponse.FailureResponse(result.Error ?? "NÃ£o foi possÃ­vel atualizar a ordem de serviÃ§o."));
        }

        return Ok(ApiResponse.SuccessResponse("Ordem de serviÃ§o atualizada com sucesso."));
    }

    /// <summary>
    /// Remove logicamente uma ordem de serviÃ§o existente.
    /// </summary>
    /// <param name="id">Identificador da ordem de serviÃ§o.</param>
    /// <param name="deleteHandler">Handler de remoÃ§Ã£o da ordem de serviÃ§o.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Mensagem de sucesso ou erro 404.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> RemoverOrdemServico(
        Guid id,
        [FromServices] ICommandHandler<DeleteOrdemServicoCommand, Result> deleteHandler,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a remoÃ§Ã£o da ordem de serviÃ§o com Id: {Id}", id);

        var result = await deleteHandler.HandleAsync(new DeleteOrdemServicoCommand { Id = id });

        if (!result.IsSuccess)
        {
            _logger.LogError("Erro ao remover a ordem de serviÃ§o com Id: {Id}. Erro: {Erro}", id, result.Error);
            return NotFound(ApiResponse.FailureResponse(result.Error ?? "Ordem de serviÃ§o nÃ£o encontrada."));
        }

        return Ok(ApiResponse.SuccessResponse("Ordem de serviÃ§o removida com sucesso."));
    }

    /// <summary>
    /// Inicia o diagnÃ³stico da ordem de serviÃ§o.
    /// </summary>
    /// <param name="id">Identificador da ordem de serviÃ§o.</param>
    /// <param name="statusHandler">Handler de alteraÃ§Ã£o de status da ordem de serviÃ§o.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Mensagem de sucesso ou erro de status.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPut("{id:guid}/diagnostico")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public Task<ActionResult<ApiResponse>> IniciarDiagnostico(
        Guid id,
        [FromServices] ICommandHandler<AlterarStatusOrdemServicoCommand, Result> statusHandler,
        CancellationToken cancellationToken)
        => AlterarStatusAsync(
            id,
            StatusOrdemServico.EmDiagnostico,
            "DiagnÃ³stico da ordem de serviÃ§o iniciado com sucesso.",
            statusHandler,
            cancellationToken);

    /// <summary>
    /// Solicita a aprovaÃ§Ã£o da ordem de serviÃ§o.
    /// </summary>
    /// <param name="id">Identificador da ordem de serviÃ§o.</param>
    /// <param name="statusHandler">Handler de alteraÃ§Ã£o de status da ordem de serviÃ§o.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Mensagem de sucesso ou erro de status.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPut("{id:guid}/aprovacao")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public Task<ActionResult<ApiResponse>> SolicitarAprovacao(
        Guid id,
        [FromServices] ICommandHandler<AlterarStatusOrdemServicoCommand, Result> statusHandler,
        CancellationToken cancellationToken)
        => AlterarStatusAsync(
            id,
            StatusOrdemServico.AguardandoAprovacao,
            "AprovaÃ§Ã£o da ordem de serviÃ§o solicitada com sucesso.",
            statusHandler,
            cancellationToken);

    /// <summary>
    /// Aprova a execuÃ§Ã£o da ordem de serviÃ§o.
    /// </summary>
    /// <param name="id">Identificador da ordem de serviÃ§o.</param>
    /// <param name="statusHandler">Handler de alteraÃ§Ã£o de status da ordem de serviÃ§o.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Mensagem de sucesso ou erro de status.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPut("{id:guid}/aprovar")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public Task<ActionResult<ApiResponse>> AprovarOrdemServico(
        Guid id,
        [FromServices] ICommandHandler<AlterarStatusOrdemServicoCommand, Result> statusHandler,
        CancellationToken cancellationToken)
        => AlterarStatusAsync(
            id,
            StatusOrdemServico.EmExecucao,
            "Ordem de serviÃ§o aprovada com sucesso.",
            statusHandler,
            cancellationToken);

    /// <summary>
    /// Finaliza a ordem de serviÃ§o.
    /// </summary>
    /// <param name="id">Identificador da ordem de serviÃ§o.</param>
    /// <param name="statusHandler">Handler de alteraÃ§Ã£o de status da ordem de serviÃ§o.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Mensagem de sucesso ou erro de status.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPut("{id:guid}/finalizar")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public Task<ActionResult<ApiResponse>> FinalizarOrdemServico(
        Guid id,
        [FromServices] ICommandHandler<AlterarStatusOrdemServicoCommand, Result> statusHandler,
        CancellationToken cancellationToken)
        => AlterarStatusAsync(
            id,
            StatusOrdemServico.Finalizada,
            "Ordem de serviÃ§o finalizada com sucesso.",
            statusHandler,
            cancellationToken);

    /// <summary>
    /// Marca a ordem de serviÃ§o como entregue.
    /// </summary>
    /// <param name="id">Identificador da ordem de serviÃ§o.</param>
    /// <param name="statusHandler">Handler de alteraÃ§Ã£o de status da ordem de serviÃ§o.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Mensagem de sucesso ou erro de status.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPut("{id:guid}/entregar")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public Task<ActionResult<ApiResponse>> EntregarOrdemServico(
        Guid id,
        [FromServices] ICommandHandler<AlterarStatusOrdemServicoCommand, Result> statusHandler,
        CancellationToken cancellationToken)
        => AlterarStatusAsync(
            id,
            StatusOrdemServico.Entregue,
            "Ordem de serviÃ§o entregue com sucesso.",
            statusHandler,
            cancellationToken);

    /// <summary>
    /// Cancela a ordem de serviÃ§o.
    /// </summary>
    /// <param name="id">Identificador da ordem de serviÃ§o.</param>
    /// <param name="statusHandler">Handler de alteraÃ§Ã£o de status da ordem de serviÃ§o.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Mensagem de sucesso ou erro de status.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPut("{id:guid}/cancelar")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public Task<ActionResult<ApiResponse>> CancelarOrdemServico(
        Guid id,
        [FromServices] ICommandHandler<AlterarStatusOrdemServicoCommand, Result> statusHandler,
        CancellationToken cancellationToken)
        => AlterarStatusAsync(
            id,
            StatusOrdemServico.Cancelada,
            "Ordem de serviÃ§o cancelada com sucesso.",
            statusHandler,
            cancellationToken);

    private async Task<ActionResult<ApiResponse>> AlterarStatusAsync(
        Guid id,
        StatusOrdemServico statusDestino,
        string mensagemSucesso,
        ICommandHandler<AlterarStatusOrdemServicoCommand, Result> statusHandler,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a alteraÃ§Ã£o de status da ordem de serviÃ§o com Id: {Id} para {StatusDestino}.", id, statusDestino);

        var result = await statusHandler.HandleAsync(new AlterarStatusOrdemServicoCommand
        {
            Id = id,
            StatusDestino = statusDestino
        });

        if (!result.IsSuccess)
        {
            _logger.LogError("Erro ao alterar status da ordem de serviÃ§o com Id: {Id}. Erro: {Erro}", id, result.Error);
            return result.Error == "Ordem de serviÃ§o nÃ£o encontrada."
                ? NotFound(ApiResponse.FailureResponse(result.Error))
                : BadRequest(ApiResponse.FailureResponse(result.Error ?? "NÃ£o foi possÃ­vel alterar o status da ordem de serviÃ§o."));
        }

        return Ok(ApiResponse.SuccessResponse(mensagemSucesso));
    }
}

