namespace Ofichina.Domain.Enums;

/// <summary>
/// Define os possíveis estados de uma ordem de serviço.
/// </summary>
public enum StatusOrdemServico
{
    Recebida = 1,
    EmDiagnostico = 2,
    AguardandoAprovacao = 3,
    EmExecucao = 4,
    Finalizada = 5,
    Entregue = 6,
    Cancelada = 7
}