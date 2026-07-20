using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Veiculos.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Application.Abstractions.Interfaces;
using Ofichina.Domain.Common;

namespace Ofichina.Application.UseCases.Veiculos.Handlers;

/// <summary>
/// Handler para remocao logica de veiculo.
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

    public async Task<Result> HandleAsync(DeleteVeiculoCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var veiculo = await _veiculoRepository.GetByIdAsync(command.Id, cancellationToken);

            if (veiculo is null || veiculo.EstaExcluida())
                return Result.Failure("VeÃ­culo nÃ£o encontrado.");

            veiculo.Desativar();

            await _veiculoRepository.UpdateAsync(veiculo, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao remover veÃ­culo. VeiculoId: {VeiculoId}", command.Id);
            return Result.Failure("NÃ£o foi possÃ­vel remover o veÃ­culo.");
        }
    }
}
