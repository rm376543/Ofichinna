using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Pessoas.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Exceptions;
using Ofichina.Domain.ValueObjects;

namespace Ofichina.Application.UseCases.Pessoas.Handlers;

/// <summary>
/// Handler para atualizar uma pessoa.
/// </summary>
public sealed class UpdatePessoaCommandHandler : ICommandHandler<UpdatePessoaCommand, Result>
{
    private readonly IPessoaRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdatePessoaCommandHandler> _logger;

    public UpdatePessoaCommandHandler(
        IPessoaRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<UpdatePessoaCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(UpdatePessoaCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Iniciando atualização da pessoa com Id: {PessoaId}.", command.PessoaId);

            var pessoa = await _repository.GetByIdAsync(command.PessoaId, cancellationToken);

            if (pessoa is null || pessoa.EstaExcluida())
            {
                _logger.LogWarning("Pessoa não encontrada para atualização. PessoaId: {PessoaId}", command.PessoaId);
                return Result.Failure("Pessoa não encontrada.");
            }

            pessoa.AlterarNome(command.Nome);
            pessoa.AlterarTelefone(new Telefone(command.Telefone));
            pessoa.AlterarEndereco(new Endereco(
                command.Logradouro,
                command.Numero,
                command.Complemento,
                command.Bairro,
                command.Cidade,
                command.Estado,
                new Cep(command.Cep)));

            await _repository.UpdateAsync(pessoa, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Pessoa atualizada com sucesso. PessoaId: {PessoaId}", command.PessoaId);
            return Result.Success();
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domínio ao atualizar pessoa. PessoaId: {PessoaId}", command.PessoaId);
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao atualizar pessoa. PessoaId: {PessoaId}", command.PessoaId);
            return Result.Failure("Ocorreu um erro ao atualizar a pessoa.");
        }
    }
}

