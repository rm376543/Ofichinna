using Bogus;
using Ofichina.Domain.Entities;
using Ofichina.Domain.ValueObjects;

namespace Ofichina.UnitTests.TestInfrastructure.Fakers;

public class PessoaFaker
{
    private readonly Faker _faker = new();

    public Pessoa Criar(Action<Pessoa>? customizar = null)
    {
        var nome = _faker.Name.FullName();
        var cpf = new Cpf(FakerHelpers.GerarCpfValido());
        var telefone = new Telefone(FakerHelpers.GerarTelefoneValido());
        var endereco = new Endereco(_faker.Address.StreetName(), _faker.Address.BuildingNumber(), _faker.Address.SecondaryAddress(), _faker.Address.County(), _faker.Address.City(), _faker.Address.StateAbbr(), new Cep(FakerHelpers.GerarCep()));
        var usuarioId = Guid.NewGuid();

        var pessoa = new Pessoa(nome, cpf, telefone, endereco, usuarioId);

        customizar?.Invoke(pessoa);

        return pessoa;
    }
}
