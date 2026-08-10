namespace Ofichina.Domain.Enums;

/// <summary>
/// Define os possíveis estados de uma ordem de serviço.
/// </summary>
public enum StatusOrdemServico
{
    Criado = 1,
    Recebida = 2,
    EmExecucao = 3,
    Finalizada = 4,
    Entregue = 5,
    Cancelada = 6
}