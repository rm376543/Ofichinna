using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Orcamentos.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Exceptions;

namespace Ofichina.Application.UseCases.Orcamentos.Handlers;

/// <summary>
/// Handler para atualização de desconto do orçamento.
/// </summary>
public sealed class UpdateOrcamentoDescontoCommandHandler : ICommandHandler<UpdateOrcamentoDescontoCommand, Result>
{
    private readonly IOrcamentoRepository _orcamentoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateOrcamentoDescontoCommandHandler> _logger;

    public UpdateOrcamentoDescontoCommandHandler(
        IOrcamentoRepository orcamentoRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateOrcamentoDescontoCommandHandler> logger)
    {
        _orcamentoRepository = orcamentoRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(UpdateOrcamentoDescontoCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var orcamento = await _orcamentoRepository.GetByIdAsync(command.OrcamentoId, includeItens: true, cancellationToken: cancellationToken, tracking: true);
            if (orcamento is null || orcamento.EstaExcluida())
                return Result.Failure("Orçamento não encontrado.");

            orcamento.AtualizarDesconto(command.Desconto, command.DescontoEmDinheiro);

            await _orcamentoRepository.UpdateAsync(orcamento, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao atualizar desconto do orçamento. OrcamentoId: {OrcamentoId}", command.OrcamentoId);
            return Result.Failure("Não foi possível atualizar o desconto do orçamento.");
        }
    }
}
