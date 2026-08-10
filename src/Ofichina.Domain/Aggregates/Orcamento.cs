using Ofichina.Domain.Entities;
using Ofichina.Domain.Enums;
using Ofichina.Domain.Exceptions;

namespace Ofichina.Domain.Aggregates;

/// <summary>
/// Representa o orçamento da oficina e seu ciclo de aprovação.
/// </summary>
public sealed class Orcamento : Entity
{
    private readonly List<ItemServico> _itensServico = [];

    public Guid PessoaId { get; private set; }

    public Guid VeiculoId { get; private set; }

    public Guid AgendamentoId { get; private set; }

    public Guid MecanicoId { get; private set; }

    public Guid ConsultorId { get; private set; }

    public DateTime DataValidade { get; private set; }

    public decimal Desconto { get; private set; }

    public bool DescontoEmDinheiro { get; private set; }

    public string? Observacoes { get; private set; }

    public StatusOrcamento Status { get; private set; }

    public DateTime DataCriacao => CreatedAt;

#pragma warning disable S1144
    public Agendamento? Agendamento { get; private set; }
#pragma warning restore S1144

    public IReadOnlyCollection<ItemServico> ItensServico => _itensServico.AsReadOnly();

    public decimal ValorBruto => CalcularValorBruto();

    public decimal ValorTotal => CalcularValorTotal();

    public decimal ValorTotalDesconto => CalcularValorTotalDesconto();

    private Orcamento()
    {
    }

#pragma warning disable S107
    public Orcamento(
        Guid pessoaId,
        Guid veiculoId,
        Guid agendamentoId,
        Guid mecanicoId,
        Guid consultorId,
        DateTime dataValidade,
        decimal desconto,
        string? observacoes)
#pragma warning restore S107
    {
        ValidarIdentificador(pessoaId, "Pessoa obrigatória.");
        ValidarIdentificador(veiculoId, "Veículo obrigatório.");
        ValidarIdentificador(agendamentoId, "Agendamento obrigatório.");
        ValidarIdentificador(mecanicoId, "Mecânico obrigatório.");
        ValidarIdentificador(consultorId, "Consultor obrigatório.");

        if (desconto < 0)
            throw new DomainException("O desconto não pode ser negativo.");

        if (dataValidade == default)
            throw new DomainException("A data de validade é obrigatória.");

        PessoaId = pessoaId;
        VeiculoId = veiculoId;
        AgendamentoId = agendamentoId;
        MecanicoId = mecanicoId;
        ConsultorId = consultorId;
        DataValidade = dataValidade;
        Desconto = desconto;
        DescontoEmDinheiro = false;
        Observacoes = observacoes;
        Status = StatusOrcamento.Criado;
    }

    public void IniciarDiagnostico()
    {
        ValidarStatus(StatusOrcamento.Criado);

        Status = StatusOrcamento.EmDiagnostico;
        AtualizarDataModificacao();
    }

    public void FinalizarDiagnostico()
    {
        ValidarStatus(StatusOrcamento.EmDiagnostico);
        ValidarItensCadastrados();

        Status = StatusOrcamento.AguardandoEnvio;
        AtualizarDataModificacao();
    }

    public void EnviarParaCliente()
    {
        ValidarStatus(StatusOrcamento.AguardandoEnvio);

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

    public void ReenviarAposReprovacao()
    {
        ValidarStatus(StatusOrcamento.Reprovado);

        Status = StatusOrcamento.EmDiagnostico;
        AtualizarDataModificacao();
    }

    public void AtualizarDados(
        Guid pessoaId,
        Guid veiculoId,
        Guid mecanicoId,
        Guid consultorId,
        DateTime dataValidade,
        string? observacoes)
    {
        ValidarIdentificador(pessoaId, "Pessoa obrigatória.");
        ValidarIdentificador(veiculoId, "Veículo obrigatório.");
        ValidarIdentificador(mecanicoId, "Mecânico obrigatório.");
        ValidarIdentificador(consultorId, "Consultor obrigatório.");

        if (dataValidade == default)
            throw new DomainException("A data de validade é obrigatória.");

        PessoaId = pessoaId;
        VeiculoId = veiculoId;
        MecanicoId = mecanicoId;
        ConsultorId = consultorId;
        DataValidade = dataValidade;
        Observacoes = observacoes;

        AtualizarDataModificacao();
    }

    public void AtualizarDesconto(decimal desconto)
    {
        AtualizarDesconto(desconto, false);
    }

    public void AtualizarDesconto(decimal desconto, bool descontoEmDinheiro)
    {
        if (desconto < 0)
            throw new DomainException("O desconto não pode ser negativo.");

        Desconto = desconto;
        DescontoEmDinheiro = descontoEmDinheiro;
        AtualizarDataModificacao();
    }

    public ItemServico AdicionarServico(Guid servicoId, Guid? pecaId, int quantidade, StatusOrcamento statusOrcamento)
    {
        ValidarAlteracaoItens(statusOrcamento);

        var item = ItemServico.ParaOrcamento(Id, servicoId, pecaId, quantidade);
        _itensServico.Add(item);

        AtualizarDataModificacao();

        return item;
    }

    public void AtualizarServico(Guid itemServicoId, Guid servicoId, Guid? pecaId, int quantidade, StatusOrcamento statusOrcamento)
    {
        ValidarAlteracaoItens(statusOrcamento);

        var item = ObterServico(itemServicoId);

        if (item is null || item.EstaExcluida())
            throw new DomainException("Serviço não encontrado.");

        item.AtualizarDados(servicoId, pecaId, quantidade);
        AtualizarDataModificacao();
    }

    public void RemoverServico(Guid itemServicoId, StatusOrcamento statusOrcamento)
    {
        ValidarAlteracaoItens(statusOrcamento);

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

    private void ValidarAlteracaoItens(StatusOrcamento statusOrcamento)
    {
        if (Status != statusOrcamento)
            throw new DomainException("Não é possível alterar itens nesta etapa do orçamento.");
    }

    private void ValidarItensCadastrados()
    {
        if (!_itensServico.Any(item => !item.EstaExcluida()))
            throw new DomainException("O orçamento precisa ter ao menos um serviço para ser finalizado.");
    }

    private decimal CalcularValorBruto()
    {
        return _itensServico
            .Where(item => !item.EstaExcluida())
            .Sum(item => item.ValorTotal);
    }

    private decimal CalcularValorTotal()
    {
        return CalcularValorTotalDesconto();
    }

    private decimal CalcularValorTotalDesconto()
    {
        if (Desconto == 0)
            return ValorBruto;

        var valorDescontoAplicado = DescontoEmDinheiro
            ? ValorBruto * (Desconto / 100m)
            : Desconto;

        return Math.Max(0m, ValorBruto - valorDescontoAplicado);
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
