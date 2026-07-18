using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Servicos.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Exceptions;
using Ofichina.Domain.Interfaces;

namespace Ofichina.Application.UseCases.Servicos.Handlers;

/// <summary>
/// Handler para criaÃ§Ã£o de serviÃ§o.
/// </summary>
public sealed class CreateServicoCommandHandler : ICommandHandler<CreateServicoCommand, Result<Guid>>
{
    private readonly IRepository<Servico> _servicoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateServicoCommandHandler> _logger;

    public CreateServicoCommandHandler(
        IRepository<Servico> servicoRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateServicoCommandHandler> logger)
    {
        _servicoRepository = servicoRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> HandleAsync(CreateServicoCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var servico = new Servico(command.Nome, command.Descricao, command.Valor);

            if (!command.Ativo)
                servico.Desativar();

            await _servicoRepository.AddAsync(servico, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success(servico.Id);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domÃ­nio ao criar serviÃ§o.");
            return Result.Failure<Guid>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar serviÃ§o.");
            return Result.Failure<Guid>("NÃ£o foi possÃ­vel criar o serviÃ§o.");
        }
    }
}
