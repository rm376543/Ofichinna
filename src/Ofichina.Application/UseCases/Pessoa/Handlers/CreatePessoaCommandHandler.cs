using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Pessoas.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Exceptions;
using Ofichina.Domain.Interfaces;
using Ofichina.Domain.ValueObjects;

namespace Ofichina.Application.UseCases.Pessoas.Handlers;

/// <summary>
/// Handler para criar uma pessoa.
/// </summary>
public sealed class CreatePessoaCommandHandler : ICommandHandler<CreatePessoaCommand, Result<Guid>>
{
    private readonly IPessoaRepository _repository;
    private readonly IRepository<Usuario> _usuarioRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreatePessoaCommandHandler> _logger;

    public CreatePessoaCommandHandler(
        IPessoaRepository repository,
        IRepository<Usuario> usuarioRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreatePessoaCommandHandler> logger)
    {
        _repository = repository;
        _usuarioRepository = usuarioRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> HandleAsync(CreatePessoaCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Iniciando a criação da pessoa com documento {Documento}.", command.Documento);

            var usuario = await _usuarioRepository.GetByIdAsync(command.UsuarioId, cancellationToken);

            if (usuario is null)
            {
                _logger.LogWarning("Usuário não encontrado para vinculação da pessoa. UsuarioId: {UsuarioId}", command.UsuarioId);
                return Result.Failure<Guid>("Usuário não encontrado.");
            }

            var documento = CriarDocumento(command.Documento);
            var telefone = new Telefone(command.Telefone);
            var endereco = new Endereco(
                command.Logradouro,
                command.Numero,
                command.Complemento,
                command.Bairro,
                command.Cidade,
                command.Estado,
                new Cep(command.Cep));

            var pessoa = new Pessoa(command.Nome,
                                    documento,
                                    telefone,
                                    endereco,
                                    command.UsuarioId);

            await _repository.AddAsync(pessoa, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Pessoa criada com sucesso. PessoaId: {PessoaId}", pessoa.Id);
            return Result.Success(pessoa.Id);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domínio ao criar pessoa.");
            return Result.Failure<Guid>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao criar pessoa.");
            return Result.Failure<Guid>("Ocorreu um erro ao criar a pessoa.");
        }
    }

    private static Documento CriarDocumento(string numero)
    {
        var somenteDigitos = new string(numero.Where(char.IsDigit).ToArray());

        return somenteDigitos.Length switch
        {
            11 => new Cpf(somenteDigitos),
            14 => new Cnpj(somenteDigitos),
            _ => throw new DomainException("Documento inválido.")
        };
    }
}
