namespace Ofichina.Domain.Enums;

/// <summary>
/// Define os possíveis estados de um orçamento.
/// </summary>
public enum StatusOrcamento
{
    EmDiagnostico = 1,
    AguardandoAprovacao = 2,
    Aprovado = 3,
    Reprovado = 4
}
