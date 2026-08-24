using Ofichina.Domain.Entities;

namespace Ofichina.UnitTests.TestInfrastructure.Builders;

public class PerfilBuilder
{
    private Perfil _perfil;

    public PerfilBuilder()
    {
        _perfil = TestDataFactory.Perfis.Criar();
    }

    public PerfilBuilder ComId(Guid id)
    {
        ReflectionHelpers.DefinirId(_perfil, id);
        return this;
    }

    public PerfilBuilder ComNome(string nome)
    {
        _perfil.AlterarNome(nome);
        return this;
    }

    public PerfilBuilder ComDescricao(string descricao)
    {
        _perfil.AlterarDescricao(descricao);
        return this;
    }

    public PerfilBuilder Desativar()
    {
        _perfil.Desativar();
        return this;
    }

    public Perfil Build() => _perfil;
}
