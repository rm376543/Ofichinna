using Ofichina.Domain.Exceptions;
using Ofichina.Domain.ValueObjects;

namespace Ofichina.UnitTests.Domain.ValueObjects;

public class HodometroTests
{
    [Fact]
    public void Hodometro_DeveInicializar_ComValorValido()
    {
        var hodometro = new Hodometro(10_000);

        Assert.Equal(10_000, hodometro.Valor);
    }

    [Fact]
    public void Hodometro_DeveAceitar_ValorZero()
    {
        var hodometro = new Hodometro(0);

        Assert.Equal(0, hodometro.Valor);
    }

    [Fact]
    public void Hodometro_DeveAceitar_ValorMaximoDeInteiro()
    {
        var hodometro = new Hodometro(int.MaxValue);

        Assert.Equal(int.MaxValue, hodometro.Valor);
    }

    [Fact]
    public void Hodometro_DeveLancarExcecao_QuandoValorForNegativo()
    {
        var exception = Assert.Throws<DomainException>(() =>
            new Hodometro(-1));

        Assert.Equal(
            "A quilometragem não pode ser negativa.",
            exception.Message);
    }

    [Fact]
    public void Hodometro_ToString_DeveRetornarValorFormatado()
    {
        var hodometro = new Hodometro(10_000);

        Assert.Equal(
            "10.000 km",
            hodometro.ToString());
    }

    [Fact]
    public void Hodometro_ToString_DeveFormatarMilhares()
    {
        var hodometro = new Hodometro(123_456);

        Assert.Equal(
            "123.456 km",
            hodometro.ToString());
    }

    [Fact]
    public void Hodometro_ToString_DeveRetornarZeroKm_QuandoValorForZero()
    {
        var hodometro = new Hodometro(0);

        Assert.Equal(
            "0 km",
            hodometro.ToString());
    }

    [Fact]
    public void Hodometro_DeveSerIgual_QuandoValoresForemIguais()
    {
        var hodometro1 = new Hodometro(10_000);
        var hodometro2 = new Hodometro(10_000);

        Assert.True(hodometro1.Equals(hodometro2));
        Assert.True(hodometro1 == hodometro2);
        Assert.False(hodometro1 != hodometro2);
    }

    [Fact]
    public void Hodometro_DeveSerDiferente_QuandoValoresForemDiferentes()
    {
        var hodometro1 = new Hodometro(10_000);
        var hodometro2 = new Hodometro(20_000);

        Assert.False(hodometro1.Equals(hodometro2));
        Assert.False(hodometro1 == hodometro2);
        Assert.True(hodometro1 != hodometro2);
    }

    [Fact]
    public void Hodometro_DeveGerarMesmoHashCode_QuandoValoresForemIguais()
    {
        var hodometro1 = new Hodometro(10_000);
        var hodometro2 = new Hodometro(10_000);

        Assert.Equal(
            hodometro1.GetHashCode(),
            hodometro2.GetHashCode());
    }

    [Fact]
    public void Hodometro_DeveSerDiferente_QuandoComparadoComNull()
    {
        var hodometro = new Hodometro(10_000);

        Assert.False(hodometro.Equals(null));
    }

    [Fact]
    public void Hodometro_DeveSerDiferente_QuandoComparadoComOutroTipo()
    {
        var hodometro = new Hodometro(10_000);
        var outroValueObject = new OutroValueObject();

        Assert.False(hodometro.Equals(outroValueObject));
    }

    [Fact]
    public void Hodometro_OperadorIgualdade_DeveRetornarTrue_QuandoAmbosForemNulos()
    {
        Hodometro? hodometro1 = null;
        Hodometro? hodometro2 = null;

        Assert.True(hodometro1 == hodometro2);
    }

    [Fact]
    public void Hodometro_OperadorIgualdade_DeveRetornarFalse_QuandoApenasEsquerdaForNula()
    {
        Hodometro? hodometro1 = null;
        var hodometro2 = new Hodometro(10_000);

        Assert.False(hodometro1 == hodometro2);
    }

    [Fact]
    public void Hodometro_OperadorIgualdade_DeveRetornarFalse_QuandoApenasDireitaForNula()
    {
        var hodometro1 = new Hodometro(10_000);
        Hodometro? hodometro2 = null;

        Assert.False(hodometro1 == hodometro2);
    }

    [Fact]
    public void Hodometro_OperadorDiferente_DeveRetornarTrue_QuandoApenasEsquerdaForNula()
    {
        Hodometro? hodometro1 = null;
        var hodometro2 = new Hodometro(10_000);

        Assert.True(hodometro1 != hodometro2);
    }

    [Fact]
    public void Hodometro_OperadorDiferente_DeveRetornarTrue_QuandoApenasDireitaForNula()
    {
        var hodometro1 = new Hodometro(10_000);
        Hodometro? hodometro2 = null;

        Assert.True(hodometro1 != hodometro2);
    }

    [Fact]
    public void Hodometro_ConstrutorSemParametros_DeveInicializarComZero()
    {
        var hodometro = new Hodometro();

        Assert.Equal(0, hodometro.Valor);
    }

    private sealed class OutroValueObject : ValueObject
    {
        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return 10_000;
        }
    }
}