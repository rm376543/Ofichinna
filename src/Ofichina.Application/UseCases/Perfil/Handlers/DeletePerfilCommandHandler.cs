using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Perfil.Commands;
using Ofichina.Domain.Interfaces;
using Ofichina.Domain.ValueObjects;

namespace Ofichina.Application.UseCases.Perfil.Handlers;

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

        perfil.Ativo = false;

        await _repository.UpdateAsync(perfil);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}