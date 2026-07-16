using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Servicos.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Exceptions;
using Ofichina.Domain.Interfaces;

namespace Ofichina.Application.UseCases.Servicos.Handlers;

/// <summary>
/// Handler para criação de serviço.
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

    public async Task<Result<Guid>> HandleAsync(CreateServicoCommand command)
    {
        try
        {
            var servico = new Servico(command.Nome, command.Descricao, command.Valor, command.Ativo);

            await _servicoRepository.AddAsync(servico);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success(servico.Id);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domínio ao criar serviço.");
            return Result.Failure<Guid>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar serviço.");
            return Result.Failure<Guid>("Não foi possível criar o serviço.");
        }
    }
}