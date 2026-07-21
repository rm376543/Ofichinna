using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.OrdensServico.Commands;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.OrdemServico;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Exceptions;
using Ofichina.Domain.Common;
using DomainPeca = Ofichina.Domain.Entities.Peca;

namespace Ofichina.Application.UseCases.OrdensServico.Handlers;

/// <summary>
/// Handler para atualizacao de ordem de servico.
/// </summary>
public sealed class UpdateOrdemServicoCommandHandler : ICommandHandler<UpdateOrdemServicoCommand, Result>
{
    private readonly IRepository<OrdemServico> _ordemServicoRepository;
    private readonly IRepository<Servico> _servicoRepository;
    private readonly IRepository<DomainPeca> _pecaRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateOrdemServicoCommandHandler> _logger;

    public UpdateOrdemServicoCommandHandler(
        IRepository<OrdemServico> ordemServicoRepository,
        IRepository<Servico> servicoRepository,
        IRepository<DomainPeca> pecaRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateOrdemServicoCommandHandler> logger)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _servicoRepository = servicoRepository;
        _pecaRepository = pecaRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(UpdateOrdemServicoCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Iniciando atualização da ordem de serviço. OrdemServicoId: {OrdemServicoId}.", command.Id);

            var ordemServico = await _ordemServicoRepository.GetByIdAsync(command.Id, cancellationToken);
            if (ordemServico is null || ordemServico.EstaExcluida())
                return Result.Failure("Ordem de serviço não encontrada.");

            ordemServico.AtualizarAtendimento(command.FuncionarioId, command.Observacoes);

            await _ordemServicoRepository.UpdateAsync(ordemServico, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Ordem de serviço atualizada com sucesso. OrdemServicoId: {OrdemServicoId}", command.Id);
            return Result.Success();
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domínio ao atualizar ordem de serviço. OrdemServicoId: {OrdemServicoId}", command.Id);
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao atualizar ordem de serviço. OrdemServicoId: {OrdemServicoId}", command.Id);
            return Result.Failure("Não foi possível atualizar a ordem de serviço.");
        }
    }

    private async Task ReconciliarServicos(
        OrdemServico ordemServico,
        ICollection<UpdateOrdemServicoItemServicoRequest> servicos,
        CancellationToken cancellationToken)
    {
        var servicosExistentes = ordemServico.Servicos
            .Where(x => !x.EstaExcluida())
            .ToDictionary(x => x.Id, x => x);

        foreach (var existente in servicosExistentes.Values)
        {
            if (!servicos.Any(x => x.Id == existente.Id))
                existente.Excluir();
        }

        foreach (var itemServicoRequest in servicos)
        {
            var servico = await _servicoRepository.GetByIdAsync(itemServicoRequest.ServicoId, cancellationToken);
            if (servico is null || servico.EstaExcluida())
                throw new DomainException("Serviço não encontrado.");

            if (itemServicoRequest.Id != Guid.Empty && servicosExistentes.TryGetValue(itemServicoRequest.Id, out var itemServico))
            {
                itemServico.AtualizarServico(servico.Id, servico.Nome, servico.Valor);
                await ReconciliarPecasAsync(itemServico, itemServicoRequest.Pecas, cancellationToken);
                continue;
            }

            var novoServico = ordemServico.AdicionarServico(servico.Id, servico.Nome, servico.Valor);
            await ReconciliarPecasAsync(novoServico, itemServicoRequest.Pecas, cancellationToken);
        }
    }

    private async Task ReconciliarPecasAsync(
        Ofichina.Domain.Entities.ItemServico itemServico,
        ICollection<UpdateOrdemServicoPecaRequest> pecas,
        CancellationToken cancellationToken)
    {
        var pecasExistentes = itemServico.Pecas
            .Where(x => !x.EstaExcluida())
            .ToDictionary(x => x.Id, x => x);

        foreach (var existente in pecasExistentes.Values)
        {
            if (!pecas.Any(x => x.Id == existente.Id))
                itemServico.RemoverPeca(existente.Id);
        }

        foreach (var pecaRequest in pecas)
        {
            var pecaCatalogo = await _pecaRepository.GetByIdAsync(pecaRequest.PecaId, cancellationToken);
            if (pecaCatalogo is null || pecaCatalogo.EstaExcluida())
                throw new DomainException("Peça não encontrada.");

            if (pecaRequest.Id != Guid.Empty && pecasExistentes.ContainsKey(pecaRequest.Id))
            {
                itemServico.AtualizarPeca(pecaRequest.Id, pecaCatalogo.Id, pecaCatalogo.Nome, pecaRequest.Quantidade, pecaCatalogo.Valor);
                continue;
            }

            itemServico.AdicionarPeca(pecaCatalogo.Id, pecaCatalogo.Nome, pecaRequest.Quantidade, pecaCatalogo.Valor);
        }
    }
}

