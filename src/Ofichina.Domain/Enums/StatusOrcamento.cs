namespace Ofichina.Domain.Enums;

/// <summary>
/// Define os possíveis estados de um orçamento.
/// </summary>
public enum StatusOrcamento
{
    Criado = 1,
    EmDiagnostico = 2,
    AguardandoAprovacao = 3,
    Aprovado = 4,
    Reprovado = 5
}
