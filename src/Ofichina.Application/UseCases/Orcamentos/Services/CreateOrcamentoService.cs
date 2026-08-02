using Ofichina.Application.Abstractions.Common;
using Ofichina.Application.Abstractions.Interfaces;
using Ofichina.Application.UseCases.Orcamentos.Commands;
using Ofichina.Contracts;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Exceptions;

namespace Ofichina.Application.UseCases.Orcamentos.Services;

/// <summary>
/// Serviço responsável por criar orçamentos.
/// </summary>
public sealed class CreateOrcamentoService : ICreateOrcamentoService
{
    private readonly IRepository<Orcamento> _orcamentoRepository;
    private readonly IRepository<Pessoa> _pessoaRepository;
    private readonly IRepository<Veiculo> _veiculoRepository;
    private readonly IRepository<Servico> _servicoRepository;
    private readonly IRepository<Peca> _pecaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateOrcamentoService(
        IRepository<Orcamento> orcamentoRepository,
        IRepository<Pessoa> pessoaRepository,
        IRepository<Veiculo> veiculoRepository,
        IRepository<Servico> servicoRepository,
        IRepository<Peca> pecaRepository,
        IUnitOfWork unitOfWork)
    {
        _orcamentoRepository = orcamentoRepository;
        _pessoaRepository = pessoaRepository;
        _veiculoRepository = veiculoRepository;
        _servicoRepository = servicoRepository;
        _pecaRepository = pecaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> CriarAsync(CreateOrcamentoCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
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

            var orcamento = new Orcamento(
                command.PessoaId,
                command.VeiculoId,
                command.MecanicoDiagnosticoId,
                command.ResponsavelId,
                command.DataValidade,
                command.Desconto,
                command.Observacoes);

            foreach (var item in command.Servicos)
            {
                var servico = await _servicoRepository.GetByIdAsync(item.ServicoId, cancellationToken);
                if (servico is null || servico.EstaExcluida())
                    return Result.Failure("Serviço não encontrado.");

                orcamento.AdicionarServico(item.ServicoId, Guid.Empty, Convert.ToInt32(item.Quantidade));
            }

            foreach (var item in command.Pecas)
            {
                var peca = await _pecaRepository.GetByIdAsync(item.PecaId, cancellationToken);
                if (peca is null || peca.EstaExcluida())
                    return Result.Failure("Peça não encontrada.");

                orcamento.AdicionarServico(Guid.Empty, item.PecaId, Convert.ToInt32(item.Quantidade));
            }

            await _orcamentoRepository.AddAsync(orcamento, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message);
        }
        catch (Exception)
        {
            return Result.Failure("Não foi possível criar o orçamento.");
        }
    }
}
