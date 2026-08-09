using Ofichina.Domain.Entities;
using Ofichina.Domain.Enums;
using Ofichina.Domain.Exceptions;

namespace Ofichina.Domain.Aggregates;

/// <summary>
/// Representa a ordem de serviço da oficina.
/// É o Aggregate Root responsável por controlar serviços,
/// peças e ciclo de vida do atendimento.
/// </summary>
public sealed class OrdemServico : Entity
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
    /// Funcionário responsável pelo atendimento.
    /// </summary>
    public Guid FuncionarioId { get; private set; }

    /// <summary>
    /// Mecânico responsável pelo reparo.
    /// </summary>
    public Guid MecanicoReparoId { get; private set; }

    /// <summary>
    /// Hodômetro de entrada do veículo na ordem de serviço.
    /// </summary>
    public int HodometroEntrada { get; private set; }

    /// <summary>
    /// Problema relatado na abertura da ordem de serviço.
    /// </summary>
    public string ProblemaRelatado { get; private set; } = string.Empty;

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
    public IReadOnlyCollection<ItemServico> Servicos => _servicos.AsReadOnly();

    /// <summary>
    /// Valor total da ordem de serviço.
    /// Calculado através dos serviços e suas peças.
    /// </summary>
    public decimal ValorTotal =>
        _servicos.Where(x => !x.EstaExcluida())
            .Sum(x => (x.Servico?.Valor ?? 0) + ((x.Peca?.Valor ?? 0) * x.Quantidade));

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
        : this(pessoaId, veiculoId, funcionarioId, 0, string.Empty, observacao)
    {
    }

    /// <summary>
    /// Cria uma nova ordem de serviço.
    /// </summary>
    public OrdemServico(
        Guid pessoaId,
        Guid veiculoId,
        Guid funcionarioId,
        int hodometroEntrada,
        string problemaRelatado,
        string? observacao)
    {
        if (pessoaId == Guid.Empty)
            throw new DomainException("Pessoa obrigatória.");

        if (veiculoId == Guid.Empty)
            throw new DomainException("Veículo obrigatório.");

        if (funcionarioId == Guid.Empty)
            throw new DomainException("Funcionário obrigatório.");

        if (hodometroEntrada < 0)
            throw new DomainException("A quilometragem não pode ser negativa.");

        if (string.IsNullOrWhiteSpace(problemaRelatado))
            throw new DomainException("Problema relatado obrigatório.");

        PessoaId = pessoaId;
        VeiculoId = veiculoId;
        FuncionarioId = funcionarioId;
        HodometroEntrada = hodometroEntrada;
        ProblemaRelatado = problemaRelatado;
        Observacao = observacao;
        MecanicoReparoId = Guid.Empty;
        Status = StatusOrdemServico.Recebida;
        DataAbertura = DateTime.UtcNow;
    }

    /// <summary>
    /// Cria uma ordem de serviço a partir de um orçamento aprovado.
    /// </summary>
    public static OrdemServico CriarAPartirDoOrcamento(Orcamento orcamento, Guid mecanicoReparoId)
    {
        ArgumentNullException.ThrowIfNull(orcamento);

        if (orcamento.Status != StatusOrcamento.Aprovado)
            throw new DomainException("O orçamento precisa estar aprovado para gerar a ordem de serviço.");

        var problemaRelatado = orcamento.Observacoes;
        if (string.IsNullOrWhiteSpace(problemaRelatado))
            problemaRelatado = orcamento.Observacoes ?? "Orçamento aprovado";

        var hodometroEntrada = 0;

        var ordemServico = new OrdemServico(
            orcamento.PessoaId,
            orcamento.VeiculoId,
            orcamento.ConsultorId,
            hodometroEntrada,
            problemaRelatado,
            orcamento.Observacoes);

        ordemServico.DesignarMecanicoReparo(mecanicoReparoId, orcamento.MecanicoId);

        foreach (var item in orcamento.ItensServico.Where(x => !x.EstaExcluida()))
            ordemServico.AdicionarServico(item.ServicoId, item.PecaId, item.Quantidade);

        return ordemServico;
    }

    /// <summary>
    /// Atualiza os dados da ordem de serviço.
    /// </summary>
    public void AtualizarDados(
        Guid pessoaId,
        Guid veiculoId,
        Guid funcionarioId,
        int hodometroEntrada,
        string problemaRelatado,
        string? observacao)
    {
        if (pessoaId == Guid.Empty)
            throw new DomainException("Pessoa obrigatória.");

        if (veiculoId == Guid.Empty)
            throw new DomainException("Veículo obrigatório.");

        if (funcionarioId == Guid.Empty)
            throw new DomainException("Funcionário obrigatório.");

        if (hodometroEntrada < 0)
            throw new DomainException("A quilometragem não pode ser negativa.");

        if (string.IsNullOrWhiteSpace(problemaRelatado))
            throw new DomainException("Problema relatado obrigatório.");

        PessoaId = pessoaId;
        VeiculoId = veiculoId;
        FuncionarioId = funcionarioId;
        HodometroEntrada = hodometroEntrada;
        ProblemaRelatado = problemaRelatado;
        Observacao = observacao;

        AtualizarDataModificacao();
    }

    /// <summary>
    /// Desenha o mecânico responsável pelo reparo.
    /// </summary>
    public void DesignarMecanicoReparo(Guid mecanicoReparoId, Guid mecanicoId)
    {
        if (mecanicoReparoId == Guid.Empty)
            throw new DomainException("Mecânico de reparo obrigatório.");

        MecanicoReparoId = mecanicoReparoId;
        AtualizarDataModificacao();
    }

    /// <summary>
    /// Inicia a execução da ordem de serviço.
    /// </summary>
    public void IniciarExecucao()
    {
        ValidarStatus(StatusOrdemServico.Recebida);

        Status = StatusOrdemServico.EmExecucao;
        AtualizarDataModificacao();
    }

    /// <summary>
    /// Mantém compatibilidade com fluxos existentes.
    /// </summary>
    public void Aprovar()
    {
        IniciarExecucao();
    }

    /// <summary>
    /// Adiciona um serviço na ordem de serviço.
    /// </summary>
    public ItemServico AdicionarServico(Guid servicoId, Guid? pecaId, int quantidade)
    {
        ValidarAlteracaoItens();

        var item = ItemServico.ParaOrdemServico(Id, servicoId, pecaId, quantidade);
        _servicos.Add(item);

        AtualizarDataModificacao();

        return item;
    }

    /// <summary>
    /// Atualiza um serviço existente na ordem de serviço.
    /// </summary>
    public void AtualizarServico(
        Guid itemServicoId,
        Guid servicoId,
        Guid? pecaId,
        int quantidade)
    {
        ValidarAlteracaoItens();

        var item = ObterServico(itemServicoId);

        if (item is null || item.EstaExcluida())
            throw new DomainException("Serviço não encontrado.");

        item.AtualizarDados(servicoId, pecaId, quantidade);
        AtualizarDataModificacao();
    }

    /// <summary>
    /// Remove um serviço da ordem de serviço.
    /// </summary>
    public void RemoverServico(Guid itemServicoId)
    {
        ValidarAlteracaoItens();

        var item = ObterServico(itemServicoId);

        if (item is null || item.EstaExcluida())
            throw new DomainException("Serviço não encontrado.");

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
    /// Finaliza a ordem de serviço.
    /// </summary>
    public void Finalizar()
    {
        if (Status != StatusOrdemServico.EmExecucao)
            throw new DomainException("A OS precisa estar em execução para ser finalizada.");

        if (!_servicos.Any(x => !x.EstaExcluida()))
            throw new DomainException("A ordem de serviço precisa possuir itens cadastrados.");

        Status = StatusOrdemServico.Finalizada;
        DataFinalizacao = DateTime.UtcNow;

        AtualizarDataModificacao();
    }

    /// <summary>
    /// Marca a ordem de serviço como entregue ao cliente.
    /// </summary>
    public void Entregar()
    {
        ValidarStatus(StatusOrdemServico.Finalizada);

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
            throw new DomainException("Não é possível cancelar uma OS finalizada ou entregue.");
        }

        Status = StatusOrdemServico.Cancelada;
        AtualizarDataModificacao();
    }

    /// <summary>
    /// Valida se a ordem permite alteração de itens.
    /// </summary>
    private void ValidarAlteracaoItens()
    {
        if (Status != StatusOrdemServico.Recebida)
            throw new DomainException("Não é possível alterar itens nesta etapa da OS.");
    }

    /// <summary>
    /// Valida se a OS está no status esperado.
    /// </summary>
    private void ValidarStatus(StatusOrdemServico statusEsperado)
    {
        if (Status != statusEsperado)
            throw new DomainException($"A OS precisa estar no status {statusEsperado}.");
    }
}