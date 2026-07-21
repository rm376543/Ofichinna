using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Veiculos.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Exceptions;
using Ofichina.Application.Abstractions.Interfaces;
using Ofichina.Domain.ValueObjects;
using Ofichina.Domain.Common;

namespace Ofichina.Application.UseCases.Veiculos.Handlers;

/// <summary>
/// Handler para criação de veículo.
/// </summary>
public sealed class CreateVeiculoCommandHandler : ICommandHandler<CreateVeiculoCommand, Result<Guid>>
{
    private readonly IVeiculoRepository _veiculoRepository;
    private readonly IRepository<Pessoa> _pessoaRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateVeiculoCommandHandler> _logger;

    public CreateVeiculoCommandHandler(
        IVeiculoRepository veiculoRepository,
        IRepository<Pessoa> pessoaRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateVeiculoCommandHandler> logger)
    {
        _veiculoRepository = veiculoRepository;
        _pessoaRepository = pessoaRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> HandleAsync(CreateVeiculoCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Iniciando criação de veículo. Placa: {Placa}", command.Placa);

            var placa = new Placa(command.Placa);

            var pessoa = await _pessoaRepository.GetByIdAsync(command.PessoaId, cancellationToken);

            if (pessoa is null || pessoa.EstaExcluida())
                return Result.Failure<Guid>("Pessoa não encontrada.");

            var existente = (await _veiculoRepository.GetAllWithPessoaAsync(cancellationToken))
                .FirstOrDefault(v => v.Placa.Numero == placa.Numero);

            if (existente is not null)
                return Result.Failure<Guid>("Já existe um veículo cadastrado com esta placa.");

            var veiculo = new Veiculo(
                command.PessoaId,
                placa,
                command.Marca,
                command.Modelo,
                command.AnoFabricacao,
                command.Cor ?? string.Empty,
                command.Observacoes,
                new Hodometro(command.Hodometro));

            if (!command.Ativo)
                veiculo.Desativar();

            await _veiculoRepository.AddAsync(veiculo, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success(veiculo.Id);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domínio ao criar veículo.");
            return Result.Failure<Guid>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar veículo.");
            return Result.Failure<Guid>("Não foi possível criar o veículo.");
        }
    }
}
