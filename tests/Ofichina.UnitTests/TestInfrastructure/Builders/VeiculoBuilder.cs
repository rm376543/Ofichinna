using Ofichina.Domain.Entities;
using Ofichina.Domain.ValueObjects;

namespace Ofichina.UnitTests.TestInfrastructure.Builders;

public class VeiculoBuilder
{
    private Veiculo _veiculo;

    public VeiculoBuilder()
    {
        _veiculo = TestDataFactory.Veiculos.Criar();
    }

    public VeiculoBuilder ComId(Guid id)
    {
        ReflectionHelpers.DefinirId(_veiculo, id);
        return this;
    }

    public VeiculoBuilder ComPessoaId(Guid pessoaId)
    {
        _veiculo.AlterarPessoa(pessoaId);
        return this;
    }

    public VeiculoBuilder ComPlaca(string placa)
    {
        _veiculo.AlterarPlaca(new Placa(placa));
        return this;
    }

    public VeiculoBuilder ComMarca(string marca)
    {
        _veiculo.AlterarMarca(marca);
        return this;
    }

    public VeiculoBuilder ComModelo(string modelo)
    {
        _veiculo.AlterarModelo(modelo);
        return this;
    }

    public VeiculoBuilder ComAno(int ano)
    {
        _veiculo.AlterarAnoFabricacao(ano);
        return this;
    }

    public VeiculoBuilder ComCor(string cor)
    {
        _veiculo.AlterarCor(cor);
        return this;
    }

    public VeiculoBuilder ComHodometro(int valor)
    {
        _veiculo.AlterarHodometro(new Hodometro(valor));
        return this;
    }

    public Veiculo Build() => _veiculo;
}
