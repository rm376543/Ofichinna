using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Perfis.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Interfaces;

namespace Ofichina.Application.UseCases.Perfis.Handlers;

/// <summary>
/// Handler para desativar um perfil.
/// </summary>
public class DeletePerfilCommandHandler : ICommandHandler<DeletePerfilCommand, Result>
{
    private readonly IPerfilRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeletePerfilCommandHandler> _logger;

    public DeletePerfilCommandHandler(
        IPerfilRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<DeletePerfilCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(DeletePerfilCommand command)
    {
        try
        {
            _logger.LogInformation("Iniciando desativação do perfil com Id {PerfilId}.", command.Id);
            var perfil = await _repository.GetByIdAsync(command.Id);

            if (perfil is null)
            {
                _logger.LogWarning("Perfil com Id {PerfilId} não encontrado.", command.Id);
                return Result.Failure("Perfil não encontrado.");
            }

            perfil.Excluir();

            await _repository.UpdateAsync(perfil);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Perfil com Id {PerfilId} desativado com sucesso.", command.Id);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao desativar o perfil com ID {PerfilId}.", command.Id);
            return Result.Failure("Ocorreu um erro ao desativar o perfil.");
        }

    }
}