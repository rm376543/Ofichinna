using Ofichina.Domain.Exceptions;

namespace Ofichina.Domain.Entities;

/// <summary>
/// Representa o checklist de entrada do veículo.
/// </summary>
public class Checklist : Entity
{
    public Guid VeiculoId { get; private set; }

    public Veiculo? Veiculo { get; private set; }

    public Guid PessoaId { get; private set; }

    public Pessoa? Pessoa { get; private set; }

    public Guid? AgendamentoId { get; private set; }

    public int HodometroEntrada { get; private set; }

    public string ItensVerificados { get; private set; } = string.Empty;

    public string? Observacoes { get; private set; }

    public bool Finalizado { get; private set; }

    private Checklist()
    {
    }

    public Checklist(Guid veiculoId, Guid pessoaId, int hodometroEntrada, string itensVerificados, string? observacoes)
    {
        if (veiculoId == Guid.Empty)
            throw new DomainException("Veículo obrigatório.");

        if (pessoaId == Guid.Empty)
            throw new DomainException("Pessoa obrigatória.");

        if (hodometroEntrada < 0)
            throw new DomainException("O hodômetro de entrada não pode ser negativo.");

        VeiculoId = veiculoId;
        PessoaId = pessoaId;
        HodometroEntrada = hodometroEntrada;
        ItensVerificados = itensVerificados ?? string.Empty;
        Observacoes = observacoes;
        Finalizado = false;
    }

    public void Finalizar()
    {
        if (Finalizado)
            throw new DomainException("O checklist já foi finalizado.");

        Finalizado = true;
        AtualizarDataModificacao();
    }

    public void VincularAgendamento(Guid agendamentoId)
    {
        if (agendamentoId == Guid.Empty)
            throw new DomainException("O agendamento deve ser informado.");

        AgendamentoId = agendamentoId;
        AtualizarDataModificacao();
    }
}
