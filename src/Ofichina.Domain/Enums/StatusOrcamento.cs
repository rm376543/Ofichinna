namespace Ofichina.Domain.Enums;

/// <summary>
/// Define os possíveis estados de um orçamento.
/// </summary>
public enum StatusOrcamento
{
    Criado = 1,
    EmDiagnostico = 2,
    AguardandoEnvio = 3,
    AguardandoAprovacao = 4,
    Aprovado = 5,
    Reprovado = 6
}
