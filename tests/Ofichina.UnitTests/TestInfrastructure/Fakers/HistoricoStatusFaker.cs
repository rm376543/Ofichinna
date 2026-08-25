using Bogus;
using Ofichina.Domain.Entities;

namespace Ofichina.UnitTests.TestInfrastructure.Fakers;

public class HistoricoStatusFaker
{
    private readonly Faker _faker = new();

    public HistoricoStatus ParaOrcamento(Guid? orcamentoId = null, Guid? alteradoPor = null, string? statusAnterior = null, string? statusNovo = null)
    {
        var oid = orcamentoId ?? Guid.NewGuid();
        var alterador = alteradoPor ?? Guid.NewGuid();
        var antigo = statusAnterior ?? "Criado";
        var novo = statusNovo ?? "Aprovado";

        return HistoricoStatus.ParaOrcamento(oid, antigo, novo, alterador, DateTime.UtcNow);
    }

    public HistoricoStatus ParaOrdemServico(Guid? ordemServicoId = null, Guid? alteradoPor = null, string? statusAnterior = null, string? statusNovo = null)
    {
        var oid = ordemServicoId ?? Guid.NewGuid();
        var alterador = alteradoPor ?? Guid.NewGuid();
        var antigo = statusAnterior ?? "Recebida";
        var novo = statusNovo ?? "EmExecucao";

        return HistoricoStatus.ParaOrdemServico(oid, antigo, novo, alterador, DateTime.UtcNow);
    }
}
