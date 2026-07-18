using Ofichina.Domain.Exceptions;

namespace Ofichina.Domain.ValueObjects;

public class Endereco : ValueObject
{
    public string Logradouro { get; private set; } = string.Empty;
    public string Numero { get; private set; } = string.Empty;
    public string Complemento { get; private set; } = string.Empty;
    public string Bairro { get; private set; } = string.Empty;
    public string Cidade { get; private set; } = string.Empty;
    public string Estado { get; private set; } = string.Empty;
    public Cep Cep { get; private set; } = null!;

    private Endereco()
    {
    }

    public Endereco(
        string logradouro,
        string? numero,
        string? complemento,
        string? bairro,
        string? cidade,
        string? estado,
        Cep cep)
    {
        if (string.IsNullOrWhiteSpace(logradouro))
            throw new DomainException("Logradouro é obrigatório.");

        if (cep is null)
            throw new DomainException("CEP é obrigatório.");

        Logradouro = logradouro.Trim();
        Numero = string.IsNullOrWhiteSpace(numero) ? string.Empty : numero.Trim();
        Complemento = string.IsNullOrWhiteSpace(complemento) ? string.Empty : complemento.Trim();
        Bairro = string.IsNullOrWhiteSpace(bairro) ? string.Empty : bairro.Trim();
        Cidade = string.IsNullOrWhiteSpace(cidade) ? string.Empty : cidade.Trim();
        Estado = string.IsNullOrWhiteSpace(estado) ? string.Empty : estado.Trim().ToUpperInvariant();
        Cep = cep;
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Logradouro;
        yield return Numero;
        yield return Complemento;
        yield return Bairro;
        yield return Cidade;
        yield return Estado;
        yield return Cep;
    }
}
