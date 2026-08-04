using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.Abstractions.Authentication;
using Ofichina.Application.Abstractions.Common;
using Ofichina.Application.Abstractions.Interfaces;
using Ofichina.Application.UseCases.Orcamentos.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Common;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Exceptions;

namespace Ofichina.Application.UseCases.Orcamentos.Handlers;

/// <summary>
/// Handler para enviar orçamento ao cliente.
/// </summary>
public sealed class EnviarOrcamentoParaClienteCommandHandler : ICommandHandler<EnviarOrcamentoParaClienteCommand, Result>
{
    private readonly IOrcamentoRepository _orcamentoRepository;
    private readonly IRepository<HistoricoStatus> _historicoStatusRepository;
    private readonly IUsuarioAtualService _usuarioAtualService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<EnviarOrcamentoParaClienteCommandHandler> _logger;

    public EnviarOrcamentoParaClienteCommandHandler(
        IOrcamentoRepository orcamentoRepository,
        IRepository<HistoricoStatus> historicoStatusRepository,
        IUsuarioAtualService usuarioAtualService,
        IUnitOfWork unitOfWork,
        ILogger<EnviarOrcamentoParaClienteCommandHandler> logger)
    {
        _orcamentoRepository = orcamentoRepository;
        _historicoStatusRepository = historicoStatusRepository;
        _usuarioAtualService = usuarioAtualService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(EnviarOrcamentoParaClienteCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var orcamento = await _orcamentoRepository.GetByIdAsync(command.Id, cancellationToken, tracking: true);
            if (orcamento is null || orcamento.EstaExcluida())
                return Result.Failure("Orçamento não encontrado.");

            var statusAnterior = orcamento.Status;
            orcamento.EnviarParaCliente();

            await _orcamentoRepository.UpdateAsync(orcamento, cancellationToken);
            await _historicoStatusRepository.AddAsync(
                HistoricoStatus.ParaOrcamento(
                    orcamento.Id,
                    statusAnterior.ToUpperSnakeCase(),
                    orcamento.Status.ToUpperSnakeCase(),
                    _usuarioAtualService.ObterUsuarioId()),
                cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domínio ao enviar orçamento ao cliente. OrcamentoId: {OrcamentoId}", command.Id);
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao enviar orçamento ao cliente. OrcamentoId: {OrcamentoId}", command.Id);
            return Result.Failure("Não foi possível enviar o orçamento.");
        }
    }
}
