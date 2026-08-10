using Ofichina.Application.Abstractions;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Application.UseCases.Orcamentos.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Exceptions;

namespace Ofichina.Application.UseCases.Orcamentos.Handlers;

/// <summary>
/// Handler para atualização de orçamento.
/// </summary>
public sealed class UpdateOrcamentoCommandHandler : ICommandHandler<UpdateOrcamentoCommand, Result>
{
    private readonly IOrcamentoRepository _orcamentoRepository;
    private readonly IRepository<Pessoa> _pessoaRepository;
    private readonly IRepository<Veiculo> _veiculoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateOrcamentoCommandHandler> _logger;

    public UpdateOrcamentoCommandHandler(
        IOrcamentoRepository orcamentoRepository,
        IRepository<Pessoa> pessoaRepository,
        IRepository<Veiculo> veiculoRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateOrcamentoCommandHandler> logger)
    {
        _orcamentoRepository = orcamentoRepository;
        _pessoaRepository = pessoaRepository;
        _veiculoRepository = veiculoRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(UpdateOrcamentoCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var orcamento = await _orcamentoRepository.GetByIdAsync(command.OrcamentoId, includeItens: true, cancellationToken: cancellationToken, tracking: true);
            if (orcamento is null || orcamento.EstaExcluida())
                return Result.Failure("Orçamento não encontrado.");

            var pessoa = await _pessoaRepository.GetByIdAsync(command.PessoaId, cancellationToken);
            if (pessoa is null || pessoa.EstaExcluida())
                return Result.Failure("Pessoa não encontrada.");

            var veiculo = await _veiculoRepository.GetByIdAsync(command.VeiculoId, cancellationToken);
            if (veiculo is null || veiculo.EstaExcluida())
                return Result.Failure("Veículo não encontrado.");

            var mecanicoDiagnostico = await _pessoaRepository.GetByIdAsync(command.MecanicoId, cancellationToken);
            if (mecanicoDiagnostico is null || mecanicoDiagnostico.EstaExcluida())
                return Result.Failure("Mecânico do diagnóstico não encontrado.");

            var consultor = await _pessoaRepository.GetByIdAsync(command.ConsultorId, cancellationToken);
            if (consultor is null || consultor.EstaExcluida())
                return Result.Failure("Consultor não encontrado.");

            orcamento.AtualizarDados(
                command.PessoaId,
                command.VeiculoId,
                command.MecanicoId,
                command.ConsultorId,
                command.DataValidade.ToDateTime(TimeOnly.MinValue),
                command.Observacoes);

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
            _logger.LogError(ex, "Erro inesperado ao atualizar orçamento. OrcamentoId: {OrcamentoId}", command.OrcamentoId);
            return Result.Failure("Não foi possível atualizar o orçamento.");
        }
    }
}
