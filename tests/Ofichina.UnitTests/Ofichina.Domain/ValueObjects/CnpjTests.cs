using Ofichina.Domain.Exceptions;
using Ofichina.Domain.ValueObjects;

namespace Ofichina.UnitTests.Domain.ValueObjects;

public sealed class CnpjTests
{
    [Fact]
    public void Deve_Normalizar_E_Manter_Cnpj_Valido()
    {
        var cnpj = new Cnpj("04.252.011/0001-10");

        Assert.Equal("04252011000110", cnpj.Numero);
        Assert.Equal("04252011000110", cnpj.ToString());
        Assert.Equal(TipoDocumento.CNPJ, cnpj.Tipo);
    }

    [Fact]
    public void Deve_Rejeitar_Cnpj_Invalido()
    {
        Assert.Throws<DomainException>(() => new Cnpj(string.Empty));
        Assert.Throws<DomainException>(() => new Cnpj("11.111.111/1111-11"));
    }
}