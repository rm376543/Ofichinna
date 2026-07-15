using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;

namespace Ofichina.Application.UseCases.Pessoas.Commands;

/// <summary>
/// Comando para criação de pessoa.
/// </summary>
public sealed class CreatePessoaCommand : ICommand<Result<Guid>>
{
    /// <summary>
    /// Nome completo ou razão social.
    /// </summary>
    public string Nome { get; init; } = string.Empty;

    /// <summary>
    /// Documento da pessoa.
    /// </summary>
    public string Documento { get; init; } = string.Empty;

    /// <summary>
    /// Telefone de contato.
    /// </summary>
    public string Telefone { get; init; } = string.Empty;

    /// <summary>
    /// Logradouro do endereço.
    /// </summary>
    public string Logradouro { get; init; } = string.Empty;

    /// <summary>
    /// Número do endereço.
    /// </summary>
    public string Numero { get; init; } = string.Empty;

    /// <summary>
    /// Complemento do endereço.
    /// </summary>
    public string Complemento { get; init; } = string.Empty;

    /// <summary>
    /// Bairro do endereço.
    /// </summary>
    public string Bairro { get; init; } = string.Empty;

    /// <summary>
    /// Cidade do endereço.
    /// </summary>
    public string Cidade { get; init; } = string.Empty;

    /// <summary>
    /// Estado do endereço.
    /// </summary>
    public string Estado { get; init; } = string.Empty;

    /// <summary>
    /// CEP do endereço.
    /// </summary>
    public string Cep { get; init; } = string.Empty;

    /// <summary>
    /// Usuário vinculado à pessoa.
    /// </summary>
    public Guid UsuarioId { get; init; }
}
