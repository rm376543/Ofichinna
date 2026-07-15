namespace Ofichina.Contracts.Responses.Pessoa;

/// <summary>
/// Resposta com os dados de uma pessoa.
/// </summary>
public sealed class PessoaResponse
{
    /// <summary>
    /// Identificador da pessoa.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Nome completo ou razão social.
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Documento da pessoa.
    /// </summary>
    public string Documento { get; set; } = string.Empty;

    /// <summary>
    /// Telefone de contato.
    /// </summary>
    public string Telefone { get; set; } = string.Empty;

    /// <summary>
    /// Logradouro do endereço.
    /// </summary>
    public string Logradouro { get; set; } = string.Empty;

    /// <summary>
    /// Número do endereço.
    /// </summary>
    public string Numero { get; set; } = string.Empty;

    /// <summary>
    /// Complemento do endereço.
    /// </summary>
    public string Complemento { get; set; } = string.Empty;

    /// <summary>
    /// Bairro do endereço.
    /// </summary>
    public string Bairro { get; set; } = string.Empty;

    /// <summary>
    /// Cidade do endereço.
    /// </summary>
    public string Cidade { get; set; } = string.Empty;

    /// <summary>
    /// Estado do endereço.
    /// </summary>
    public string Estado { get; set; } = string.Empty;

    /// <summary>
    /// CEP do endereço.
    /// </summary>
    public string Cep { get; set; } = string.Empty;

    /// <summary>
    /// Identificador do usuário vinculado à pessoa.
    /// </summary>
    public Guid UsuarioId { get; set; }

    /// <summary>
    /// Data de criação do registro.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Data da última atualização.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Data da exclusão lógica.
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}
