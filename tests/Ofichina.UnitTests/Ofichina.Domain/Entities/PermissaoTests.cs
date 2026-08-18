using Ofichina.Domain.Entities;
using Ofichina.Domain.Exceptions;

namespace Ofichina.UnitTests.Domain.Entities;

public sealed class PermissaoTests
{
    [Fact]
    public void Deve_Criar_Permissao_Com_Valores_Informados()
    {
        var permissao = new Permissao("PERMISSAO_CADASTRAR", "Permite cadastrar registros");

        Assert.NotEqual(Guid.Empty, permissao.Id);
        Assert.Equal("PERMISSAO_CADASTRAR", permissao.Codigo);
        Assert.Equal("Permite cadastrar registros", permissao.Descricao);
    }

    [Fact]
    public void Deve_Atualizar_Permissao_Com_Valores_Informados()
    {
        var permissao = new Permissao("PERMISSAO_INICIAL", "Descrição inicial");

        permissao.Atualizar("PERMISSAO_ATUALIZADA", "Descrição atualizada");

        Assert.Equal("PERMISSAO_ATUALIZADA", permissao.Codigo);
        Assert.Equal("Descrição atualizada", permissao.Descricao);
    }

    [Fact]
    public void Deve_Rejeitar_Permissao_Com_Dados_Invalidos()
    {
        Assert.Throws<DomainException>(() => new Permissao(string.Empty, "Descrição"));
        Assert.Throws<DomainException>(() => new Permissao("PERMISSAO", string.Empty));
    }
}
