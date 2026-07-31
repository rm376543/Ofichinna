namespace Ofichina.Contracts.Responses.OrdemServico;

/// <summary>
/// Resposta com os dados de um item de serviço vinculado à ordem de serviço.
/// </summary>
public class OrdemServicoItensResponse
{
    public Guid OrdemServicoId { get; set; }

    public List<ServicoItemResponse> Servicos { get; set; } = [];
}

/// <summary>
/// Resposta com os dados de um serviço vinculado a um item de serviço da ordem de serviço.
/// </summary>
public class ServicoItemResponse
{
    public Guid ServicoId { get; set; }

    public string Descricao { get; set; } = string.Empty;

    public decimal ValorServico { get; set; }

    public decimal ValorTotal { get; set; }

    public List<PecaItemResponse> Pecas { get; set; } = [];
}

/// <summary>
/// Resposta com os dados de uma peça vinculada a um item de serviço da ordem de serviço.
/// </summary>
public class PecaItemResponse
{
    public Guid PecaId { get; set; }

    public string Descricao { get; set; } = string.Empty;

    public int Quantidade { get; set; }

    public decimal ValorUnitario { get; set; }

    public decimal ValorTotal { get; set; }
}