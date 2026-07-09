using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Perfis.Commands;
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

        var codigoExistente = await _repository.GetByCodigoAsync(command.Codigo);

        if (codigoExistente is not null && codigoExistente.Id != command.Id)
        {
            return Result.Failure("Já existe um perfil com este código.");
        }

        perfil.Codigo = command.Codigo;
        perfil.Nome = command.Nome;
        perfil.Descricao = command.Descricao;
        perfil.Ativo = command.Ativo;

        await _repository.UpdateAsync(perfil);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}