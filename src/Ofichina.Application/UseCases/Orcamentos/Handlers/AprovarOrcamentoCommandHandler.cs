using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.Abstractions.Common;
using Ofichina.Application.Abstractions.Interfaces;
using Ofichina.Application.UseCases.Orcamentos.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Exceptions;

namespace Ofichina.Application.UseCases.Orcamentos.Handlers;

/// <summary>
/// Handler para aprovar orçamento e gerar ordem de serviço.
/// </summary>
public sealed class AprovarOrcamentoCommandHandler : ICommandHandler<AprovarOrcamentoCommand, Result>
{
    private readonly IOrcamentoRepository _orcamentoRepository;
    private readonly IRepository<OrdemServico> _ordemServicoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AprovarOrcamentoCommandHandler> _logger;

    public AprovarOrcamentoCommandHandler(
        IOrcamentoRepository orcamentoRepository,
        IRepository<OrdemServico> ordemServicoRepository,
        IUnitOfWork unitOfWork,
        ILogger<AprovarOrcamentoCommandHandler> logger)
    {
        _orcamentoRepository = orcamentoRepository;
        _ordemServicoRepository = ordemServicoRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(AprovarOrcamentoCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var orcamento = await _orcamentoRepository.GetByIdAsync(command.Id, includeItens: true, cancellationToken, tracking: true);
            if (orcamento is null || orcamento.EstaExcluida())
                return Result.Failure("Orçamento não encontrado.");

            orcamento.Aprovar();
            var ordemServico = OrdemServico.CriarAPartirDoOrcamento(orcamento, command.MecanicoReparoId);

            await _orcamentoRepository.UpdateAsync(orcamento, cancellationToken);
            await _ordemServicoRepository.AddAsync(ordemServico, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domínio ao aprovar orçamento. OrcamentoId: {OrcamentoId}", command.Id);
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao aprovar orçamento. OrcamentoId: {OrcamentoId}", command.Id);
            return Result.Failure("Não foi possível aprovar o orçamento.");
        }
    }
}
