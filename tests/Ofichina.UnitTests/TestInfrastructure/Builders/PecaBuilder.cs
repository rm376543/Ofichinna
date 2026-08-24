using Ofichina.Domain.Entities;

namespace Ofichina.UnitTests.TestInfrastructure.Builders;

public class PecaBuilder
{
    private readonly Peca _peca;

    public PecaBuilder()
    {
        _peca = TestDataFactory.Pecas.Criar();
    }

    public PecaBuilder ComId(Guid id)
    {
        ReflectionHelpers.DefinirId(_peca, id);
        return this;
    }

    public PecaBuilder ComNome(string nome)
    {
        _peca.AtualizarDados(nome, _peca.Descricao, _peca.Codigo, _peca.Valor, _peca.QuantidadeEstoque);
        return this;
    }

    public PecaBuilder ComCodigo(string codigo)
    {
        _peca.AtualizarDados(_peca.Nome, _peca.Descricao, codigo, _peca.Valor, _peca.QuantidadeEstoque);
        return this;
    }

    public PecaBuilder ComValor(decimal valor)
    {
        _peca.AtualizarDados(_peca.Nome, _peca.Descricao, _peca.Codigo, valor, _peca.QuantidadeEstoque);
        return this;
    }

    public PecaBuilder ComQuantidadeEstoque(int quantidade)
    {
        _peca.AtualizarDados(_peca.Nome, _peca.Descricao, _peca.Codigo, _peca.Valor, quantidade);
        return this;
    }

    public Peca Build() => _peca;
}
