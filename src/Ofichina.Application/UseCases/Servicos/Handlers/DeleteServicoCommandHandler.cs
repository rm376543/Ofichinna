using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Servicos.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Interfaces;

namespace Ofichina.Application.UseCases.Servicos.Handlers;

/// <summary>
/// Handler para exclusão lógica de serviço.
/// </summary>
public sealed class DeleteServicoCommandHandler : ICommandHandler<DeleteServicoCommand, Result>
{
    private readonly IRepository<Servico> _servicoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteServicoCommandHandler> _logger;

    public DeleteServicoCommandHandler(
        IRepository<Servico> servicoRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteServicoCommandHandler> logger)
    {
        _servicoRepository = servicoRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(DeleteServicoCommand command)
    {
        try
        {
            var servico = await _servicoRepository.GetByIdAsync(command.Id);

            if (servico is null || servico.EstaExcluida())
                return Result.Failure("Serviço não encontrado.");

            servico.Desativar();
            servico.Excluir();

            await _servicoRepository.UpdateAsync(servico);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao remover serviço. ServicoId: {ServicoId}", command.Id);
            return Result.Failure("Não foi possível remover o serviço.");
        }
    }
}