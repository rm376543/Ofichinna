using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Veiculos.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Interfaces;

namespace Ofichina.Application.UseCases.Veiculos.Handlers;

/// <summary>
/// Handler para remoção lógica de veículo.
/// </summary>
public sealed class DeleteVeiculoCommandHandler : ICommandHandler<DeleteVeiculoCommand, Result>
{
    private readonly IVeiculoRepository _veiculoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteVeiculoCommandHandler> _logger;

    public DeleteVeiculoCommandHandler(
        IVeiculoRepository veiculoRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteVeiculoCommandHandler> logger)
    {
        _veiculoRepository = veiculoRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(DeleteVeiculoCommand command)
    {
        try
        {
            var veiculo = await _veiculoRepository.GetByIdAsync(command.Id);

            if (veiculo is null || veiculo.EstaExcluida())
                return Result.Failure("Veículo não encontrado.");

            veiculo.Desativar();
            veiculo.Excluir();

            await _veiculoRepository.UpdateAsync(veiculo);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao remover veículo. VeiculoId: {VeiculoId}", command.Id);
            return Result.Failure("Não foi possível remover o veículo.");
        }
    }
}