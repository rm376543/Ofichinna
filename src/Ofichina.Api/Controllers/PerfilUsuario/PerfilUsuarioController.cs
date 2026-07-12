using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.PerfilCliente.Commands;
using Ofichina.Application.UseCases.PerfilCliente.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Enums;
using Ofichina.Contracts.Requests.PerfilCliente;
using Ofichina.Contracts.Responses;
using Ofichina.Contracts.Responses.PerfilCliente;

namespace Ofichina.Api.Controllers.PerfilCliente;

[Authorize]
[ApiController]
[Route("api/cliente/{clienteId:guid}/perfil")]
public sealed class PerfilUsuarioController : ControllerBase
{
    private readonly IValidator<VincularPerfilClienteRequest> _vincularValidator;
    private readonly ICommandHandler<VincularPerfilClienteCommand, Result<VincularPerfilClienteResponse>> _vincularHandler;
    private readonly IQueryHandler<ObterPerfisDoClienteQuery, IReadOnlyCollection<string>> _obterPerfisHandler;
    private readonly ILogger<PerfilUsuarioController> _logger;

    public PerfilUsuarioController(
        IValidator<VincularPerfilClienteRequest> vincularValidator,
        ICommandHandler<VincularPerfilClienteCommand, Result<VincularPerfilClienteResponse>> vincularHandler,
        IQueryHandler<ObterPerfisDoClienteQuery, IReadOnlyCollection<string>> obterPerfisHandler,
        ILogger<PerfilUsuarioController> logger)
    {
        _vincularValidator = vincularValidator;
        _vincularHandler = vincularHandler;
        _obterPerfisHandler = obterPerfisHandler;
        _logger = logger;
    }

    /// <summary>
    /// Vincula um perfil a um cliente existente.
    /// </summary>
    /// <param name="clienteId">Identificador do cliente.</param>
    /// <param name="perfilId">Identificador do perfil.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>
    /// Retorna sucesso quando o vínculo é criado;
    /// retorna erro quando cliente, perfil ou vínculo já existirem.
    /// </returns>
    [Authorize(Policy = UserPolicyEnum.Escrever)]
    [HttpPost("{perfilId:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse>> VincularAsync(
        Guid clienteId,
        Guid perfilId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando vinculação de perfil. ClienteId: {ClienteId}, PerfilId: {PerfilId}", clienteId, perfilId);

        var request = new VincularPerfilClienteRequest
        {
            UsuarioId = clienteId,
            PerfilId = perfilId
        };

        var validation = await _vincularValidator.ValidateAsync(request, cancellationToken);

        if (!validation.IsValid)
        {
            _logger.LogWarning("Validação falhou ao vincular perfil. ClienteId: {ClienteId}, PerfilId: {PerfilId}, Erros: {Errors}",
                clienteId, perfilId, string.Join(", ", validation.Errors.Select(x => x.ErrorMessage)));
            return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));
        }

        var result = await _vincularHandler.HandleAsync(new VincularPerfilClienteCommand(
            request.UsuarioId,
            request.PerfilId));

        if (!result.IsSuccess)
        {
            _logger.LogError("Falha ao vincular perfil. ClienteId: {ClienteId}, PerfilId: {PerfilId}, Erro: {Error}",
                clienteId, perfilId, result.Error);
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível vincular o perfil."));
        }

        _logger.LogInformation("Perfil vinculado com sucesso. ClienteId: {ClienteId}, PerfilId: {PerfilId}", clienteId, perfilId);
        return Ok(ApiResponse.SuccessResponse(result.Value?.Mensagem ?? "Perfil vinculado com sucesso."));
    }

    /// <summary>
    /// Retorna todos os perfis vinculados a um cliente.
    /// </summary>
    /// <param name="clienteId">Identificador do cliente.</param>
    /// <returns>Lista com os códigos dos perfis vinculados ao cliente.</returns>
    [Authorize(Policy = UserPolicyEnum.Ler)]
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<string>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<string>>>> ObterPerfisAsync(Guid clienteId)
    {
        _logger.LogInformation("Consultando perfis do cliente. ClienteId: {ClienteId}", clienteId);

        var perfis = await _obterPerfisHandler.HandleAsync(new ObterPerfisDoClienteQuery(clienteId));

        _logger.LogInformation("Perfis obtidos com sucesso. ClienteId: {ClienteId}, Quantidade: {Quantidade}", clienteId, perfis.Count);

        return Ok(ApiResponse<IReadOnlyCollection<string>>.SuccessResponse(perfis));
    }
}