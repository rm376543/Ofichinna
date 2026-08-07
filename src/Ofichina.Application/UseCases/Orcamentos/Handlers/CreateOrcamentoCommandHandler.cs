using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Orcamentos.Commands;
using Ofichina.Contracts.Common;

namespace Ofichina.Application.UseCases.Orcamentos.Handlers;

/// <summary>
/// Handler para criação de orçamento.
/// </summary>
public sealed class CreateOrcamentoCommandHandler : ICommandHandler<CreateOrcamentoCommand, Result>
{
    private readonly ICreateOrcamentoService _createOrcamentoService;
    private readonly ILogger<CreateOrcamentoCommandHandler> _logger;

    public CreateOrcamentoCommandHandler(
        ICreateOrcamentoService createOrcamentoService,
        ILogger<CreateOrcamentoCommandHandler> logger)
    {
        _createOrcamentoService = createOrcamentoService;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(CreateOrcamentoCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Iniciando a criação do orçamento. PessoaId: {PessoaId}, VeiculoId: {VeiculoId}, AgendamentoId: {AgendamentoId}.", command.PessoaId, command.VeiculoId, command.AgendamentoId);

            var result = await _createOrcamentoService.CreateAsync(command, cancellationToken);

            if (result.IsSuccess)
                _logger.LogInformation("Orçamento criado com sucesso.");

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao criar orçamento.");
            return Result.Failure("Não foi possível criar o orçamento.");
        }
    }
}
