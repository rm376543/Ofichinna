using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Requests.Pessoa;

namespace Ofichina.Application.UseCases.Pessoas.Commands;

/// <summary>
/// Comando para atualização de pessoa.
/// </summary>
public sealed class UpdatePessoaCommand : ICommand<Result>
{
    /// <summary>
    /// Identificador da pessoa.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Nome completo ou razão social.
    /// </summary>
    public string Nome { get; init; }

    /// <summary>
    /// Telefone de contato.
    /// </summary>
    public string Telefone { get; init; }

    /// <summary>
    /// Logradouro do endereço.
    /// </summary>
    public string Logradouro { get; init; }

    /// <summary>
    /// Número do endereço.
    /// </summary>
    public string Numero { get; init; }

    /// <summary>
    /// Complemento do endereço.
    /// </summary>
    public string Complemento { get; init; }

    /// <summary>
    /// Bairro do endereço.
    /// </summary>
    public string Bairro { get; init; }

    /// <summary>
    /// Cidade do endereço.
    /// </summary>
    public string Cidade { get; init; }

    /// <summary>
    /// Estado do endereço.
    /// </summary>
    public string Estado { get; init; }

    /// <summary>
    /// CEP do endereço.
    /// </summary>
    public string Cep { get; init; }

    public UpdatePessoaCommand(UpdatePessoaRequest request)
    {
        Id = request.Id;
        Nome = request.Nome;
        Telefone = request.Telefone;
        Logradouro = request.Logradouro;
        Numero = request.Numero;
        Complemento = request.Complemento;
        Bairro = request.Bairro;
        Cidade = request.Cidade;
        Estado = request.Estado;
        Cep = request.Cep;
    }
}
