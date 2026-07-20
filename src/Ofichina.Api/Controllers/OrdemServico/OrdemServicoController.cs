using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ofichina.Application.UseCases.OrdensServico.Commands;
using Ofichina.Application.UseCases.OrdensServico.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Enums;
using Ofichina.Contracts.Requests.OrdemServico;
using Ofichina.Contracts.Responses;
using Ofichina.Contracts.Responses.OrdemServico;

namespace Ofichina.Api.Controllers.OrdemServico;

/// <summary>
/// Controller responsável pelo CRUD de ordens de serviço e pelas transições de status.
/// </summary>
[Authorize]
[ApiController]
[Route("api/ordens-servico")]
public sealed class OrdemServicoController : ControllerBase
{
    private readonly IValidator<CreateOrdemServicoRequest> _createValidator;
    private readonly IValidator<UpdateOrdemServicoRequest> _updateValidator;
    private readonly IMediator _mediator;
    private readonly ILogger<OrdemServicoController> _logger;

    public OrdemServicoController(
        IValidator<CreateOrdemServicoRequest> createValidator,
        IValidator<UpdateOrdemServicoRequest> updateValidator,
        IMediator mediator,
        ILogger<OrdemServicoController> logger)
    {
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Retorna todas as ordens de serviço cadastradas.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <param name="getAllHandler">Handler de consulta das ordens de serviço.</param>
    /// <returns>Lista de ordens de serviço.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<OrdemServicoResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<OrdemServicoResponse>>>> BuscarOrdensServico(
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a obtenção de todas as ordens de serviço.");

        var result = await _mediator.Send(new GetOrdensServicoQuery(), cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogError("Erro ao obter as ordens de serviço: {Erro}", result.Error);
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível obter as ordens de serviço."));
        }

        return Ok(ApiResponse<IReadOnlyCollection<OrdemServicoResponse>>.SuccessResponse(result.Value ?? []));
    }

    /// <summary>
    /// Retorna uma ordem de serviço pelo identificador.
    /// </summary>
    /// <param name="id">Identificador da ordem de serviço.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <param name="getByIdHandler">Handler de consulta por identificador.</param>
    /// <returns>Ordem de serviço encontrada ou erro 404.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<OrdemServicoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<OrdemServicoResponse>>> BuscarOrdemServicoPorId(
        Guid id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a obtenção da ordem de serviço com Id: {Id}", id);

        var result = await _mediator.Send(new GetOrdemServicoByIdQuery { Id = id }, cancellationToken);

        if (!result.IsSuccess || result.Value is null)
        {
            _logger.LogError("Ordem de serviço com Id: {Id} não encontrada.", id);
            return NotFound(ApiResponse.FailureResponse(result.Error ?? "Ordem de serviço não encontrada."));
        }

