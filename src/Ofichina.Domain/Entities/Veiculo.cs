using Ofichina.Domain.Exceptions;
using Ofichina.Domain.ValueObjects;

namespace Ofichina.Domain.Entities
{
    public class Veiculo : Entity
    {
        public Guid ClienteId { get; private set; } = Guid.Empty;

        public Placa Placa { get; private set; } = null!;

        public string Marca { get; private set; } = null!;

        public string Modelo { get; private set; } = null!;

        public int Ano { get; private set; } = 0;

        public Cliente Cliente { get; private set; } = null!;

        private Veiculo()
        {
            // Necessário para o Entity Framework
        }

        public Veiculo(
            Guid clienteId,
            Placa placa,
            string marca,
            string modelo,
            int ano)
        {
            if (clienteId == Guid.Empty)
                throw new DomainException("O cliente deve ser informado.");

            if (placa is null)
                throw new DomainException("A placa deve ser informada.");

            if (string.IsNullOrWhiteSpace(marca))
                throw new DomainException("A marca deve ser informada.");

            if (string.IsNullOrWhiteSpace(modelo))
                throw new DomainException("O modelo deve ser informado.");

            var anoAtual = DateTime.Now.Year + 1;

            if (ano < 1900 || ano > anoAtual)
                throw new DomainException("Ano do veículo inválido.");

            ClienteId = clienteId;
            Placa = placa;
            Marca = marca.Trim();
            Modelo = modelo.Trim();
            Ano = ano;
        }

        public void AlterarPlaca(Placa novaPlaca)
        {
            if (novaPlaca is null)
                throw new DomainException("A placa deve ser informada.");

            Placa = novaPlaca;
        }

        public void AlterarModelo(string modelo)
        {
            if (string.IsNullOrWhiteSpace(modelo))
                throw new DomainException("O modelo deve ser informado.");

            Modelo = modelo.Trim();
        }

        public void AlterarMarca(string marca)
        {
            if (string.IsNullOrWhiteSpace(marca))
                throw new DomainException("A marca deve ser informada.");

            Marca = marca.Trim();
        }

        public void AlterarAno(int ano)
        {
            var anoAtual = DateTime.Now.Year + 1;

            if (ano < 1900 || ano > anoAtual)
                throw new DomainException("Ano do veículo inválido.");

            Ano = ano;
        }

        public void TransferirPara(Guid novoClienteId)
        {
            if (novoClienteId == Guid.Empty)
                throw new DomainException("O cliente deve ser informado.");

            ClienteId = novoClienteId;
        }
    }
}