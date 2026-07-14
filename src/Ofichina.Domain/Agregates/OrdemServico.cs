using Ofichina.Domain.Entities;
using Ofichina.Domain.Exceptions;

namespace Ofichina.Domain.Aggregates;

/// <summary>
/// Define os possíveis estados de uma ordem de serviço.
/// </summary>
public enum StatusOrdemServico
{
    Recebida = 1,
    EmDiagnostico = 2,
    AguardandoAprovacao = 3,
    EmExecucao = 4,
    Finalizada = 5,
    Entregue = 6,
    Cancelada = 7
}


/// <summary>
/// Representa a ordem de serviço da oficina.
/// É o Aggregate Root responsável por controlar serviços,
/// peças e ciclo de vida do atendimento.
/// </summary>
public class OrdemServico : Entity
{
    private readonly List<ItemServico> _servicos = [];

    private readonly List<ItemPeca> _pecas = [];


    /// <summary>
    /// Cliente relacionado à ordem de serviço.
    /// </summary>
    public Guid ClienteId { get; private set; }


    /// <summary>
    /// Veículo relacionado à ordem de serviço.
    /// </summary>
    public Guid VeiculoId { get; private set; }


    /// <summary>
    /// Funcionário responsável pelo atendimento.
    /// </summary>
    public Guid FuncionarioId { get; private set; }


    /// <summary>
    /// Status atual da ordem de serviço.
    /// </summary>
    public StatusOrdemServico Status { get; private set; }


    /// <summary>
    /// Data de abertura da ordem de serviço.
    /// </summary>
    public DateTime DataAbertura { get; private set; }


    /// <summary>
    /// Data em que a ordem foi finalizada.
    /// </summary>
    public DateTime? DataFinalizacao { get; private set; }


    /// <summary>
    /// Observações gerais da ordem de serviço.
    /// </summary>
    public string? Observacao { get; private set; }


    /// <summary>
    /// Serviços adicionados na ordem.
    /// </summary>
    public IReadOnlyCollection<ItemServico> Servicos =>
        _servicos.AsReadOnly();


    /// <summary>
    /// Peças adicionadas na ordem.
    /// </summary>
    public IReadOnlyCollection<ItemPeca> Pecas =>
        _pecas.AsReadOnly();


    /// <summary>
    /// Valor total da ordem de serviço.
    /// Calculado através dos serviços e peças.
    /// </summary>
    public decimal ValorTotal =>
        _servicos.Sum(x => x.ValorTotal) +
        _pecas.Sum(x => x.ValorTotal);


    /// <summary>
    /// Construtor utilizado pelo Entity Framework Core.
    /// </summary>
    private OrdemServico()
    {
    }


    /// <summary>
    /// Cria uma nova ordem de serviço.
    /// </summary>
    public OrdemServico(
        Guid clienteId,
        Guid veiculoId,
        Guid funcionarioId,
        string? observacao)
    {
        if (clienteId == Guid.Empty)
            throw new DomainException("Cliente obrigatório.");

        if (veiculoId == Guid.Empty)
            throw new DomainException("Veículo obrigatório.");

        if (funcionarioId == Guid.Empty)
            throw new DomainException("Funcionário obrigatório.");


        ClienteId = clienteId;
        VeiculoId = veiculoId;
        FuncionarioId = funcionarioId;

        Observacao = observacao;

        Status = StatusOrdemServico.Recebida;

        DataAbertura = DateTime.UtcNow;
    }


    /// <summary>
    /// Inicia o diagnóstico do veículo.
    /// </summary>
    public void IniciarDiagnostico()
    {
        ValidarStatus(
            StatusOrdemServico.Recebida);

        Status = StatusOrdemServico.EmDiagnostico;

        AtualizarDataModificacao();
    }


    /// <summary>
    /// Solicita aprovação do cliente após diagnóstico.
    /// </summary>
    public void SolicitarAprovacao()
    {
        ValidarStatus(
            StatusOrdemServico.EmDiagnostico);

        Status = StatusOrdemServico.AguardandoAprovacao;

        AtualizarDataModificacao();
    }