        return Ok(ApiResponse<OrdemServicoResponse>.SuccessResponse(result.Value));
    }

    /// <summary>
    /// Cria uma nova ordem de serviço.
    /// </summary>
    /// <param name="request">Dados da ordem de serviço.</param>
    /// <param name="createHandler">Handler de criação da ordem de serviço.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Identificador da ordem de serviço criada ou erro de validação.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<Guid>>> CriarOrdemServico(
        [FromBody] CreateOrdemServicoRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a criação de uma nova ordem de serviço. PessoaId: {PessoaId}, VeiculoId: {VeiculoId}.", request.PessoaId, request.VeiculoId);

        var validation = await _createValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            _logger.LogError("Erro ao validar a criação da ordem de serviço. Erros: {Erros}", string.Join(", ", validation.Errors.Select(x => x.ErrorMessage)));
            return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));
        }

        var result = await _mediator.Send(new CreateOrdemServicoCommand
        {
            PessoaId = request.PessoaId,
            VeiculoId = request.VeiculoId,
            FuncionarioId = request.FuncionarioId,
            HodometroEntrada = request.HodometroEntrada,
            ProblemaRelatado = request.ProblemaRelatado,
            Observacoes = request.Observacoes,
            Servicos = request.Servicos,
            Pecas = request.Pecas
        }, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogError("Erro ao criar a ordem de serviço. Erro: {Erro}", result.Error);
            return result.Error is "Pessoa não encontrada." or "Funcionário não encontrado." or "Veículo não encontrado."
                ? NotFound(ApiResponse.FailureResponse(result.Error ?? "Não foi possível criar a ordem de serviço."))
                : BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível criar a ordem de serviço."));
        }

        return StatusCode(StatusCodes.Status201Created, ApiResponse<Guid>.SuccessResponse(result.Value, "Ordem de serviço criada com sucesso."));
    }

    /// <summary>
    /// Atualiza uma ordem de serviço existente.
    /// </summary>
    /// <param name="request">Dados atualizados da ordem de serviço.</param>
    /// <param name="updateHandler">Handler de atualização da ordem de serviço.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Mensagem de sucesso, erro de validação ou ordem de serviço não encontrada.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPut]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> AtualizarOrdemServico(
        [FromBody] UpdateOrdemServicoRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a atualização da ordem de serviço com Id: {Id}", request.Id);

        var validation = await _updateValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            _logger.LogError("Erro ao validar a atualização da ordem de serviço com Id: {Id}. Erros: {Erros}", request.Id, string.Join(", ", validation.Errors.Select(x => x.ErrorMessage)));
            return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));
        }

        var result = await _mediator.Send(new UpdateOrdemServicoCommand
        {
            Id = request.Id,
            FuncionarioId = request.FuncionarioId,
            ProblemaRelatado = request.ProblemaRelatado,
            Observacoes = request.Observacoes,
            Servicos = request.Servicos,
            Pecas = request.Pecas
        }, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogError("Erro ao atualizar a ordem de serviço com Id: {Id}. Erro: {Erro}", request.Id, result.Error);
            return result.Error == "Ordem de serviço não encontrada."
                ? NotFound(ApiResponse.FailureResponse(result.Error))
                : BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível atualizar a ordem de serviço."));
        }

        return Ok(ApiResponse.SuccessResponse("Ordem de serviço atualizada com sucesso."));
    }

    /// <summary>
    /// Remove logicamente uma ordem de serviço existente.
    /// </summary>
    /// <param name="id">Identificador da ordem de serviço.</param>
    /// <param name="deleteHandler">Handler de remoção da ordem de serviço.</param>
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
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a remoção da ordem de serviço com Id: {Id}", id);

        var result = await _mediator.Send(new DeleteOrdemServicoCommand { Id = id }, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogError("Erro ao remover a ordem de serviço com Id: {Id}. Erro: {Erro}", id, result.Error);
            return NotFound(ApiResponse.FailureResponse(result.Error ?? "Ordem de serviço não encontrada."));
        }

        return Ok(ApiResponse.SuccessResponse("Ordem de serviço removida com sucesso."));
    }

    /// <summary>
    /// Inicia o diagnóstico da ordem de serviço.
    /// </summary>
    /// <param name="id">Identificador da ordem de serviço.</param>
    /// <param name="statusHandler">Handler de alteração de status da ordem de serviço.</param>
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
        CancellationToken cancellationToken)
        => AlterarStatusAsync(id, StatusOrdemServico.EmDiagnostico, "Diagnóstico da ordem de serviço iniciado com sucesso.", cancellationToken);

    /// <summary>
    /// Solicita a aprovação da ordem de serviço.
    /// </summary>
    /// <param name="id">Identificador da ordem de serviço.</param>
    /// <param name="statusHandler">Handler de alteração de status da ordem de serviço.</param>
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
        CancellationToken cancellationToken)
        => AlterarStatusAsync(id, StatusOrdemServico.AguardandoAprovacao, "Aprovação da ordem de serviço solicitada com sucesso.", cancellationToken);

    /// <summary>
    /// Aprova a execução da ordem de serviço.
    /// </summary>
    /// <param name="id">Identificador da ordem de serviço.</param>
    /// <param name="statusHandler">Handler de alteração de status da ordem de serviço.</param>
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
        CancellationToken cancellationToken)
        => AlterarStatusAsync(id, StatusOrdemServico.EmExecucao, "Ordem de serviço aprovada com sucesso.", cancellationToken);

    /// <summary>
    /// Finaliza a ordem de serviço.
    /// </summary>
    /// <param name="id">Identificador da ordem de serviço.</param>
    /// <param name="statusHandler">Handler de alteração de status da ordem de serviço.</param>
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
        CancellationToken cancellationToken)
        => AlterarStatusAsync(id, StatusOrdemServico.Finalizada, "Ordem de serviço finalizada com sucesso.", cancellationToken);

    /// <summary>
    /// Marca a ordem de serviço como entregue.
    /// </summary>
    /// <param name="id">Identificador da ordem de serviço.</param>
    /// <param name="statusHandler">Handler de alteração de status da ordem de serviço.</param>
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
        CancellationToken cancellationToken)
        => AlterarStatusAsync(id, StatusOrdemServico.Entregue, "Ordem de serviço entregue com sucesso.", cancellationToken);

    /// <summary>
    /// Cancela a ordem de serviço.
    /// </summary>
    /// <param name="id">Identificador da ordem de serviço.</param>
    /// <param name="statusHandler">Handler de alteração de status da ordem de serviço.</param>
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
        CancellationToken cancellationToken)
        => AlterarStatusAsync(id, StatusOrdemServico.Cancelada, "Ordem de serviço cancelada com sucesso.", cancellationToken);

    private async Task<ActionResult<ApiResponse>> AlterarStatusAsync(
        Guid id,
        StatusOrdemServico statusDestino,
        string mensagemSucesso,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a alteração de status da ordem de serviço com Id: {Id} para {StatusDestino}.", id, statusDestino);

        var result = await _mediator.Send(new AlterarStatusOrdemServicoCommand
        {
            Id = id,
            StatusDestino = statusDestino
        }, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogError("Erro ao alterar status da ordem de serviço com Id: {Id}. Erro: {Erro}", id, result.Error);
            return result.Error == "Ordem de serviço não encontrada."
                ? NotFound(ApiResponse.FailureResponse(result.Error))
                : BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível alterar o status da ordem de serviço."));
        }

        return Ok(ApiResponse.SuccessResponse(mensagemSucesso));
    }
}



