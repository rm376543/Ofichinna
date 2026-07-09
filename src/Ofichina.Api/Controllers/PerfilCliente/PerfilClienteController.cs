using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.PerfilCliente.Commands;
using Ofichina.Application.UseCases.PerfilCliente.Queries;
using Ofichina.Contracts.Enums;
using Ofichina.Contracts.Requests.PerfilCliente;
using Ofichina.Contracts.Responses;
using Ofichina.Contracts.Responses.PerfilCliente;
using Ofichina.Domain.ValueObjects;

namespace Ofichina.Api.Controllers.PerfilCliente;

[Authorize]
[ApiController]
[Route("api/clientes/{clienteId:guid}/perfis")]
public sealed class PerfilClienteController : ControllerBase
{
    private readonly IValidator<VincularPerfilClienteRequest> _vincularValidator;
    private readonly ICommandHandler<VincularPerfilClienteCommand, Result<VincularPerfilClienteResponse>> _vincularHandler;
    private readonly IQueryHandler<ObterPerfisDoClienteQuery, IReadOnlyCollection<string>> _obterPerfisHandler;

    public PerfilClienteController(
        IValidator<VincularPerfilClienteRequest> vincularValidator,
        ICommandHandler<VincularPerfilClienteCommand, Result<VincularPerfilClienteResponse>> vincularHandler,
        IQueryHandler<ObterPerfisDoClienteQuery, IReadOnlyCollection<string>> obterPerfisHandler)
    {
        _vincularValidator = vincularValidator;
        _vincularHandler = vincularHandler;
        _obterPerfisHandler = obterPerfisHandler;
    }

    /// <summary>
    /// Vincula um perfil a um cliente existente.
    /// </summary>
    /// <param name="clienteId">Identificador do cliente.</param>
    /// <param name="perfilId">Identificador do perfil.</param>
    /// <returns>
    /// Retorna sucesso quando o vínculo é criado;
    /// retorna erro quando cliente, perfil ou vínculo já existirem.
    /// </returns>
    [Authorize(Policy = UserPolicyEnum.Escrever)]
    [HttpPost("{perfilId:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse>> VincularAsync(
        Guid clienteId,
        Guid perfilId,
        CancellationToken cancellationToken)
    {
        var request = new VincularPerfilClienteRequest
        {
            UsuarioId = clienteId,
            PerfilId = perfilId
        };

        var validation = await _vincularValidator.ValidateAsync(request, cancellationToken);

        if (!validation.IsValid)
        {
            return BadRequest(ApiResponse.FailureResponse(validation.Errors.Select(x => x.ErrorMessage)));
        }

        var result = await _vincularHandler.HandleAsync(new VincularPerfilClienteCommand(
            request.UsuarioId,
            request.PerfilId));

        if (!result.IsSuccess)
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível vincular o perfil."));

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
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<string>>>> ObterPerfisAsync(Guid clienteId)
    {
        var perfis = await _obterPerfisHandler.HandleAsync(new ObterPerfisDoClienteQuery(clienteId));
        return Ok(ApiResponse<IReadOnlyCollection<string>>.SuccessResponse(perfis));
    }
}