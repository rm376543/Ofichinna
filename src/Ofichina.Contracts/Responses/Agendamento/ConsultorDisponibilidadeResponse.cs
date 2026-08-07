using Ofichina.Contracts.Requests;

namespace Ofichina.Contracts.Responses.Agendamento;

/// <summary>
/// Response com informações do consultor disponível.
/// </summary>
public sealed class ConsultorDisponibilidadeResponse : BaseRequest
{
    public Guid ConsultorDisponibilidadeId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Documento { get; set; } = string.Empty;
}
