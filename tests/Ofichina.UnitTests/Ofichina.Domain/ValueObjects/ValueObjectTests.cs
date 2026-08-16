using Ofichina.Domain.ValueObjects;

namespace Ofichina.UnitTests.Domain.ValueObjects;

public class ValueObjectTests
{
    [Fact]
    public void ValueObject_Equals_DeveRetornarTrue_QuandoValoresForemIguais()
    {
        var valueObject1 = new TestValueObject("ABC", 123);
        var valueObject2 = new TestValueObject("ABC", 123);

        Assert.True(valueObject1.Equals(valueObject2));
    }

    [Fact]
    public void ValueObject_Equals_DeveRetornarFalse_QuandoValoresForemDiferentes()
    {
        var valueObject1 = new TestValueObject("ABC", 123);
        var valueObject2 = new TestValueObject("XYZ", 456);

        Assert.False(valueObject1.Equals(valueObject2));
    }

    [Fact]
    public void ValueObject_Equals_DeveRetornarFalse_QuandoOutroObjetoForNulo()
    {
        var valueObject = new TestValueObject("ABC", 123);

        Assert.False(valueObject.Equals(null));
    }

    [Fact]
    public void ValueObject_Equals_DeveRetornarFalse_QuandoTiposForemDiferentes()
    {
        var valueObject = new TestValueObject("ABC", 123);
        var outroValueObject = new OutroTestValueObject("ABC", 123);

        Assert.False(valueObject.Equals(outroValueObject));
    }

    [Fact]
    public void ValueObject_Equals_DeveRetornarTrue_QuandoForAmesmaInstancia()
    {
        var valueObject = new TestValueObject("ABC", 123);

        Assert.True(valueObject.Equals(valueObject));
    }

    [Fact]
    public void ValueObject_GetHashCode_DeveSerIgual_QuandoValoresForemIguais()
    {
        var valueObject1 = new TestValueObject("ABC", 123);
        var valueObject2 = new TestValueObject("ABC", 123);

        Assert.Equal(
            valueObject1.GetHashCode(),
            valueObject2.GetHashCode());
    }

    [Fact]
    public void ValueObject_GetHashCode_DeveSerDiferente_QuandoValoresForemDiferentes()
    {
        var valueObject1 = new TestValueObject("ABC", 123);
        var valueObject2 = new TestValueObject("XYZ", 456);

        Assert.NotEqual(
            valueObject1.GetHashCode(),
            valueObject2.GetHashCode());
    }

    [Fact]
    public void ValueObject_OperadorIgualdade_DeveRetornarTrue_QuandoAmbosForemNulos()
    {
        TestValueObject? valueObject1 = null;
        TestValueObject? valueObject2 = null;

        Assert.True(valueObject1 == valueObject2);
    }

    [Fact]
    public void ValueObject_OperadorIgualdade_DeveRetornarFalse_QuandoApenasEsquerdaForNula()
    {
        TestValueObject? valueObject1 = null;
        var valueObject2 = new TestValueObject("ABC", 123);

        Assert.False(valueObject1 == valueObject2);
    }

    [Fact]
    public void ValueObject_OperadorIgualdade_DeveRetornarFalse_QuandoApenasDireitaForNula()
    {
        var valueObject1 = new TestValueObject("ABC", 123);
        TestValueObject? valueObject2 = null;

        Assert.False(valueObject1 == valueObject2);
    }

    [Fact]
    public void ValueObject_OperadorIgualdade_DeveRetornarTrue_QuandoValoresForemIguais()
    {
        var valueObject1 = new TestValueObject("ABC", 123);
        var valueObject2 = new TestValueObject("ABC", 123);

        Assert.True(valueObject1 == valueObject2);
    }

    [Fact]
    public void ValueObject_OperadorIgualdade_DeveRetornarFalse_QuandoValoresForemDiferentes()
    {
        var valueObject1 = new TestValueObject("ABC", 123);
        var valueObject2 = new TestValueObject("XYZ", 456);

        Assert.False(valueObject1 == valueObject2);
    }

    [Fact]
    public void ValueObject_OperadorDiferente_DeveRetornarFalse_QuandoAmbosForemNulos()
    {
        TestValueObject? valueObject1 = null;
        TestValueObject? valueObject2 = null;

        Assert.False(valueObject1 != valueObject2);
    }

    [Fact]
    public void ValueObject_OperadorDiferente_DeveRetornarTrue_QuandoApenasEsquerdaForNula()
    {
        TestValueObject? valueObject1 = null;
        var valueObject2 = new TestValueObject("ABC", 123);

        Assert.True(valueObject1 != valueObject2);
    }

    [Fact]
    public void ValueObject_OperadorDiferente_DeveRetornarTrue_QuandoApenasDireitaForNula()
    {
        var valueObject1 = new TestValueObject("ABC", 123);
        TestValueObject? valueObject2 = null;

        Assert.True(valueObject1 != valueObject2);
    }

    [Fact]
    public void ValueObject_OperadorDiferente_DeveRetornarFalse_QuandoValoresForemIguais()
    {
        var valueObject1 = new TestValueObject("ABC", 123);
        var valueObject2 = new TestValueObject("ABC", 123);

        Assert.False(valueObject1 != valueObject2);
    }

    [Fact]
    public void ValueObject_OperadorDiferente_DeveRetornarTrue_QuandoValoresForemDiferentes()
    {
        var valueObject1 = new TestValueObject("ABC", 123);
        var valueObject2 = new TestValueObject("XYZ", 456);

        Assert.True(valueObject1 != valueObject2);
    }

    [Fact]
    public void ValueObject_Equals_DeveConsiderarTodosOsValoresAtomicos()
    {
        var valueObject1 = new TestValueObject("ABC", 123);
        var valueObject2 = new TestValueObject("ABC", 456);

        Assert.False(valueObject1.Equals(valueObject2));
    }

    [Fact]
    public void ValueObject_Equals_DeveConsiderarAOrdemDosValoresAtomicos()
    {
        var valueObject1 = new TestValueObject("ABC", 123);
        var valueObject2 = new TestValueObject("123", "ABC");

        Assert.False(valueObject1.Equals(valueObject2));
    }

    [Fact]
    public void ValueObject_GetHashCode_DeveConsiderarValoresNulos()
    {
        var valueObject1 = new TestValueObject(null, 123);
        var valueObject2 = new TestValueObject(null, 123);

        Assert.True(valueObject1.Equals(valueObject2));
        Assert.Equal(
            valueObject1.GetHashCode(),
            valueObject2.GetHashCode());
    }

    private sealed class TestValueObject : ValueObject
    {
        private readonly object? _valor1;
        private readonly object? _valor2;

        public TestValueObject(object? valor1, object? valor2)
        {
            _valor1 = valor1;
            _valor2 = valor2;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return _valor1!;
            yield return _valor2!;
        }
    }

    private sealed class OutroTestValueObject : ValueObject
    {
        private readonly object? _valor1;
        private readonly object? _valor2;

        public OutroTestValueObject(object? valor1, object? valor2)
        {
            _valor1 = valor1;
            _valor2 = valor2;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return _valor1!;
            yield return _valor2!;
        }
    }
}