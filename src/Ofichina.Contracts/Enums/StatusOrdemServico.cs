namespace Ofichina.Contracts.Enums;

/// <summary>
/// Define os possíveis estados de uma ordem de serviço.
/// </summary>
public enum StatusOrdemServico
{
    Recebida = 1,
    EmExecucao = 2,
    Finalizada = 3,
    Entregue = 4,
    Cancelada = 5
}