    /// <summary>
    /// Aprova a execução da ordem de serviço.
    /// </summary>
    public void Aprovar()
    {
        ValidarStatus(
            StatusOrdemServico.AguardandoAprovacao);

        Status = StatusOrdemServico.EmExecucao;

        AtualizarDataModificacao();
    }


    /// <summary>
    /// Adiciona um serviço na ordem de serviço.
    /// </summary>
    public void AdicionarServico(
        Guid servicoId,
        string descricao,
        decimal valor)
    {
        ValidarAlteracaoItens();

        var item = new ItemServico(
            Id,
            servicoId,
            descricao,
            valor);


        _servicos.Add(item);

        AtualizarDataModificacao();
    }


    /// <summary>
    /// Adiciona uma peça na ordem de serviço.
    /// </summary>
    public void AdicionarPeca(
        Guid produtoId,
        string descricao,
        int quantidade,
        decimal valorUnitario)
    {
        ValidarAlteracaoItens();

        var item = new ItemPeca(
            Id,
            produtoId,
            descricao,
            quantidade,
            valorUnitario);


        _pecas.Add(item);

        AtualizarDataModificacao();
    }


    /// <summary>
    /// Remove uma peça da ordem de serviço.
    /// </summary>
    public void RemoverPeca(Guid itemPecaId)
    {
        ValidarAlteracaoItens();

        var peca = _pecas
            .FirstOrDefault(x => x.Id == itemPecaId);


        if (peca is null)
            throw new DomainException(
                "Peça não encontrada.");


        peca.ValidarRemocao();

        _pecas.Remove(peca);

        AtualizarDataModificacao();
    }


    /// <summary>
    /// Marca uma peça como utilizada no veículo.
    /// </summary>
    public void UtilizarPeca(Guid itemPecaId)
    {
        if (Status != StatusOrdemServico.EmExecucao)
            throw new DomainException(
                "Peças somente podem ser utilizadas durante a execução da OS.");


        var peca = _pecas
            .FirstOrDefault(x => x.Id == itemPecaId);


        if (peca is null)
            throw new DomainException(
                "Peça não encontrada.");


        peca.MarcarComoUtilizada();

        AtualizarDataModificacao();
    }


    /// <summary>
    /// Finaliza a ordem de serviço.
    /// </summary>
    public void Finalizar()
    {
        if (Status != StatusOrdemServico.EmExecucao)
            throw new DomainException(
                "A OS precisa estar em execução para ser finalizada.");


        if (_pecas.Any(x => !x.Utilizada))
            throw new DomainException(
                "Existem peças pendentes de utilização.");


        Status = StatusOrdemServico.Finalizada;

        DataFinalizacao = DateTime.UtcNow;

        AtualizarDataModificacao();
    }


    /// <summary>
    /// Marca a ordem de serviço como entregue ao cliente.
    /// </summary>
    public void Entregar()
    {
        ValidarStatus(
            StatusOrdemServico.Finalizada);


        Status = StatusOrdemServico.Entregue;

        AtualizarDataModificacao();
    }


    /// <summary>
    /// Cancela a ordem de serviço.
    /// </summary>
    public void Cancelar()
    {
        if (Status == StatusOrdemServico.Finalizada ||
            Status == StatusOrdemServico.Entregue)
        {
            throw new DomainException(
                "Não é possível cancelar uma OS finalizada ou entregue.");
        }


        Status = StatusOrdemServico.Cancelada;

        this.AtualizarDataModificacao();
    }


    /// <summary>
    /// Valida se a ordem permite alteração de itens.
    /// </summary>
    private void ValidarAlteracaoItens()
    {
        if (Status != StatusOrdemServico.Recebida &&
            Status != StatusOrdemServico.EmDiagnostico)
        {
            throw new DomainException(
                "Não é possível alterar itens nesta etapa da OS.");
        }
    }


    /// <summary>
    /// Valida se a OS está no status esperado.
    /// </summary>
    private void ValidarStatus(
        StatusOrdemServico statusEsperado)
    {
        if (Status != statusEsperado)
        {
            throw new DomainException(
                $"A OS precisa estar no status {statusEsperado}.");
        }
    }
}