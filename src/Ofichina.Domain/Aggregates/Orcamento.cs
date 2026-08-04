using Ofichina.Domain.Entities;
using Ofichina.Domain.Enums;
using Ofichina.Domain.Exceptions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ofichina.Domain.Aggregates;

/// <summary>
/// Representa o orçamento da oficina e seu ciclo de aprovação.
/// </summary>
public sealed class Orcamento : Entity
{
    private readonly List<ItemServico> _itensServico = [];

    public Guid PessoaId { get; private set; }

    public Guid VeiculoId { get; private set; }

    public Guid MecanicoDiagnosticoId { get; private set; }

    public Guid ResponsavelId { get; private set; }

    public DateTime DataValidade { get; private set; }

    public decimal Desconto { get; private set; }

    public string? Observacoes { get; private set; }

    public Guid? ChecklistId { get; private set; }

    public StatusOrcamento Status { get; private set; }

    public DateTime DataCriacao => CreatedAt;

    public Checklist? Checklist { get; private set; }

    public IReadOnlyCollection<ItemServico> ItensServico => _itensServico.AsReadOnly();

    [NotMapped]
    public IReadOnlyCollection<ItemServico> Servicos => ItensServico;

    [NotMapped]
    public IReadOnlyCollection<ItemServico> ItensPrevistos => ItensServico;

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
        string? observacoes,
        Guid? checklistId = null)
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
        ChecklistId = checklistId;
        Status = StatusOrcamento.Recebida;
    }

    public void IniciarDiagnostico()
    {
        ValidarStatus(StatusOrcamento.Recebida);

        Status = StatusOrcamento.EmDiagnostico;
        AtualizarDataModificacao();
    }

    public void FinalizarDiagnostico()
    {
        ValidarStatus(StatusOrcamento.EmDiagnostico);

        Status = StatusOrcamento.AguardandoAprovacao;
        AtualizarDataModificacao();
    }

    public void EnviarParaCliente()
    {
        FinalizarDiagnostico();
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

    public void ReenviarAposReprovacao()
    {
        ValidarStatus(StatusOrcamento.Reprovado);

        Status = StatusOrcamento.EmDiagnostico;
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

    public ItemServico AdicionarServico(Guid servicoId, Guid pecaId, int quantidade)
    {
        ValidarAlteracaoItens();

        var item = ItemServico.ParaOrcamento(Id, servicoId, pecaId, quantidade);
        _itensServico.Add(item);

        AtualizarDataModificacao();

        return item;
    }

    public void AtualizarServico(Guid itemServicoId, Guid servicoId, Guid pecaId, int quantidade)
    {
        ValidarAlteracaoItens();

        var item = ObterServico(itemServicoId);

        if (item is null || item.EstaExcluida())
            throw new DomainException("Serviço não encontrado.");

        item.AtualizarDados(servicoId, pecaId, quantidade);
        AtualizarDataModificacao();
    }

    public void RemoverServico(Guid itemServicoId)
    {
        ValidarAlteracaoItens();

        var item = ObterServico(itemServicoId);

        if (item is null || item.EstaExcluida())
            throw new DomainException("Serviço não encontrado.");

        item.Excluir();
        AtualizarDataModificacao();
    }

    public ItemServico? ObterServico(Guid itemServicoId)
    {
        return _itensServico.FirstOrDefault(x => x.Id == itemServicoId);
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
