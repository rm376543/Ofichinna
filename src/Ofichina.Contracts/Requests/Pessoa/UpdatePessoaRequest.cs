using Ofichina.Contracts.Common;

namespace Ofichina.Contracts.Requests.Pessoa;

/// <summary>
/// Requisição para atualização de uma pessoa.
/// </summary>
public sealed class UpdatePessoaRequest : UpdateRequest
{
    /// <summary>
    /// Identificador da pessoa.
    /// </summary>
    public Guid PessoaId { get; set; }

    /// <summary>
    /// Nome completo ou razão social da pessoa.
    /// </summary>
    public string Nome { get; set; } = string.Empty;

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
}
