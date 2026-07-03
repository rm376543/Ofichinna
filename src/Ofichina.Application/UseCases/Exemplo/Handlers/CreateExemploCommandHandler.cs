using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Exemplo.Commands;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Interfaces;

namespace Ofichina.Application.UseCases.Exemplo.Handlers;

/// <summary>
/// Handler para criar um novo Exemplo.
/// </summary>
public class CreateExemploCommandHandler : ICommandHandler<CreateExemploCommand, Guid>
{
    private readonly IExemploRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateExemploCommandHandler(
        IExemploRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> HandleAsync(CreateExemploCommand command)
    {
        var exemplo = new Domain.Entities.Exemplo(command.Nome, command.Descricao);

        await _repository.AddAsync(exemplo);
        await _unitOfWork.SaveChangesAsync();

        return exemplo.Id;
    }
}
