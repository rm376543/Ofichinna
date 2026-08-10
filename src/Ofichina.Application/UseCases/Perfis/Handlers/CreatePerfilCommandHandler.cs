using Ofichina.Application.Abstractions;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Application.UseCases.Perfis.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Exceptions;

namespace Ofichina.Application.UseCases.Perfis.Handlers;

public sealed class CreatePerfilCommandHandler : ICommandHandler<CreatePerfilCommand, Result>
{
    private readonly IPerfilRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreatePerfilCommandHandler> _logger;

    public CreatePerfilCommandHandler(
        IPerfilRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<CreatePerfilCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(CreatePerfilCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Iniciando criação de perfil: [Nome] {NomePerfil}", command.NomePerfil);

            var existente = await _repository.GetByNomeAsync(command.NomePerfil, cancellationToken);

            if (existente is not null)
            {
                _logger.LogWarning("Já existe um perfil com este nome: {NomePerfil}", command.NomePerfil);
                return Result.Failure("Já existe um perfil com este nome.");
            }

            var perfil = new Perfil(command.NomePerfil, command.Descricao);

            await _repository.AddAsync(perfil, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Perfil criado com sucesso: [PerfilId] {PerfilId}, [Nome] {NomePerfil}", perfil.Id, perfil.NomePerfil);
            return Result.Success();
        }
        catch (DomainException ex)
        {
            _logger.LogError(ex, "Erro de domínio ao criar perfil.");
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar perfil.");
            return Result.Failure("Ocorreu um erro ao criar o perfil.");
        }

    }
}
