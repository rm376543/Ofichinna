using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.OrdensServico.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Exceptions;
using Ofichina.Domain.Interfaces;

namespace Ofichina.Application.UseCases.OrdensServico.Handlers;

/// <summary>
/// Handler para criaÃ§Ã£o de ordem de serviÃ§o.
/// </summary>
public sealed class CreateOrdemServicoCommandHandler : ICommandHandler<CreateOrdemServicoCommand, Result<Guid>>
{
    private readonly IRepository<OrdemServico> _ordemServicoRepository;
    private readonly IRepository<Pessoa> _pessoaRepository;
    private readonly IRepository<Veiculo> _veiculoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateOrdemServicoCommandHandler> _logger;

    public CreateOrdemServicoCommandHandler(
        IRepository<OrdemServico> ordemServicoRepository,
        IRepository<Pessoa> pessoaRepository,
        IRepository<Veiculo> veiculoRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateOrdemServicoCommandHandler> logger)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _pessoaRepository = pessoaRepository;
        _veiculoRepository = veiculoRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> HandleAsync(CreateOrdemServicoCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Iniciando a criaÃ§Ã£o da ordem de serviÃ§o. PessoaId: {PessoaId}, VeiculoId: {VeiculoId}.", command.PessoaId, command.VeiculoId);

            var pessoa = await _pessoaRepository.GetByIdAsync(command.PessoaId, cancellationToken);
            if (pessoa is null || pessoa.EstaExcluida())
                return Result.Failure<Guid>("Pessoa nÃ£o encontrada.");

            var funcionario = await _pessoaRepository.GetByIdAsync(command.FuncionarioId, cancellationToken);
            if (funcionario is null || funcionario.EstaExcluida())
                return Result.Failure<Guid>("FuncionÃ¡rio nÃ£o encontrado.");

            var veiculo = await _veiculoRepository.GetByIdAsync(command.VeiculoId, cancellationToken);
            if (veiculo is null || veiculo.EstaExcluida())
                return Result.Failure<Guid>("VeÃ­culo nÃ£o encontrado.");

            var ordemServico = new OrdemServico(
                command.PessoaId,
                command.VeiculoId,
                command.FuncionarioId,
                command.Observacoes);

            await _ordemServicoRepository.AddAsync(ordemServico, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Ordem de serviÃ§o criada com sucesso. OrdemServicoId: {OrdemServicoId}", ordemServico.Id);
            return Result.Success(ordemServico.Id);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domÃ­nio ao criar ordem de serviÃ§o.");
            return Result.Failure<Guid>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao criar ordem de serviÃ§o.");
            return Result.Failure<Guid>("NÃ£o foi possÃ­vel criar a ordem de serviÃ§o.");
        }
    }
}

