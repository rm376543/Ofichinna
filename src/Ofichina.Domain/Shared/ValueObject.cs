namespace Ofichina.Domain.Shared;

/// <summary>
/// Classe base para objetos de valor (Value Objects).
/// Objetos de valor são imutáveis e identificados pelo seu conteúdo.
/// </summary>
public abstract class ValueObject
{
    /// <summary>
    /// Retorna os componentes iguais para comparação.
    /// </summary>
    protected abstract IEnumerable<object> GetAtomicValues();

    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType())
        {
            return false;
        }

        var valueObject = (ValueObject)obj;
        return GetAtomicValues().SequenceEqual(valueObject.GetAtomicValues());
    }

    public override int GetHashCode()
    {
        return GetAtomicValues()
            .Aggregate(1, (current, obj) => unchecked(current * 23 + (obj?.GetHashCode() ?? 0)));
    }

    public static bool operator ==(ValueObject? left, ValueObject? right)
    {
        if (left is null ^ right is null)
            return false;

        return left is null || left.Equals(right);
    }

    public static bool operator !=(ValueObject? left, ValueObject? right)
    {
        return !(left == right);
    }
}
