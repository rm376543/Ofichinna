using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.Abstractions.Common;
using Ofichina.Application.UseCases.Checklists.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Exceptions;

namespace Ofichina.Application.UseCases.Checklists.Handlers;

/// <summary>
/// Handler para criação de checklist.
/// </summary>
public sealed class CreateChecklistCommandHandler : ICommandHandler<CreateChecklistCommand, Result>
{
    private readonly IRepository<Checklist> _checklistRepository;
    private readonly IRepository<Veiculo> _veiculoRepository;
    private readonly IRepository<Pessoa> _pessoaRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateChecklistCommandHandler> _logger;

    public CreateChecklistCommandHandler(
        IRepository<Checklist> checklistRepository,
        IRepository<Veiculo> veiculoRepository,
        IRepository<Pessoa> pessoaRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateChecklistCommandHandler> logger)
    {
        _checklistRepository = checklistRepository;
        _veiculoRepository = veiculoRepository;
        _pessoaRepository = pessoaRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(CreateChecklistCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var veiculo = await _veiculoRepository.GetByIdAsync(command.VeiculoId, cancellationToken);
            if (veiculo is null || veiculo.EstaExcluida())
                return Result.Failure("Veículo não encontrado.");

            var pessoa = await _pessoaRepository.GetByIdAsync(command.PessoaId, cancellationToken);
            if (pessoa is null || pessoa.EstaExcluida())
                return Result.Failure("Pessoa não encontrada.");

            var checklist = new Checklist(
                command.VeiculoId,
                command.PessoaId,
                command.HodometroEntrada,
                command.ItensVerificados,
                command.Observacoes);

            await _checklistRepository.AddAsync(checklist, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domínio ao criar checklist.");
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao criar checklist.");
            return Result.Failure("Não foi possível criar o checklist.");
        }
    }
}