using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Cliente.Commands;
using Ofichina.Application.UseCases.Cliente.Queries;
using Ofichina.Contracts.Responses;
using Ofichina.Contracts.Responses.Cliente;
using Ofichina.Domain.ValueObjects;

namespace Ofichina.Api.Controllers.Cliente;

[Authorize(Roles = "Administrador")]
[ApiController]
[Route("api/clientes/{clienteId:guid}/perfis")]
public sealed class ClientesPerfisController : ControllerBase
{
    private readonly ICommandHandler<VincularPerfilClienteCommand, Result<VincularPerfilClienteResponse>> _vincularHandler;
    private readonly IQueryHandler<ObterPerfisDoClienteQuery, IReadOnlyCollection<string>> _obterPerfisHandler;

    public ClientesPerfisController(
        ICommandHandler<VincularPerfilClienteCommand, Result<VincularPerfilClienteResponse>> vincularHandler,
        IQueryHandler<ObterPerfisDoClienteQuery, IReadOnlyCollection<string>> obterPerfisHandler)
    {
        _vincularHandler = vincularHandler;
        _obterPerfisHandler = obterPerfisHandler;
    }

    [AllowAnonymous]
    [HttpPost("{perfilId:guid}")]
    public async Task<ActionResult<ApiResponse>> VincularAsync(Guid clienteId, Guid perfilId)
    {
        var result = await _vincularHandler.HandleAsync(new VincularPerfilClienteCommand(clienteId, perfilId));

        if (!result.IsSuccess)
            return BadRequest(ApiResponse.FailureResponse(result.Error ?? "Não foi possível vincular o perfil."));

        return Ok(ApiResponse.SuccessResponse(result.Value?.Mensagem ?? "Perfil vinculado com sucesso."));
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<string>>>> ObterPerfisAsync(Guid clienteId)
    {
        var perfis = await _obterPerfisHandler.HandleAsync(new ObterPerfisDoClienteQuery(clienteId));
        return Ok(ApiResponse<IReadOnlyCollection<string>>.SuccessResponse(perfis));
    }
}