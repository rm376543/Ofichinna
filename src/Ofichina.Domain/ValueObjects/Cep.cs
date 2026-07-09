using System.Text.RegularExpressions;

namespace Ofichina.Domain.ValueObjects;

/// <summary>
/// Representa um CEP válido do domínio.
/// </summary>
public sealed class Cep : ValueObject
{
    public string Value { get; }

    private Cep(string value)
    {
        Value = value;
    }

    public static Cep Criar(string cep)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(cep);

        #pragma warning disable S6444
        cep = Regex.Replace(cep, @"\D", "");
        #pragma warning restore S6444

        if (!EhValido(cep))
            throw new ArgumentException("CEP inválido.", nameof(cep));

        return new Cep(cep);
    }

    private static bool EhValido(string cep)
    {
        return cep.Length == 8 && cep.All(char.IsDigit);
    }

    /// <summary>
    /// Retorna o CEP formatado (00000-000).
    /// </summary>
    public string Formatado => $"{Value[..5]}-{Value[5..]}";

    public override string ToString() => Formatado;

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}