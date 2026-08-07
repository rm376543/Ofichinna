using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Orcamentos.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Enums;
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
    private readonly IRepository<Servico> _servicoRepository;
    private readonly IRepository<Peca> _pecaRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateOrcamentoCommandHandler> _logger;

    public UpdateOrcamentoCommandHandler(
        IOrcamentoRepository orcamentoRepository,
        IRepository<Pessoa> pessoaRepository,
        IRepository<Veiculo> veiculoRepository,
        IRepository<Servico> servicoRepository,
        IRepository<Peca> pecaRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateOrcamentoCommandHandler> logger)
    {
        _orcamentoRepository = orcamentoRepository;
        _pessoaRepository = pessoaRepository;
        _veiculoRepository = veiculoRepository;
        _servicoRepository = servicoRepository;
        _pecaRepository = pecaRepository;
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

            var mecanicoDiagnostico = await _pessoaRepository.GetByIdAsync(command.MecanicoDiagnosticoId, cancellationToken);
            if (mecanicoDiagnostico is null || mecanicoDiagnostico.EstaExcluida())
                return Result.Failure("Mecânico do diagnóstico não encontrado.");

            var responsavel = await _pessoaRepository.GetByIdAsync(command.ResponsavelId, cancellationToken);
            if (responsavel is null || responsavel.EstaExcluida())
                return Result.Failure("Responsável não encontrado.");

            orcamento.AtualizarDados(
                command.PessoaId,
                command.VeiculoId,
                command.MecanicoDiagnosticoId,
                command.ResponsavelId,
                command.DataValidade,
                command.Desconto,
                command.Observacoes);

            foreach (var item in orcamento.ItensServico.ToList())
                orcamento.RemoverServico(item.Id, StatusOrcamento.EmDiagnostico);

            foreach (var item in command.ItensServico)
            {
                var servico = await _servicoRepository.GetByIdAsync(item.ServicoId, cancellationToken);
                if (servico is null || servico.EstaExcluida())
                    return Result.Failure("Serviço não encontrado.");

                if (item.PecaId.HasValue)
                {
                    var peca = await _pecaRepository.GetByIdAsync(item.PecaId.Value, cancellationToken);
                    if (peca is null || peca.EstaExcluida())
                        return Result.Failure("Peça não encontrada.");
                }

                orcamento.AdicionarServico(item.ServicoId, item.PecaId, item.Quantidade, StatusOrcamento.EmDiagnostico);
            }

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
