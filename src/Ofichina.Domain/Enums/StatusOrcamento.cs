namespace Ofichina.Domain.Enums;

/// <summary>
/// Define os possíveis estados de um orçamento.
/// Mantém Recebida para compatibilidade do ciclo atual.
/// </summary>
public enum StatusOrcamento
{
    Criado = 1,
    Recebida = 2,
    EmDiagnostico = 3,
    AguardandoAprovacao = 4,
    Aprovado = 5,
    Reprovado = 6
}
