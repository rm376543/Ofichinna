using Ofichina.Domain.Exceptions;
using Ofichina.Domain.ValueObjects;

namespace Ofichina.Domain.Entities;

/// <summary>
/// Representa uma pessoa no sistema da oficina.
/// Pode ser cliente, mecânico, atendente, etc.
/// O tipo de pessoa é definido através dos perfis associados ao usuário vinculado.
/// </summary>
public class Pessoa : Entity
{
    private readonly List<Veiculo> _veiculos = [];

    /// <summary>
    /// Nome completo ou razão social da pessoa.
    /// </summary>
    public string Nome { get; private set; } = null!;

    /// <summary>
    /// Documento de identificação da pessoa.
    /// Pode representar CPF ou CNPJ.
    /// </summary>
    public Documento Documento { get; private set; } = null!;

    /// <summary>
    /// Telefone de contato da pessoa.
    /// </summary>
    public Telefone Telefone { get; private set; } = null!;

    /// <summary>
    /// Endereço residencial ou comercial da pessoa.
    /// </summary>
    public Endereco Endereco { get; private set; } = null!;

    /// <summary>
    /// Identificador do usuário vinculado à pessoa.
    /// </summary>
    public Guid UsuarioId { get; private set; } = Guid.Empty;

    /// <summary>
    /// Usuário responsável pela autenticação da pessoa na aplicação.
    /// </summary>
    public Usuario Usuario { get; private set; } = null!;

    /// <summary>
    /// Veículos vinculados à pessoa.
    /// </summary>
    public IReadOnlyCollection<Veiculo> Veiculos => _veiculos.AsReadOnly();

    /// <summary>
    /// Construtor utilizado pelo Entity Framework Core.
    /// </summary>
    private Pessoa()
    {
        // EF Core
    }

    /// <summary>
    /// Cria uma nova entidade Pessoa.
    /// </summary>
    /// <param name="nome">Nome completo ou razão social da pessoa.</param>
    /// <param name="documento">Documento da pessoa (CPF ou CNPJ).</param>
    /// <param name="telefone">Telefone de contato.</param>
    /// <param name="email">E-mail de contato.</param>
    /// <param name="endereco">Endereço da pessoa.</param>
    /// <param name="usuarioId">Usuário vinculado à pessoa.</param>
    public Pessoa(
        string nome,
        Documento documento,
        Telefone telefone,
        Endereco endereco,
        Guid usuarioId)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("O nome deve ser informado.");

        if (documento is null)
            throw new DomainException("O documento deve ser informado.");

        if (telefone is null)
            throw new DomainException("O telefone deve ser informado.");

        if (endereco is null)
            throw new DomainException("O endereço deve ser informado.");

        if (usuarioId == Guid.Empty)
            throw new DomainException("O usuário deve ser informado.");

        Nome = nome.Trim();
        Documento = documento;
        Telefone = telefone;
        Endereco = endereco;
        UsuarioId = usuarioId;
    }

    /// <summary>
    /// Altera o nome da pessoa.
    /// </summary>
    /// <param name="nome">Novo nome ou razão social.</param>
    public void AlterarNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("O nome deve ser informado.");

        Nome = nome.Trim();
        AtualizarDataModificacao();
    }

    /// <summary>
    /// Altera o telefone de contato da pessoa.
    /// </summary>
    /// <param name="telefone">Novo telefone.</param>
    public void AlterarTelefone(Telefone telefone)
    {
        Telefone = telefone ?? throw new DomainException("Telefone inválido.");
        AtualizarDataModificacao();
    }

    /// <summary>
    /// Altera o endereço da pessoa.
    /// </summary>
    /// <param name="endereco">Novo endereço.</param>
    public void AlterarEndereco(Endereco endereco)
    {
        Endereco = endereco ?? throw new DomainException("Endereço inválido.");
        AtualizarDataModificacao();
    }

    /// <summary>
    /// Desativa a pessoa através de exclusão lógica.
    /// O registro permanece armazenado, mas não deve ser utilizado em novas operações.
    /// </summary>
    public void Desativar()
    {
        Excluir();
    }

    /// <summary>
    /// Reativa uma pessoa previamente desativada.
    /// </summary>
    public void Reativar()
    {
        if (!EstaExcluida())
            return;

        DeletedAt = null;
        AtualizarDataModificacao();
    }
}
