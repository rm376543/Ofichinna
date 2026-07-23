using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.Abstractions.Interfaces;
using Ofichina.Application.UseCases.ServicosPecas.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Domain.Common;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Exceptions;

namespace Ofichina.Application.UseCases.ServicosPecas.Handlers;

/// <summary>
/// Handler para adicionar uma peça a um serviço.
/// </summary>
public sealed class CreateServicoPecaCommandHandler : ICommandHandler<CreateServicoPecaCommand, Result<Guid>>
{
    private readonly IServicoRepository _servicoRepository;
    private readonly IRepository<Peca> _pecaRepository;
    private readonly IServicoPecasRepository _servicoPecasRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateServicoPecaCommandHandler> _logger;

    public CreateServicoPecaCommandHandler(
        IServicoRepository servicoRepository,
        IRepository<Peca> pecaRepository,
        IServicoPecasRepository servicoPecasRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateServicoPecaCommandHandler> logger)
    {
        _servicoRepository = servicoRepository;
        _pecaRepository = pecaRepository;
        _servicoPecasRepository = servicoPecasRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> HandleAsync(CreateServicoPecaCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Iniciando inclusão de peça no serviço. ServicoId: {ServicoId}, PecaId: {PecaId}, Quantidade: {Quantidade}.", command.ServicoId, command.PecaId, command.Quantidade);

            var servico = await _servicoRepository.GetByIdAsync(command.ServicoId, cancellationToken: cancellationToken);
            if (servico is null || servico.EstaExcluida())
                return Result.Failure<Guid>("Serviço não encontrado.");

            var peca = await _pecaRepository.GetByIdAsync(command.PecaId, cancellationToken);
            if (peca is null || peca.EstaExcluida())
                return Result.Failure<Guid>("Peça não encontrada.");

            var pecaServico = await _servicoPecasRepository.AdicionarAsync(
                command.ServicoId,
                command.PecaId,
                command.Quantidade,
                cancellationToken);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Peça adicionada ao serviço com sucesso. ServicoId: {ServicoId}, PecaId: {PecaId}, ServicoPecaId: {ServicoPecaId}.", command.ServicoId, command.PecaId, pecaServico.Id);
            return Result.Success(pecaServico.Id);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domínio ao adicionar peça ao serviço. ServicoId: {ServicoId}, PecaId: {PecaId}.", command.ServicoId, command.PecaId);
            return Result.Failure<Guid>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao adicionar peça ao serviço. ServicoId: {ServicoId}, PecaId: {PecaId}.", command.ServicoId, command.PecaId);
            return Result.Failure<Guid>("Não foi possível adicionar a peça ao serviço.");
        }
    }
}