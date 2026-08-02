using Ofichina.Domain.Entities;
using Ofichina.Domain.Enums;
using Ofichina.Domain.Exceptions;

namespace Ofichina.Domain.Aggregates;

/// <summary>
/// Representa o orçamento da oficina e seu ciclo de aprovação.
/// </summary>
public class Orcamento : Entity
{
    private readonly List<ItemOrcamento> _itensPrevistos = [];

    public Guid PessoaId { get; private set; }

    public Guid VeiculoId { get; private set; }

    public Guid MecanicoDiagnosticoId { get; private set; }

    public Guid ResponsavelId { get; private set; }

    public DateTime DataValidade { get; private set; }

    public decimal Desconto { get; private set; }

    public string? Observacoes { get; private set; }

    public StatusOrcamento Status { get; private set; }

    public DateTime DataCriacao => CreatedAt;

    public Checklist? Checklist { get; private set; }

    public IReadOnlyCollection<ItemOrcamento> ItensPrevistos => _itensPrevistos.AsReadOnly();

    private Orcamento()
    {
    }

    public Orcamento(
        Guid pessoaId,
        Guid veiculoId,
        Guid mecanicoDiagnosticoId,
        Guid responsavelId,
        DateTime dataValidade,
        decimal desconto,
        string? observacoes)
    {
        ValidarIdentificador(pessoaId, "Pessoa obrigatória.");
        ValidarIdentificador(veiculoId, "Veículo obrigatório.");
        ValidarIdentificador(mecanicoDiagnosticoId, "Mecânico do diagnóstico obrigatório.");
        ValidarIdentificador(responsavelId, "Responsável obrigatório.");

        if (desconto < 0)
            throw new DomainException("O desconto não pode ser negativo.");

        if (dataValidade == default)
            throw new DomainException("A data de validade é obrigatória.");

        PessoaId = pessoaId;
        VeiculoId = veiculoId;
        MecanicoDiagnosticoId = mecanicoDiagnosticoId;
        ResponsavelId = responsavelId;
        DataValidade = dataValidade;
        Desconto = desconto;
        Observacoes = observacoes;
        Status = StatusOrcamento.EmDiagnostico;
    }

    public void EnviarParaCliente()
    {
        ValidarStatus(StatusOrcamento.EmDiagnostico);

        Status = StatusOrcamento.AguardandoAprovacao;
        AtualizarDataModificacao();
    }

    public void Aprovar()
    {
        ValidarStatus(StatusOrcamento.AguardandoAprovacao);

        Status = StatusOrcamento.Aprovado;
        AtualizarDataModificacao();
    }

    public void Reprovar()
    {
        ValidarStatus(StatusOrcamento.AguardandoAprovacao);

        Status = StatusOrcamento.Reprovado;
        AtualizarDataModificacao();
    }

    public void AtualizarDados(
        Guid pessoaId,
        Guid veiculoId,
        Guid mecanicoDiagnosticoId,
        Guid responsavelId,
        DateTime dataValidade,
        decimal desconto,
        string? observacoes)
    {
        ValidarIdentificador(pessoaId, "Pessoa obrigatória.");
        ValidarIdentificador(veiculoId, "Veículo obrigatório.");
        ValidarIdentificador(mecanicoDiagnosticoId, "Mecânico do diagnóstico obrigatório.");
        ValidarIdentificador(responsavelId, "Responsável obrigatório.");

        if (desconto < 0)
            throw new DomainException("O desconto não pode ser negativo.");

        if (dataValidade == default)
            throw new DomainException("A data de validade é obrigatória.");

        PessoaId = pessoaId;
        VeiculoId = veiculoId;
        MecanicoDiagnosticoId = mecanicoDiagnosticoId;
        ResponsavelId = responsavelId;
        DataValidade = dataValidade;
        Desconto = desconto;
        Observacoes = observacoes;

        AtualizarDataModificacao();
    }

    public ItemOrcamento AdicionarServico(Guid servicoId, Guid pecaId, int quantidade)
    {
        ValidarAlteracaoItens();

        var item = new ItemOrcamento(Id, servicoId, pecaId, quantidade);
        _itensPrevistos.Add(item);

        AtualizarDataModificacao();

        return item;
    }

    public void AtualizarServico(Guid itemOrcamentoId, Guid servicoId, Guid pecaId, int quantidade)
    {
        ValidarAlteracaoItens();

        var item = ObterServico(itemOrcamentoId);

        if (item is null || item.EstaExcluida())
            throw new DomainException("Serviço não encontrado.");

        item.AtualizarDados(servicoId, pecaId, quantidade);
        AtualizarDataModificacao();
    }

    public void RemoverServico(Guid itemOrcamentoId)
    {
        ValidarAlteracaoItens();

        var item = ObterServico(itemOrcamentoId);

        if (item is null || item.EstaExcluida())
            throw new DomainException("Serviço não encontrado.");

        item.Excluir();
        AtualizarDataModificacao();
    }

    public ItemOrcamento? ObterServico(Guid itemOrcamentoId)
    {
        return _itensPrevistos.FirstOrDefault(x => x.Id == itemOrcamentoId);
    }

    public void DefinirChecklist(Checklist checklist)
    {
        ArgumentNullException.ThrowIfNull(checklist);

        if (checklist.OrcamentoId != Id)
            throw new DomainException("Checklist inválido para este orçamento.");

        Checklist = checklist;
        AtualizarDataModificacao();
    }

    private void ValidarAlteracaoItens()
    {
        if (Status != StatusOrcamento.EmDiagnostico)
            throw new DomainException("Não é possível alterar itens nesta etapa do orçamento.");
    }

    private void ValidarStatus(StatusOrcamento statusEsperado)
    {
        if (Status != statusEsperado)
            throw new DomainException($"O orçamento precisa estar no status {statusEsperado}.");
    }

    private static void ValidarIdentificador(Guid id, string mensagemErro)
    {
        if (id == Guid.Empty)
            throw new DomainException(mensagemErro);
    }
}
