using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.PerfilUsuario.Commands;
using Ofichina.Application.UseCases.PerfilUsuario.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.PerfilUsuario;
using Ofichina.Contracts.Responses;
using Ofichina.Contracts.Responses.PerfilUsuario;

namespace Ofichina.Api.Controllers.PerfilUsuario;

[Authorize]
[ApiController]
[Route("api/usuario/{usuarioId:guid}/perfil")]
public sealed class PerfilUsuarioController : ControllerBase
{
    private readonly IValidator<VincularPerfilUsuarioRequest> _vincularValidator;
    private readonly ICommandHandler<VincularPerfilUsuarioCommand, Result<VincularPerfilUsuarioResponse>> _vincularHandler;
    private readonly IQueryHandler<ObterPerfisDoUsuarioQuery, IReadOnlyCollection<string>> _obterPerfisHandler;
    private readonly ILogger<PerfilUsuarioController> _logger;

    public PerfilUsuarioController(
        IValidator<VincularPerfilUsuarioRequest> vincularValidator,
        ICommandHandler<VincularPerfilUsuarioCommand, Result<VincularPerfilUsuarioResponse>> vincularHandler,
        IQueryHandler<ObterPerfisDoUsuarioQuery, IReadOnlyCollection<string>> obterPerfisHandler,
        ILogger<PerfilUsuarioController> logger)
    {
        _vincularValidator = vincularValidator;
        _vincularHandler = vincularHandler;
        _obterPerfisHandler = obterPerfisHandler;
        _logger = logger;
    }

    /// <summary>
    /// Vincula um perfil a um usuário existente.
    /// </summary>
    /// <param name="usuarioId">Identificador do usuário.</param>
    /// <param name="perfilId">Identificador do perfil.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>
    /// Retorna sucesso quando o vínculo é criado;
    /// retorna erro quando usuário, perfil ou vínculo já existirem.
    /// </returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPost("{perfilId:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse>> VincularAsync(
        Guid usuarioId,
        Guid perfilId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando vinculação de perfil. UsuarioId: {UsuarioId}, PerfilId: {PerfilId}", usuarioId, perfilId);

        var request = new VincularPerfilUsuarioRequest
        {
            UsuarioId = usuarioId,
            PerfilId = perfilId
        };

        var validation = await _vincularValidator.ValidateAsync(request, cancellationToken);

        if (!validation.IsValid)
        {
            _logger.LogWarning("Validação falhou ao vincular perfil. UsuarioId: {UsuarioId}, PerfilId: {PerfilId}, Erros: {Errors}",
                usuarioId, perfilId, string.Join(", ", validation.Errors.Select(x => x.ErrorMessage)));
            return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));
        }

        var result = await _vincularHandler.HandleAsync(new VincularPerfilUsuarioCommand(
            request.UsuarioId,
            request.PerfilId), cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogError("Falha ao vincular perfil. UsuarioId: {UsuarioId}, PerfilId: {PerfilId}, Erro: {Error}",
                usuarioId, perfilId, result.Error);
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível vincular o perfil."));
        }

        _logger.LogInformation("Perfil vinculado com sucesso. UsuarioId: {UsuarioId}, PerfilId: {PerfilId}", usuarioId, perfilId);
        return Ok(ApiResponse.SuccessResponse(result.Value?.Mensagem ?? "Perfil vinculado com sucesso."));
    }

    /// <summary>
    /// Retorna todos os perfis vinculados a um usuário.
    /// </summary>
    /// <param name="usuarioId">Identificador do usuário.</param>
    /// <returns>Lista com os códigos dos perfis vinculados ao usuário.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<string>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<string>>>> ObterPerfisAsync(Guid usuarioId)
    {
        _logger.LogInformation("Consultando perfis do usuário. UsuarioId: {UsuarioId}", usuarioId);

        var perfis = await _obterPerfisHandler.HandleAsync(new ObterPerfisDoUsuarioQuery(usuarioId), HttpContext.RequestAborted);

        _logger.LogInformation("Perfis obtidos com sucesso. UsuarioId: {UsuarioId}, Quantidade: {Quantidade}", usuarioId, perfis.Count);

        return Ok(ApiResponse<IReadOnlyCollection<string>>.SuccessResponse(perfis));
    }
}


