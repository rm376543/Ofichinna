using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.OrdensServico.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Exceptions;

namespace Ofichina.Application.UseCases.OrdensServico.Handlers;

/// <summary>
/// Handler para atualizacao de ordem de servico.
/// </summary>
public sealed class UpdateOrdemServicoCommandHandler : ICommandHandler<UpdateOrdemServicoCommand, Result>
{
    private readonly IRepository<OrdemServico> _ordemServicoRepository;
    private readonly IRepository<Pessoa> _pessoaRepository;
    private readonly IRepository<Veiculo> _veiculoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateOrdemServicoCommandHandler> _logger;

    public UpdateOrdemServicoCommandHandler(
        IRepository<OrdemServico> ordemServicoRepository,
        IRepository<Pessoa> pessoaRepository,
        IRepository<Veiculo> veiculoRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateOrdemServicoCommandHandler> logger)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _pessoaRepository = pessoaRepository;
        _veiculoRepository = veiculoRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(UpdateOrdemServicoCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Iniciando atualização da ordem de serviço. OrdemServicoId: {OrdemServicoId}.", command.OrdemServicoId);

            var ordemServico = await _ordemServicoRepository.GetByIdAsync(command.OrdemServicoId, cancellationToken);
            if (ordemServico is null || ordemServico.EstaExcluida())
                return Result.Failure("Ordem de serviço não encontrada.");

            var pessoa = await _pessoaRepository.GetByIdAsync(command.PessoaId, cancellationToken);
            if (pessoa is null || pessoa.EstaExcluida())
                return Result.Failure("Pessoa não encontrada.");

            var funcionario = await _pessoaRepository.GetByIdAsync(command.ConsultorId, cancellationToken);
            if (funcionario is null || funcionario.EstaExcluida())
                return Result.Failure("Funcionário não encontrado.");

            var veiculo = await _veiculoRepository.GetByIdAsync(command.VeiculoId, cancellationToken);
            if (veiculo is null || veiculo.EstaExcluida())
                return Result.Failure("Veículo não encontrado.");

            ordemServico.AtualizarDados(
                command.PessoaId,
                command.VeiculoId,
                command.ConsultorId,
                command.Hodometro,
                command.ProblemaRelatado,
                command.Observacoes);

            await _ordemServicoRepository.UpdateAsync(ordemServico, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Ordem de serviço atualizada com sucesso. OrdemServicoId: {OrdemServicoId}", command.OrdemServicoId);
            return Result.Success();
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domínio ao atualizar ordem de serviço. OrdemServicoId: {OrdemServicoId}", command.OrdemServicoId);
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao atualizar ordem de serviço. OrdemServicoId: {OrdemServicoId}", command.OrdemServicoId);
            return Result.Failure("Não foi possível atualizar a ordem de serviço.");
        }
    }
}

