using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Perfis.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Interfaces;

namespace Ofichina.Application.UseCases.Perfis.Handlers;

/// <summary>
/// Handler para desativar um perfil.
/// </summary>
public class DeletePerfilCommandHandler : ICommandHandler<DeletePerfilCommand>
{
    private readonly IPerfilRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeletePerfilCommandHandler(
        IPerfilRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(DeletePerfilCommand command)
    {
        var perfil = await _repository.GetByIdAsync(command.Id);

        if (perfil is null)
        {
            return Result.Failure("Perfil não encontrado.");
        }

        perfil.DeletedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(perfil);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}