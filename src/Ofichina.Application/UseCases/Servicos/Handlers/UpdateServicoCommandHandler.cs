using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Servicos.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Exceptions;
using Ofichina.Domain.Common;

namespace Ofichina.Application.UseCases.Servicos.Handlers;

/// <summary>
/// Handler para atualização de serviço.
/// </summary>
public sealed class UpdateServicoCommandHandler : ICommandHandler<UpdateServicoCommand, Result>
{
    private readonly IRepository<Servico> _servicoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateServicoCommandHandler> _logger;

    public UpdateServicoCommandHandler(
        IRepository<Servico> servicoRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateServicoCommandHandler> logger)
    {
        _servicoRepository = servicoRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(UpdateServicoCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var servico = await _servicoRepository.GetByIdAsync(command.Id, cancellationToken);

            if (servico is null || servico.EstaExcluida())
                return Result.Failure("Serviço não encontrado.");

            servico.AtualizarDados(command.Nome, command.Descricao, command.Valor);

            if (command.Ativo)
                servico.Ativar();
            else
                servico.Desativar();

            await _servicoRepository.UpdateAsync(servico, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domínio ao atualizar serviço.");
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar serviço.");
            return Result.Failure("Não foi possível atualizar o serviço.");
        }
    }
}
