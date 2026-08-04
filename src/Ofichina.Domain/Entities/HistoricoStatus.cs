using Ofichina.Domain.Common;
using Ofichina.Domain.Exceptions;
using Ofichina.Domain.Aggregates;

namespace Ofichina.Domain.Entities;

/// <summary>
/// Histórico genérico de mudanças de status de entidades do domínio.
/// </summary>
public sealed class HistoricoStatus : Entity
{
    public Guid EntidadeId { get; private set; }

    public Guid? OrcamentoId { get; private set; }

    public Orcamento? Orcamento { get; private set; }

    public Guid? OrdemServicoId { get; private set; }

    public OrdemServico? OrdemServico { get; private set; }

    public string TipoEntidade { get; private set; } = string.Empty;

    public string? StatusAnterior { get; private set; }

    public string StatusNovo { get; private set; } = string.Empty;

    public DateTime AlteradoEm { get; private set; }

    public Guid? AlteradoPor { get; private set; }

    private HistoricoStatus()
    {
    }

    public static HistoricoStatus ParaOrcamento(
        Guid orcamentoId,
        string? statusAnterior,
        string statusNovo,
        Guid? alteradoPor,
        DateTime? alteradoEm = null)
    {
        return new HistoricoStatus(
            orcamentoId,
            nameof(Orcamento),
            statusAnterior,
            statusNovo,
            alteradoPor,
            alteradoEm)
        {
            OrcamentoId = orcamentoId
        };
    }

    public static HistoricoStatus ParaOrdemServico(
        Guid ordemServicoId,
        string? statusAnterior,
        string statusNovo,
        Guid? alteradoPor,
        DateTime? alteradoEm = null)
    {
        return new HistoricoStatus(
            ordemServicoId,
            nameof(OrdemServico),
            statusAnterior,
            statusNovo,
            alteradoPor,
            alteradoEm)
        {
            OrdemServicoId = ordemServicoId
        };
    }

    public HistoricoStatus(
        Guid entidadeId,
        string tipoEntidade,
        string? statusAnterior,
        string statusNovo,
        Guid? alteradoPor,
        DateTime? alteradoEm = null)
    {
        if (entidadeId == Guid.Empty)
            throw new DomainException("Entidade obrigatória para o histórico.");

        if (string.IsNullOrWhiteSpace(tipoEntidade))
            throw new DomainException("Tipo da entidade obrigatório para o histórico.");

        if (string.IsNullOrWhiteSpace(statusNovo))
            throw new DomainException("Status novo obrigatório para o histórico.");

        EntidadeId = entidadeId;
        TipoEntidade = tipoEntidade.Trim();
        StatusAnterior = string.IsNullOrWhiteSpace(statusAnterior) ? null : statusAnterior.Trim();
        StatusNovo = statusNovo.Trim();
        AlteradoPor = alteradoPor;
        AlteradoEm = alteradoEm ?? DateTime.UtcNow;
    }
}