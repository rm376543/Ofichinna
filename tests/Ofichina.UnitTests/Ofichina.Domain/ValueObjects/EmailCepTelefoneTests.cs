using Ofichina.Domain.Exceptions;
using Ofichina.Domain.ValueObjects;

namespace Ofichina.UnitTests.Domain.ValueObjects;

public sealed class EmailCepTelefoneTests
{
    [Fact]
    public void Email_Deve_Normalizar_Para_Minuscule_E_Validar_Formato()
    {
        var email = new Email("  Usuario@Ofichina.COM.BR  ");

        Assert.Equal("usuario@ofichina.com.br", email.Value);
        Assert.Equal("usuario@ofichina.com.br", email.ToString());
    }

    [Fact]
    public void Email_Deve_Rejeitar_Valores_Invalidos()
    {
        Assert.Throws<DomainException>(() => new Email(string.Empty));
        Assert.Throws<DomainException>(() => new Email("nao-e-mail"));
    }

    [Fact]
    public void Cep_Deve_Normalizar_E_Formatar()
    {
        var cep = new Cep("01310-100");

        Assert.Equal("01310100", cep.Value);
        Assert.Equal("01310-100", cep.Formatado);
        Assert.Equal("01310-100", cep.ToString());
    }

    [Fact]
    public void Cep_Deve_Rejeitar_Valores_Invalidos()
    {
        Assert.Throws<DomainException>(() => new Cep(string.Empty));
        Assert.Throws<DomainException>(() => new Cep("123"));
    }

    [Fact]
    public void Telefone_Deve_Normalizar_E_Formatar()
    {
        var celular = new Telefone("(11) 99999-9999");
        var fixo = new Telefone("11 3333-4444");

        Assert.Equal("11999999999", celular.Value);
        Assert.Equal("(11) 99999-9999", celular.Formatado);
        Assert.Equal("(11) 3333-4444", fixo.Formatado);
    }

    [Fact]
    public void Telefone_Deve_Rejeitar_Valores_Invalidos()
    {
        Assert.Throws<DomainException>(() => new Telefone(string.Empty));
        Assert.Throws<DomainException>(() => new Telefone("1012345678"));
    }
}