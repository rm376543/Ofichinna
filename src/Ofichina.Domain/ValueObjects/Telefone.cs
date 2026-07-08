using System.Text.RegularExpressions;

namespace Ofichina.Domain.ValueObjects;

/// <summary>
/// Representa um telefone válido do domínio.
/// </summary>
public sealed class Telefone : ValueObject
{
    public string Value { get; }

    private Telefone(string value)
    {
        Value = value;
    }

    public static Telefone Criar(string telefone)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(telefone);

        #pragma warning disable S6444
        telefone = Regex.Replace(telefone, @"\D", "");
        #pragma warning restore S6444

        if (!EhValido(telefone))
            throw new ArgumentException("Telefone inválido.", nameof(telefone));

        return new Telefone(telefone);
    }

    private static bool EhValido(string telefone)
    {
        // Deve possuir 10 (fixo) ou 11 (celular) dígitos
        if (telefone.Length is not (10 or 11))
            return false;

        // DDD
        var ddd = int.Parse(telefone[..2]);

        if (ddd < 11 || ddd > 99)
            return false;

        // Celular
        if (telefone.Length == 11)
        {
            if (telefone[2] != '9')
                return false;
        }

        return true;
    }

    /// <summary>
    /// Retorna o telefone formatado.
    /// </summary>
    public string Formatado =>
        Value.Length == 11
            ? $"({Value[..2]}) {Value.Substring(2, 5)}-{Value.Substring(7)}"
            : $"({Value[..2]}) {Value.Substring(2, 4)}-{Value.Substring(6)}";

    public override string ToString() => Formatado;

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}