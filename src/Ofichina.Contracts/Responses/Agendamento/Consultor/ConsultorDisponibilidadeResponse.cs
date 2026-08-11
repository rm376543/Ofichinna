using Ofichina.Contracts.Common;

namespace Ofichina.Contracts.Responses.Agendamento.Consultor;

/// <summary>
/// Response com informações do consultor disponível.
/// </summary>
public sealed class ConsultorDisponibilidadeResponse : BaseRequest
{
    public Guid ConsultorId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Documento { get; set; } = string.Empty;
}
