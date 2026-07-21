using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.OrdensServico.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Exceptions;
using Ofichina.Domain.Common;
using DomainPeca = Ofichina.Domain.Entities.Peca;

namespace Ofichina.Application.UseCases.OrdensServico.Handlers;

/// <summary>
/// Handler para criação de ordem de serviço.
/// </summary>
public sealed class CreateOrdemServicoCommandHandler : ICommandHandler<CreateOrdemServicoCommand, Result<Guid>>
{
    private readonly IRepository<OrdemServico> _ordemServicoRepository;
    private readonly IRepository<Pessoa> _pessoaRepository;
    private readonly IRepository<Veiculo> _veiculoRepository;
    private readonly IRepository<Servico> _servicoRepository;
    private readonly IRepository<DomainPeca> _pecaRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateOrdemServicoCommandHandler> _logger;

    public CreateOrdemServicoCommandHandler(
        IRepository<OrdemServico> ordemServicoRepository,
        IRepository<Pessoa> pessoaRepository,
        IRepository<Veiculo> veiculoRepository,
        IRepository<Servico> servicoRepository,
        IRepository<DomainPeca> pecaRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateOrdemServicoCommandHandler> logger)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _pessoaRepository = pessoaRepository;
        _veiculoRepository = veiculoRepository;
        _servicoRepository = servicoRepository;
        _pecaRepository = pecaRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> HandleAsync(CreateOrdemServicoCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Iniciando a criação da ordem de serviço. PessoaId: {PessoaId}, VeiculoId: {VeiculoId}.", command.PessoaId, command.VeiculoId);

            var pessoa = await _pessoaRepository.GetByIdAsync(command.PessoaId, cancellationToken);
            if (pessoa is null || pessoa.EstaExcluida())
                return Result.Failure<Guid>("Pessoa não encontrada.");

            var funcionario = await _pessoaRepository.GetByIdAsync(command.FuncionarioId, cancellationToken);
            if (funcionario is null || funcionario.EstaExcluida())
                return Result.Failure<Guid>("Funcionário não encontrado.");

            var veiculo = await _veiculoRepository.GetByIdAsync(command.VeiculoId, cancellationToken);
            if (veiculo is null || veiculo.EstaExcluida())
                return Result.Failure<Guid>("Veículo não encontrado.");

            var ordemServico = new OrdemServico(
                command.PessoaId,
                command.VeiculoId,
                command.FuncionarioId,
                command.Observacoes);

            foreach (var itemServicoRequest in command.Servicos)
            {
                var servico = await _servicoRepository.GetByIdAsync(itemServicoRequest.ServicoId, cancellationToken);
                if (servico is null || servico.EstaExcluida())
                    return Result.Failure<Guid>("Serviço não encontrado.");

                var itemServico = ordemServico.AdicionarServico(servico.Id, servico.Nome, servico.Valor);

                foreach (var pecaRequest in itemServicoRequest.Pecas)
                {
                    var peca = await _pecaRepository.GetByIdAsync(pecaRequest.PecaId, cancellationToken);
                    if (peca is null || peca.EstaExcluida())
                        return Result.Failure<Guid>("Peça não encontrada.");

                    itemServico.AdicionarPeca(peca.Id, peca.Nome, pecaRequest.Quantidade, peca.Valor);
                }
            }

            await _ordemServicoRepository.AddAsync(ordemServico, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Ordem de serviço criada com sucesso. OrdemServicoId: {OrdemServicoId}", ordemServico.Id);
            return Result.Success(ordemServico.Id);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domínio ao criar ordem de serviço.");
            return Result.Failure<Guid>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao criar ordem de serviço.");
            return Result.Failure<Guid>("Não foi possível criar a ordem de serviço.");
        }
    }
}

