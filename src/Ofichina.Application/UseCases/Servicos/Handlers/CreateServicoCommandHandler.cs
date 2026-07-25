using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Servicos.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Exceptions;
using Ofichina.Domain.Common;

namespace Ofichina.Application.UseCases.Servicos.Handlers;

/// <summary>
/// Handler para criação de serviço.
/// </summary>
public sealed class CreateServicoCommandHandler : ICommandHandler<CreateServicoCommand, Result>
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

    public async Task<Result> HandleAsync(CreateServicoCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var servico = new Servico(command.Nome, command.Descricao, command.Valor);

            await _servicoRepository.AddAsync(servico, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domínio ao criar serviço.");
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar serviço.");
            return Result.Failure("Não foi possível criar o serviço.");
        }
    }
}
