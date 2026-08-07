using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Perfis.Commands;
using Ofichina.Contracts.Common;

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

    public async Task<Result> HandleAsync(UpdatePerfilCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Iniciando atualização do perfil com Id: {PerfilId}", command.PerfilId);

            var perfil = await _repository.GetByIdAsync(command.PerfilId, cancellationToken);

            if (perfil is null)
            {
                _logger.LogWarning("Perfil com Id: {PerfilId} não encontrado.", command.PerfilId);
                return Result.Failure("Perfil não encontrado.");
            }

            var nomeExistente = await _repository.GetByNomeAsync(command.NomePerfil, cancellationToken);

            if (nomeExistente is not null && nomeExistente.Id != command.PerfilId)
            {
                _logger.LogWarning("Já existe um perfil com o nome: {NomePerfil}", command.NomePerfil);
                return Result.Failure("Já existe um perfil com este nome.");
            }

            perfil.AlterarNome(command.NomePerfil);
            perfil.AlterarDescricao(command.Descricao);

            await _repository.UpdateAsync(perfil, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Perfil com Id: {PerfilId} atualizado com sucesso.", command.PerfilId);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar o perfil com Id: {PerfilId}", command.PerfilId);
            return Result.Failure("Ocorreu um erro ao atualizar o perfil.");
        }

    }
}
