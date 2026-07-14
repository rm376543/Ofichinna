using Ofichina.Domain.Exceptions;
using Ofichina.Domain.ValueObjects;

namespace Ofichina.Domain.Entities;

/// <summary>
/// Representa um cliente da oficina.
/// O cliente pode possuir documento do tipo CPF ou CNPJ,
/// através do Value Object Documento.
/// </summary>
public class Cliente : Entity
{
    /// <summary>
    /// Nome ou razão social do cliente.
    /// </summary>
    public string Nome { get; private set; } = null!;


    /// <summary>
    /// Documento de identificação do cliente.
    /// Pode representar CPF ou CNPJ.
    /// </summary>
    public Documento Documento { get; private set; } = null!;


    /// <summary>
    /// Telefone de contato do cliente.
    /// </summary>
    public Telefone Telefone { get; private set; } = null!;


    /// <summary>
    /// Endereço de e-mail do cliente.
    /// </summary>
    public Email Email { get; private set; } = null!;


    /// <summary>
    /// Endereço residencial ou comercial do cliente.
    /// </summary>
    public Endereco Endereco { get; private set; } = null!;


    /// <summary>
    /// Identificador do usuário vinculado ao cliente.
    /// </summary>
    public Guid UsuarioId { get; private set; } = Guid.Empty;


    /// <summary>
    /// Usuário responsável pela autenticação do cliente na aplicação.
    /// </summary>
    public Usuario Usuario { get; private set; } = null!;

    /// <summary>
    /// Veículos associados ao cliente.
    /// </summary>
    private readonly List<Veiculo> _veiculos = [];

    /// <summary>
    /// Coleção de veículos associados ao cliente.
    /// </summary>
    public IReadOnlyCollection<Veiculo> Veiculos =>
        _veiculos.AsReadOnly();


    /// <summary>
    /// Construtor utilizado pelo Entity Framework Core.
    /// </summary>
    private Cliente()
    {
        // EF Core
    }


    /// <summary>
    /// Cria uma nova entidade Cliente.
    /// </summary>
    /// <param name="nome">
    /// Nome ou razão social do cliente.
    /// </param>
    /// <param name="documento">
    /// Documento do cliente (CPF ou CNPJ).
    /// </param>
    /// <param name="telefone">
    /// Telefone de contato.
    /// </param>
    /// <param name="email">
    /// E-mail de contato.
    /// </param>
    /// <param name="endereco">
    /// Endereço do cliente.
    /// </param>
    /// <param name="usuarioId">
    /// Usuário vinculado ao cliente.
    /// </param>
    public Cliente(
        string nome,
        Documento documento,
        Telefone telefone,
        Email email,
        Endereco endereco,
        Guid usuarioId)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException(
                "O nome do cliente deve ser informado.");


        if (documento is null)
            throw new DomainException(
                "O documento deve ser informado.");


        if (telefone is null)
            throw new DomainException(
                "O telefone deve ser informado.");


        if (email is null)
            throw new DomainException(
                "O e-mail deve ser informado.");


        if (endereco is null)
            throw new DomainException(
                "O endereço deve ser informado.");


        if (usuarioId == Guid.Empty)
            throw new DomainException(
                "O usuário deve ser informado.");


        Nome = nome.Trim();
        Documento = documento;
        Telefone = telefone;
        Email = email;
        Endereco = endereco;
        UsuarioId = usuarioId;
    }


    /// <summary>
    /// Altera o nome do cliente.
    /// </summary>
    /// <param name="nome">
    /// Novo nome ou razão social.
    /// </param>
    public void AlterarNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException(
                "O nome deve ser informado.");


        Nome = nome.Trim();

        AtualizarDataModificacao();
    }


    /// <summary>
    /// Altera o telefone de contato do cliente.
    /// </summary>
    /// <param name="telefone">
    /// Novo telefone do cliente.
    /// </param>
    public void AlterarTelefone(Telefone telefone)
    {
        Telefone = telefone ??
                   throw new DomainException(
                       "Telefone inválido.");

        AtualizarDataModificacao();
    }


    /// <summary>
    /// Altera o e-mail de contato do cliente.
    /// </summary>
    /// <param name="email">
    /// Novo e-mail do cliente.
    /// </param>
    public void AlterarEmail(Email email)
    {
        Email = email ??
                throw new DomainException(
                    "E-mail inválido.");

        AtualizarDataModificacao();
    }


    /// <summary>
    /// Altera o endereço do cliente.
    /// </summary>
    /// <param name="endereco">
    /// Novo endereço do cliente.
    /// </param>
    public void AlterarEndereco(Endereco endereco)
    {
        Endereco = endereco ??
                   throw new DomainException(
                       "Endereço inválido.");

        AtualizarDataModificacao();
    }


    /// <summary>
    /// Desativa o cliente através de exclusão lógica.
    /// O registro permanece armazenado, mas não deve ser utilizado em novas operações.
    /// </summary>
    public void DesativarCliente()
    {
        Excluir();
    }


    /// <summary>
    /// Reativa um cliente previamente desativado.
    /// </summary>
    public void ReativarCliente()
    {
        if (!EstaExcluida())
            return;


        DeletedAt = null;

        AtualizarDataModificacao();
    }
}