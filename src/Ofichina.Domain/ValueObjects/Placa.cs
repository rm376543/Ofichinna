using System.Text.RegularExpressions;

namespace Ofichina.Domain.ValueObjects;

/// <summary>
/// Representa uma placa de veículo válida (Brasil ou Mercosul).
/// </summary>
public sealed class Placa : ValueObject
{
    private static readonly Regex PlacaBrasilRegex =
        #pragma warning disable S6444
        new(@"^[A-Z]{3}[0-9]{4}$", RegexOptions.Compiled);
        #pragma warning restore S6444

    private static readonly Regex PlacaMercosulRegex =
        #pragma warning disable S6444
        new(@"^[A-Z]{3}[0-9][A-Z][0-9]{2}$", RegexOptions.Compiled);
        #pragma warning restore S6444

    public string Value { get; }

    private Placa(string value)
    {
        Value = value;
    }

    public static Placa Criar(string placa)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(placa);

        placa = Normalizar(placa);

        if (!EhValida(placa))
            throw new ArgumentException("Placa inválida.", nameof(placa));

        return new Placa(placa);
    }

    private static string Normalizar(string placa)
    {
        #pragma warning disable S6444
        return Regex
            .Replace(placa, @"[^A-Za-z0-9]", "")
            .ToUpperInvariant();
        #pragma warning restore S6444
    }

    private static bool EhValida(string placa)
    {
        return PlacaBrasilRegex.IsMatch(placa)
            || PlacaMercosulRegex.IsMatch(placa);
    }

    public bool EhMercosul =>
        PlacaMercosulRegex.IsMatch(Value);

    public bool EhModeloAntigo =>
        PlacaBrasilRegex.IsMatch(Value);

    public string Formatada =>
        $"{Value[..3]}-{Value[3..]}";

    public override string ToString() => Formatada;

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}