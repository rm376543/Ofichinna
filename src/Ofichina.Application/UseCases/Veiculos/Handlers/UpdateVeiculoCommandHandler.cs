using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Veiculos.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Exceptions;
using Ofichina.Application.Abstractions.Interfaces;
using Ofichina.Domain.ValueObjects;
using Ofichina.Domain.Common;
using Ofichina.Application.Abstractions.Common;

namespace Ofichina.Application.UseCases.Veiculos.Handlers;

/// <summary>
/// Handler para atualização de veículo.
/// </summary>
public sealed class UpdateVeiculoCommandHandler : ICommandHandler<UpdateVeiculoCommand, Result>
{
    private readonly IVeiculoRepository _veiculoRepository;
    private readonly IRepository<Pessoa> _pessoaRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateVeiculoCommandHandler> _logger;

    public UpdateVeiculoCommandHandler(
        IVeiculoRepository veiculoRepository,
        IRepository<Pessoa> pessoaRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateVeiculoCommandHandler> logger)
    {
        _veiculoRepository = veiculoRepository;
        _pessoaRepository = pessoaRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(UpdateVeiculoCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Iniciando atualização do veículo. Id: {VeiculoId}", command.Id);

            var placa = new Placa(command.Placa);

            var veiculo = await _veiculoRepository.GetByIdAsync(command.Id, cancellationToken);

            if (veiculo is null || veiculo.EstaExcluida())
                return Result.Failure("Veículo não encontrado.");

            var pessoa = await _pessoaRepository.GetByIdAsync(command.PessoaId, cancellationToken);

            if (pessoa is null || pessoa.EstaExcluida())
                return Result.Failure("Pessoa não encontrada.");

            var placaDuplicada = (await _veiculoRepository.GetAllWithPessoaAsync(cancellationToken))
                .FirstOrDefault(v => v.Id != command.Id && v.Placa.Numero == placa.Numero);

            if (placaDuplicada is not null)
                return Result.Failure("Já existe outro veículo cadastrado com esta placa.");

            veiculo.AlterarPessoa(command.PessoaId);
            veiculo.AlterarPlaca(placa);
            veiculo.AlterarMarca(command.Marca);
            veiculo.AlterarModelo(command.Modelo);
            veiculo.AlterarAnoFabricacao(command.AnoFabricacao);
            veiculo.AlterarCor(command.Cor);
            veiculo.AlterarObservacoes(command.Observacoes);
            veiculo.AlterarHodometro(new Hodometro(command.Hodometro));

            if (command.Ativo)
                veiculo.Ativar();
            else
                veiculo.Desativar();

            await _veiculoRepository.UpdateAsync(veiculo, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domínio ao atualizar veículo.");
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar veículo.");
            return Result.Failure("Não foi possível atualizar o veículo.");
        }
    }
}
