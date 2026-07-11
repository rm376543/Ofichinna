using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Perfis.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Interfaces;
using Ofichina.Domain.ValueObjects;

namespace Ofichina.Application.UseCases.Perfis.Handlers;

/// <summary>
/// Handler para atualizar um perfil.
/// </summary>
public class UpdatePerfilCommandHandler : ICommandHandler<UpdatePerfilCommand>
{
    private readonly IPerfilRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePerfilCommandHandler(
        IPerfilRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(UpdatePerfilCommand command)
    {
        var perfil = await _repository.GetByIdAsync(command.Id);

        if (perfil is null)
        {
            return Result.Failure("Perfil não encontrado.");
        }

        var nomeExistente = await _repository.GetByNomeAsync(command.NomePerfil);

        if (nomeExistente is not null && nomeExistente.Id != command.Id)
        {
            return Result.Failure("Já existe um perfil com este nome.");
        }

        perfil.NomePerfil = command.NomePerfil;
        perfil.Descricao = command.Descricao;

        await _repository.UpdateAsync(perfil);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}