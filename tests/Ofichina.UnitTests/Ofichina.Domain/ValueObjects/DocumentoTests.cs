using Ofichina.Domain.Exceptions;
using Ofichina.Domain.ValueObjects;

namespace Ofichina.UnitTests.Domain.ValueObjects;

public class DocumentoTests
{
    [Fact]
    public void Documento_DeveInicializar_ComNumeroValido()
    {
        var documento = new TestDocumentoCpf("12345678909");

        Assert.Equal("12345678909", documento.Numero);
        Assert.Equal(TipoDocumento.CPF, documento.Tipo);
    }

    [Fact]
    public void Documento_DeveRemoverEspacosDasExtremidades()
    {
        var documento = new TestDocumentoCpf("  12345678909  ");

        Assert.Equal("12345678909", documento.Numero);
    }

    [Fact]
    public void Documento_DeveLancarExcecao_QuandoNumeroForNulo()
    {
        var exception = Assert.Throws<DomainException>(() =>
            new TestDocumentoCpf(null!));

        Assert.Equal(
            "Número do documento é obrigatório.",
            exception.Message);
    }

    [Fact]
    public void Documento_DeveLancarExcecao_QuandoNumeroForVazio()
    {
        var exception = Assert.Throws<DomainException>(() =>
            new TestDocumentoCpf(string.Empty));

        Assert.Equal(
            "Número do documento é obrigatório.",
            exception.Message);
    }

    [Fact]
    public void Documento_DeveLancarExcecao_QuandoNumeroContiverApenasEspacos()
    {
        var exception = Assert.Throws<DomainException>(() =>
            new TestDocumentoCpf("   "));

        Assert.Equal(
            "Número do documento é obrigatório.",
            exception.Message);
    }

    [Fact]
    public void Documento_ToString_DeveRetornarNumero()
    {
        var documento = new TestDocumentoCpf("12345678909");

        Assert.Equal("12345678909", documento.ToString());
    }

    [Fact]
    public void Documento_DeveSerIgual_QuandoNumeroETipoForemIguais()
    {
        var documento1 = new TestDocumentoCpf("12345678909");
        var documento2 = new TestDocumentoCpf("12345678909");

        Assert.True(documento1.Equals(documento2));
        Assert.True(documento1 == documento2);
        Assert.False(documento1 != documento2);
    }

    [Fact]
    public void Documento_DeveSerDiferente_QuandoNumerosForemDiferentes()
    {
        var documento1 = new TestDocumentoCpf("12345678909");
        var documento2 = new TestDocumentoCpf("98765432100");

        Assert.False(documento1.Equals(documento2));
        Assert.False(documento1 == documento2);
        Assert.True(documento1 != documento2);
    }

    [Fact]
    public void Documento_DeveSerDiferente_QuandoTiposForemDiferentes()
    {
        var cpf = new TestDocumentoCpf("12345678909");
        var cnpj = new TestDocumentoCnpj("12345678909");

        Assert.False(cpf.Equals(cnpj));
        Assert.False(cpf == cnpj);
        Assert.True(cpf != cnpj);
    }

    [Fact]
    public void Documento_DeveGerarMesmoHashCode_QuandoForIgual()
    {
        var documento1 = new TestDocumentoCpf("12345678909");
        var documento2 = new TestDocumentoCpf("12345678909");

        Assert.Equal(
            documento1.GetHashCode(),
            documento2.GetHashCode());
    }

    [Fact]
    public void Documento_DeveSerDiferente_QuandoComparadoComNull()
    {
        var documento = new TestDocumentoCpf("12345678909");

        Assert.False(documento.Equals(null));
    }

    [Fact]
    public void Documento_DeveSerDiferente_QuandoComparadoComOutroTipo()
    {
        var documento = new TestDocumentoCpf("12345678909");

        var outroObjeto = new OutroValueObject();

        Assert.False(documento.Equals(outroObjeto));
    }

    [Fact]
    public void Documento_Cpf_DeveRetornarTipoCpf()
    {
        var documento = new TestDocumentoCpf("12345678909");

        Assert.Equal(TipoDocumento.CPF, documento.Tipo);
    }

    [Fact]
    public void Documento_Cnpj_DeveRetornarTipoCnpj()
    {
        var documento = new TestDocumentoCnpj("12345678000195");

        Assert.Equal(TipoDocumento.CNPJ, documento.Tipo);
    }

    [Fact]
    public void Documento_Cpf_DevePreservarNumeroNoToString()
    {
        var documento = new TestDocumentoCpf("  12345678909  ");

        Assert.Equal(documento.Numero, documento.ToString());
        Assert.Equal("12345678909", documento.ToString());
    }

    private sealed class TestDocumentoCpf : Documento
    {
        public TestDocumentoCpf(string numero)
            : base(numero)
        {
        }

        public override TipoDocumento Tipo => TipoDocumento.CPF;
    }

    private sealed class TestDocumentoCnpj : Documento
    {
        public TestDocumentoCnpj(string numero)
            : base(numero)
        {
        }

        public override TipoDocumento Tipo => TipoDocumento.CNPJ;
    }

    private sealed class OutroValueObject : ValueObject
    {
        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return "Outro";
        }
    }
}