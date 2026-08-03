using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.Abstractions.Common;
using Ofichina.Application.Abstractions.Interfaces;
using Ofichina.Application.UseCases.Orcamentos.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Aggregates;
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
            var orcamento = await _orcamentoRepository.GetByIdAsync(command.Id, includeItens: true, cancellationToken: cancellationToken, tracking: true);
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

            foreach (var item in orcamento.ItensPrevistos.ToList())
                orcamento.RemoverServico(item.Id);

            foreach (var item in command.Servicos)
            {
                var servico = await _servicoRepository.GetByIdAsync(item.ServicoId, cancellationToken);
                if (servico is null || servico.EstaExcluida())
                    return Result.Failure("Serviço não encontrado.");

                var servicoOrcamento = orcamento.AdicionarServico(item.ServicoId);

                foreach (var pecaItem in item.Pecas)
                {
                    var peca = await _pecaRepository.GetByIdAsync(pecaItem.PecaId, cancellationToken);
                    if (peca is null || peca.EstaExcluida())
                        return Result.Failure("Peça não encontrada.");

                    servicoOrcamento.AdicionarPeca(pecaItem.PecaId, pecaItem.Quantidade);
                }
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
            _logger.LogError(ex, "Erro inesperado ao atualizar orçamento. OrcamentoId: {OrcamentoId}", command.Id);
            return Result.Failure("Não foi possível atualizar o orçamento.");
        }
    }
}
