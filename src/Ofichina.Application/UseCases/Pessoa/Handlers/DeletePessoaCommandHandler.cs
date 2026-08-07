using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Pessoas.Commands;
using Ofichina.Contracts.Common;

namespace Ofichina.Application.UseCases.Pessoas.Handlers;

/// <summary>
/// Handler para desativar uma pessoa.
/// </summary>
public sealed class DeletePessoaCommandHandler : ICommandHandler<DeletePessoaCommand, Result>
{
    private readonly IPessoaRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeletePessoaCommandHandler> _logger;

    public DeletePessoaCommandHandler(
        IPessoaRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<DeletePessoaCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(DeletePessoaCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Iniciando desativação da pessoa com Id: {PessoaId}.", command.PessoaId);

            var pessoa = await _repository.GetByIdAsync(command.PessoaId, cancellationToken);

            if (pessoa is null || pessoa.EstaExcluida())
            {
                _logger.LogWarning("Pessoa não encontrada para desativação. PessoaId: {PessoaId}", command.PessoaId);
                return Result.Failure("Pessoa não encontrada.");
            }

            pessoa.Desativar();

            await _repository.UpdateAsync(pessoa, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Pessoa desativada com sucesso. PessoaId: {PessoaId}", command.PessoaId);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao desativar pessoa. PessoaId: {PessoaId}", command.PessoaId);
            return Result.Failure("Ocorreu um erro ao desativar a pessoa.");
        }
    }
}

