using Ofichina.Domain.Exceptions;

namespace Ofichina.Domain.ValueObjects;

/// <summary>
/// Representa a quilometragem atual de um veículo.
/// </summary>
public sealed class Hodometro : ValueObject
{
    public int Valor { get; private set; }

    public Hodometro()
    {
    }

    public Hodometro(int valor)
    {
        if (valor < 0)
            throw new DomainException("A quilometragem não pode ser negativa.");

        Valor = valor;
    }

    public override string ToString() => $"{Valor:N0} km";

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Valor;
    }
}