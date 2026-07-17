using System.Text.RegularExpressions;
using Ofichina.Domain.Exceptions;

namespace Ofichina.Domain.ValueObjects;

/// <summary>
/// Representa uma placa de veículo válida (Brasil ou Mercosul).
/// </summary>
public sealed class Placa : ValueObject
{
#pragma warning disable S6444
    private static readonly Regex PlacaBrasilRegex = new(@"^[A-Z]{3}[0-9]{4}$", RegexOptions.Compiled);
#pragma warning restore S6444

#pragma warning disable S6444
    private static readonly Regex PlacaMercosulRegex = new(@"^[A-Z]{3}[0-9][A-Z][0-9]{2}$", RegexOptions.Compiled);
#pragma warning restore S6444

    public string Numero { get; private set; } = null!;

    public Placa()
    {
        // EF Core
    }

    public Placa(string placa)
    {
        if (string.IsNullOrWhiteSpace(placa))
            throw new DomainException("Placa inválida.");

        placa = Normalizar(placa);

        if (!EhValida(placa))
            throw new DomainException("Placa inválida.");

        Numero = placa;
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
        PlacaMercosulRegex.IsMatch(Numero);

    public bool EhModeloAntigo =>
        PlacaBrasilRegex.IsMatch(Numero);

    public string Formatada =>
        $"{Numero[..3]}-{Numero[3..]}";

    public override string ToString() => Formatada;

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Numero;
    }
}