using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Perfis.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Interfaces;
using Ofichina.Domain.ValueObjects;

namespace Ofichina.Application.UseCases.Perfis.Handlers;

/// <summary>
/// Handler para atualizar um perfil.
/// </summary>
public class UpdatePerfilCommandHandler : ICommandHandler<UpdatePerfilCommand, Result>
{
    private readonly IPerfilRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdatePerfilCommandHandler> _logger;

    public UpdatePerfilCommandHandler(
        IPerfilRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<UpdatePerfilCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(UpdatePerfilCommand command)
    {
        try
        {
            _logger.LogInformation("Iniciando atualização do perfil com Id: {PerfilId}", command.Id);

            var perfil = await _repository.GetByIdAsync(command.Id);

            if (perfil is null)
            {
                _logger.LogWarning("Perfil com Id: {PerfilId} não encontrado.", command.Id);
                return Result.Failure("Perfil não encontrado.");
            }

            var nomeExistente = await _repository.GetByNomeAsync(command.NomePerfil);

            if (nomeExistente is not null && nomeExistente.Id != command.Id)
            {
                _logger.LogWarning("Já existe um perfil com o nome: {NomePerfil}", command.NomePerfil);
                return Result.Failure("Já existe um perfil com este nome.");
            }

            perfil.AlterarNome(command.NomePerfil);
            perfil.AlterarDescricao(command.Descricao);

            await _repository.UpdateAsync(perfil);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Perfil com Id: {PerfilId} atualizado com sucesso.", command.Id);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar o perfil com Id: {PerfilId}", command.Id);
            return Result.Failure("Ocorreu um erro ao atualizar o perfil.");
        }

    }
}