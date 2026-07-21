using Ofichina.Domain.Entities;
using Ofichina.Domain.Exceptions;
using Ofichina.Domain.Enums;

namespace Ofichina.Domain.Aggregates;


/// <summary>
/// Representa a ordem de serviço da oficina.
/// É o Aggregate Root responsável por controlar serviços,
/// peças e ciclo de vida do atendimento.
/// </summary>
public class OrdemServico : Entity
{
    private readonly List<ItemServico> _servicos = [];


    /// <summary>
    /// Pessoa relacionada à ordem de serviço.
    /// </summary>
    public Guid PessoaId { get; private set; }


    /// <summary>
    /// Veículo relacionado à ordem de serviço.
    /// </summary>
    public Guid VeiculoId { get; private set; }


    /// <summary>
    /// Funcionário responsável pelo atendimento (mecânico/atendente).
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
    /// Valor total da ordem de serviço.
    /// Calculado através dos serviços e suas peças.
    /// </summary>
    public decimal ValorTotal =>
        _servicos.Where(x => !x.EstaExcluida()).Sum(x => x.ValorTotal);


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
        Guid pessoaId,
        Guid veiculoId,
        Guid funcionarioId,
        string? observacao)
    {
        if (pessoaId == Guid.Empty)
            throw new DomainException("Pessoa obrigatória.");

        if (veiculoId == Guid.Empty)
            throw new DomainException("Veículo obrigatório.");

        if (funcionarioId == Guid.Empty)
            throw new DomainException("Funcionário obrigatório.");


        PessoaId = pessoaId;
        VeiculoId = veiculoId;
        FuncionarioId = funcionarioId;

        Observacao = observacao;

        Status = StatusOrdemServico.Recebida;

        DataAbertura = DateTime.UtcNow;
    }


    /// <summary>
    /// Atualiza os dados do atendimento da ordem de serviço.
    /// </summary>
    public void AtualizarAtendimento(
        Guid funcionarioId,
        string? observacao)
    {
        if (funcionarioId == Guid.Empty)
            throw new DomainException("Funcionário obrigatório.");


        FuncionarioId = funcionarioId;
        Observacao = observacao;

        AtualizarDataModificacao();
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
    public ItemServico AdicionarServico(
        Guid servicoId,
        string descricao,
        decimal valor)
    {
        ValidarAlteracaoItens();

        if (_servicos.Any(x => x.ServicoId == servicoId && !x.EstaExcluida()))
            throw new DomainException("O serviço já foi adicionado à ordem de serviço.");

        var item = new ItemServico(
            servicoId,
            Id,
            descricao,
            valor);


        _servicos.Add(item);

        AtualizarDataModificacao();

        return item;
    }


    /// <summary>
    /// Atualiza um serviço existente na ordem de serviço.
    /// </summary>
    public void AtualizarServico(
        Guid itemServicoId,
        string descricao,
        decimal valor)
    {
        ValidarAlteracaoItens();


        var item = ObterServico(itemServicoId);


        if (item is null)
            throw new DomainException(
                "Serviço não encontrado.");

        if (item.EstaExcluida())
            throw new DomainException(
                "Serviço não encontrado.");


        item.AtualizarDados(
            descricao,
            valor);

        AtualizarDataModificacao();
    }


    /// <summary>
    /// Remove um serviço da ordem de serviço.
    /// </summary>
    public void RemoverServico(Guid itemServicoId)
    {
        ValidarAlteracaoItens();


        var item = ObterServico(itemServicoId);


        if (item is null)
            throw new DomainException(
                "Serviço não encontrado.");

        if (item.EstaExcluida())
            throw new DomainException(
                "Serviço não encontrado.");


        item.Excluir();

        AtualizarDataModificacao();
    }


    /// <summary>
    /// Obtém um serviço da ordem pelo identificador.
    /// </summary>
    public ItemServico? ObterServico(Guid itemServicoId)
    {
        return _servicos.FirstOrDefault(x => x.Id == itemServicoId);
    }


    /// <summary>
    /// Adiciona uma peça a um serviço da ordem de serviço.
    /// </summary>
    public void AdicionarPeca(
        Guid itemServicoId,
        Guid pecaId,
        string descricao,
        int quantidade,
        decimal valorUnitario)
    {
        ValidarAlteracaoItens();

        var servico = ObterServico(itemServicoId);

        if (servico is null)
            throw new DomainException("Serviço não encontrado.");

        servico.AdicionarPeca(pecaId, descricao, quantidade, valorUnitario);

        AtualizarDataModificacao();
    }


    /// <summary>
    /// Remove uma peça de um serviço da ordem de serviço.
    /// </summary>
    public void RemoverPeca(Guid itemServicoId, Guid itemPecaId)
    {
        ValidarAlteracaoItens();

        var servico = ObterServico(itemServicoId);

        if (servico is null)
            throw new DomainException("Serviço não encontrado.");

        servico.RemoverPeca(itemPecaId);

        AtualizarDataModificacao();
    }


    /// <summary>
    /// Marca uma peça de um serviço como utilizada no veículo.
    /// </summary>
    public void UtilizarPeca(Guid itemServicoId, Guid itemPecaId)
    {
        if (Status != StatusOrdemServico.EmExecucao)
            throw new DomainException(
                "Peças somente podem ser utilizadas durante a execução da OS.");

        var servico = ObterServico(itemServicoId);

        if (servico is null)
            throw new DomainException("Serviço não encontrado.");

        servico.UtilizarPeca(itemPecaId);

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


        var pecasPendentes = _servicos
            .Where(x => !x.EstaExcluida())
            .SelectMany(s => s.Pecas)
            .Where(x => !x.EstaExcluida() && !x.Utilizada)
            .Any();

        if (pecasPendentes)
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