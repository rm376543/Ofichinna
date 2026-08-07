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
    private readonly IRepository<Checklist> _checklistRepository;
    private readonly IAgendamentoRepository _agendamentoRepository;
    private readonly IRepository<Pessoa> _pessoaRepository;
    private readonly IRepository<Veiculo> _veiculoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateOrcamentoService(
        IRepository<Orcamento> orcamentoRepository,
        IRepository<Checklist> checklistRepository,
        IAgendamentoRepository agendamentoRepository,
        IRepository<Pessoa> pessoaRepository,
        IRepository<Veiculo> veiculoRepository,
        IUnitOfWork unitOfWork)
    {
        _orcamentoRepository = orcamentoRepository;
        _checklistRepository = checklistRepository;
        _agendamentoRepository = agendamentoRepository;
        _pessoaRepository = pessoaRepository;
        _veiculoRepository = veiculoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> CreateAsync(CreateOrcamentoCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var pessoa = await _pessoaRepository.GetByIdAsync(command.PessoaId, cancellationToken);
            if (pessoa is null || pessoa.EstaExcluida())
                return Result.Failure("Pessoa não encontrada.");

            var veiculo = await _veiculoRepository.GetByIdAsync(command.VeiculoId, cancellationToken);
            if (veiculo is null || veiculo.EstaExcluida())
                return Result.Failure("Veículo não encontrado.");

            var agendamento = await _agendamentoRepository.GetByIdAsync(command.AgendamentoId, cancellationToken);
            if (agendamento is null || agendamento.EstaExcluida())
                return Result.Failure("Agendamento não encontrado.");

            if (agendamento.ClientePessoaId != command.PessoaId || agendamento.VeiculoId != command.VeiculoId)
                return Result.Failure("O agendamento informado não corresponde à pessoa e ao veículo do orçamento.");

            var checklists = (await _checklistRepository.GetAllAsync(cancellationToken))
                .Where(x => x.AgendamentoId == command.AgendamentoId && !x.EstaExcluida())
                .ToList();

            if (checklists.Count == 0)
                return Result.Failure("Nenhum checklist encontrado para o agendamento informado.");

            if (checklists.Any(x => !x.Finalizado))
                return Result.Failure("Existem checklists pendentes para o agendamento informado.");

            var mecanicoDiagnostico = await _pessoaRepository.GetByIdAsync(command.MecanicoDiagnosticoId, cancellationToken);
            if (mecanicoDiagnostico is null || mecanicoDiagnostico.EstaExcluida())
                return Result.Failure("Mecânico do diagnóstico não encontrado.");

            var responsavel = await _pessoaRepository.GetByIdAsync(command.ResponsavelId, cancellationToken);
            if (responsavel is null || responsavel.EstaExcluida())
                return Result.Failure("Responsável não encontrado.");

            var orcamento = new Orcamento(
                command.PessoaId,
                command.VeiculoId,
                command.AgendamentoId,
                command.MecanicoDiagnosticoId,
                command.ResponsavelId,
                command.DataValidade,
                command.Desconto,
                command.Observacoes);

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
