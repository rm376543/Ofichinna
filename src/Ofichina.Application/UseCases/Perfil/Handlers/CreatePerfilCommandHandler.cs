using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Perfis.Commands;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Interfaces;

namespace Ofichina.Application.UseCases.Perfis.Handlers;

public class CreatePerfilCommandHandler : ICommandHandler<CreatePerfilCommand, Guid>
{
    private readonly IPerfilRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePerfilCommandHandler(
        IPerfilRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> HandleAsync(CreatePerfilCommand command)
    {
        var existente = await _repository.GetByNomeAsync(command.NomePerfil);

        if (existente is not null)
        {
            throw new InvalidOperationException("Já existe um perfil com este nome.");
        }

        var perfil = new Perfil(command.NomePerfil, command.Descricao);
        perfil.DeletedAt = null;

        await _repository.AddAsync(perfil);
        await _unitOfWork.SaveChangesAsync();
        return perfil.Id;
    }
}