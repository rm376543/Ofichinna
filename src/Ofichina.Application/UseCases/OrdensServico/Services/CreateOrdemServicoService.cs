using Ofichina.Application.Abstractions.Common;
using Ofichina.Application.Abstractions.Interfaces;
using Ofichina.Application.UseCases.OrdensServico.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Exceptions;

namespace Ofichina.Application.UseCases.OrdensServico.Services;

/// <summary>
/// Serviço responsável por criar ordens de serviço.
/// </summary>
public sealed class CreateOrdemServicoService : ICreateOrdemServicoService
{
    private readonly IRepository<OrdemServico> _ordemServicoRepository;
    private readonly IRepository<Pessoa> _pessoaRepository;
    private readonly IRepository<Veiculo> _veiculoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateOrdemServicoService(
        IRepository<OrdemServico> ordemServicoRepository,
        IRepository<Pessoa> pessoaRepository,
        IRepository<Veiculo> veiculoRepository,
        IUnitOfWork unitOfWork)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _pessoaRepository = pessoaRepository;
        _veiculoRepository = veiculoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> CriarAsync(CreateOrdemServicoCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var pessoa = await _pessoaRepository.GetByIdAsync(command.PessoaId, cancellationToken);
            if (pessoa is null || pessoa.EstaExcluida())
                return Result.Failure("Pessoa não encontrada.");

            var funcionario = await _pessoaRepository.GetByIdAsync(command.FuncionarioId, cancellationToken);
            if (funcionario is null || funcionario.EstaExcluida())
                return Result.Failure("Funcionário não encontrado.");

            var veiculo = await _veiculoRepository.GetByIdAsync(command.VeiculoId, cancellationToken);
            if (veiculo is null || veiculo.EstaExcluida())
                return Result.Failure("Veículo não encontrado.");

            var ordemServico = new OrdemServico(
                command.PessoaId,
                command.VeiculoId,
                command.FuncionarioId,
                command.HodometroEntrada,
                command.ProblemaRelatado,
                command.Observacoes);

            await _ordemServicoRepository.AddAsync(ordemServico, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message);
        }
        catch (Exception)
        {
            return Result.Failure("Não foi possível criar a ordem de serviço.");
        }
    }
}