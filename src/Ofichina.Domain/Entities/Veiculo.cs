using Ofichina.Domain.Exceptions;
using Ofichina.Domain.ValueObjects;

namespace Ofichina.Domain.Entities
{
    public class Veiculo : Entity
    {
        public Guid PessoaId { get; private set; } = Guid.Empty;

        public Placa Placa { get; private set; } = null!;

        public string Marca { get; private set; } = null!;

        public string Modelo { get; private set; } = null!;

        public int AnoFabricacao { get; private set; } = 0;

        public string Cor { get; private set; } = string.Empty;

        public string? Observacoes { get; private set; }

        public Hodometro Hodometro { get; private set; } = null!;

        public bool Ativo { get; private set; } = true;

        public Pessoa Pessoa { get; private set; } = null!;

        private Veiculo()
        {
            // Necessário para o Entity Framework
        }

#pragma warning disable S107
        public Veiculo(
            Guid pessoaId,
            Placa placa,
            string marca,
            string modelo,
            int anoFabricacao,
            string cor,
            string? observacoes,
            Hodometro quilometragem,
            bool ativo)
#pragma warning restore S107
        {
            if (pessoaId == Guid.Empty)
                throw new DomainException("A pessoa deve ser informada.");

            if (placa is null)
                throw new DomainException("A placa deve ser informada.");

            if (string.IsNullOrWhiteSpace(marca))
                throw new DomainException("A marca deve ser informada.");

            if (string.IsNullOrWhiteSpace(modelo))
                throw new DomainException("O modelo deve ser informado.");

            var anoAtual = DateTime.Now.Year + 1;

            if (anoFabricacao < 1900 || anoFabricacao > anoAtual)
                throw new DomainException("Ano do veículo inválido.");

            if (quilometragem is null)
                throw new DomainException("A quilometragem deve ser informada.");

            PessoaId = pessoaId;
            Placa = placa;
            Marca = marca.Trim();
            Modelo = modelo.Trim();
            AnoFabricacao = anoFabricacao;
            Cor = string.IsNullOrWhiteSpace(cor) ? string.Empty : cor.Trim();
            Observacoes = string.IsNullOrWhiteSpace(observacoes) ? null : observacoes.Trim();
            Hodometro = quilometragem;
            Ativo = ativo;
        }

        public void AlterarPessoa(Guid novaPessoaId)
        {
            if (novaPessoaId == Guid.Empty)
                throw new DomainException("A pessoa deve ser informada.");

            PessoaId = novaPessoaId;
            AtualizarDataModificacao();
        }

        public void AlterarPlaca(Placa novaPlaca)
        {
            if (novaPlaca is null)
                throw new DomainException("A placa deve ser informada.");

            Placa = novaPlaca;
            AtualizarDataModificacao();
        }

        public void AlterarModelo(string modelo)
        {
            if (string.IsNullOrWhiteSpace(modelo))
                throw new DomainException("O modelo deve ser informado.");

            Modelo = modelo.Trim();
            AtualizarDataModificacao();
        }

        public void AlterarMarca(string marca)
        {
            if (string.IsNullOrWhiteSpace(marca))
                throw new DomainException("A marca deve ser informada.");

            Marca = marca.Trim();
            AtualizarDataModificacao();
        }

        public void AlterarAnoFabricacao(int anoFabricacao)
        {
            var anoAtual = DateTime.Now.Year + 1;

            if (anoFabricacao < 1900 || anoFabricacao > anoAtual)
                throw new DomainException("Ano do veículo inválido.");

            AnoFabricacao = anoFabricacao;
            AtualizarDataModificacao();
        }

        public void AlterarCor(string? cor)
        {
            Cor = string.IsNullOrWhiteSpace(cor) ? string.Empty : cor.Trim();
            AtualizarDataModificacao();
        }

        public void AlterarObservacoes(string? observacoes)
        {
            Observacoes = string.IsNullOrWhiteSpace(observacoes) ? null : observacoes.Trim();
            AtualizarDataModificacao();
        }

        public void AlterarHodometro(Hodometro quilometragem)
        {
            if (quilometragem is null)
                throw new DomainException("A quilometragem deve ser informada.");

            Hodometro = quilometragem;
            AtualizarDataModificacao();
        }

        public void Ativar()
        {
            if (Ativo)
                return;

            Ativo = true;
            AtualizarDataModificacao();
        }

        public void Desativar()
        {
            if (!Ativo)
                return;

            Ativo = false;
            AtualizarDataModificacao();
        }
    }
}