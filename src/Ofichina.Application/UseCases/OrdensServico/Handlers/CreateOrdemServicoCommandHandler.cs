using Ofichina.Application.Abstractions;
using Ofichina.Application.Abstractions.Interfaces.Service;
using Ofichina.Application.UseCases.OrdensServico.Commands;
using Ofichina.Contracts.Common;

namespace Ofichina.Application.UseCases.OrdensServico.Handlers;

/// <summary>
/// Handler para criação de ordem de serviço.
/// </summary>
public sealed class CreateOrdemServicoCommandHandler : ICommandHandler<CreateOrdemServicoCommand, Result>
{
    private readonly ICreateOrdemServicoService _createOrdemServicoService;
    private readonly ILogger<CreateOrdemServicoCommandHandler> _logger;

    public CreateOrdemServicoCommandHandler(
        ICreateOrdemServicoService createOrdemServicoService,
        ILogger<CreateOrdemServicoCommandHandler> logger)
    {
        _createOrdemServicoService = createOrdemServicoService;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(CreateOrdemServicoCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Iniciando a criação da ordem de serviço. PessoaId: {PessoaId}, VeiculoId: {VeiculoId}.", command.PessoaId, command.VeiculoId);

            var result = await _createOrdemServicoService.CreateAsync(command, cancellationToken);

            if (result.IsSuccess)
                _logger.LogInformation("Ordem de serviço criada com sucesso.");

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao criar ordem de serviço.");
            return Result.Failure("Não foi possível criar a ordem de serviço.");
        }
    }
}

