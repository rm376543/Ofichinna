using Ofichina.Domain.Exceptions;
using Ofichina.Domain.ValueObjects;

namespace Ofichina.Domain.Entities;

/// <summary>
/// Representa um veículo associado a uma pessoa no domínio da oficina.
/// </summary>
public sealed class Veiculo : Entity
{
    /// <summary>
    /// Identificador da pessoa proprietária do veículo.
    /// </summary>
    public Guid PessoaId { get; private set; } = Guid.Empty;

    /// <summary>
    /// Placa do veículo.
    /// </summary>
    public Placa Placa { get; private set; } = null!;

    /// <summary>
    /// Marca do veículo.
    /// </summary>
    public string Marca { get; private set; } = null!;

    /// <summary>
    /// Modelo do veículo.
    /// </summary>
    public string Modelo { get; private set; } = null!;

    /// <summary>
    /// Ano de fabricação do veículo.
    /// </summary>
    public int AnoFabricacao { get; private set; } = 0;

    /// <summary>
    /// Cor do veículo.
    /// </summary>
    public string Cor { get; private set; } = string.Empty;

    /// <summary>
    /// Hodômetro atual do veículo.
    /// </summary>
    public Hodometro Hodometro { get; private set; } = null!;

    /// <summary>
    /// Navegação para a pessoa proprietária.
    /// </summary>
    public Pessoa Pessoa { get; private set; } = null!;

    private Veiculo()
    {
    }

    /// <summary>
    /// Cria um novo veículo para uma pessoa.
    /// </summary>
    /// <param name="pessoaId">Identificador da pessoa proprietária.</param>
    /// <param name="placa">Placa do veículo.</param>
    /// <param name="marca">Marca do veículo.</param>
    /// <param name="modelo">Modelo do veículo.</param>
    /// <param name="anoFabricacao">Ano de fabricação.</param>
    /// <param name="cor">Cor do veículo.</param>
    /// <param name="quilometragem">Hodômetro do veículo.</param>
    public Veiculo(
        Guid pessoaId,
        Placa placa,
        string marca,
        string modelo,
        int anoFabricacao,
        string cor,
        Hodometro quilometragem)
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
        Hodometro = quilometragem;
    }

    /// <summary>
    /// Altera a pessoa proprietária do veículo.
    /// </summary>
    public void AlterarPessoa(Guid novaPessoaId)
    {
        if (novaPessoaId == Guid.Empty)
            throw new DomainException("A pessoa deve ser informada.");

        PessoaId = novaPessoaId;
        AtualizarDataModificacao();
    }

    /// <summary>
    /// Altera a placa do veículo.
    /// </summary>
    public void AlterarPlaca(Placa novaPlaca)
    {
        if (novaPlaca is null)
            throw new DomainException("A placa deve ser informada.");

        Placa = novaPlaca;
        AtualizarDataModificacao();
    }

    /// <summary>
    /// Altera o modelo do veículo.
    /// </summary>
    public void AlterarModelo(string modelo)
    {
        if (string.IsNullOrWhiteSpace(modelo))
            throw new DomainException("O modelo deve ser informado.");

        Modelo = modelo.Trim();
        AtualizarDataModificacao();
    }

    /// <summary>
    /// Altera a marca do veículo.
    /// </summary>
    public void AlterarMarca(string marca)
    {
        if (string.IsNullOrWhiteSpace(marca))
            throw new DomainException("A marca deve ser informada.");

        Marca = marca.Trim();
        AtualizarDataModificacao();
    }

    /// <summary>
    /// Altera o ano de fabricação do veículo.
    /// </summary>
    public void AlterarAnoFabricacao(int anoFabricacao)
    {
        var anoAtual = DateTime.Now.Year + 1;

        if (anoFabricacao < 1900 || anoFabricacao > anoAtual)
            throw new DomainException("Ano do veículo inválido.");

        AnoFabricacao = anoFabricacao;
        AtualizarDataModificacao();
    }

    /// <summary>
    /// Altera a cor do veículo.
    /// </summary>
    public void AlterarCor(string? cor)
    {
        Cor = string.IsNullOrWhiteSpace(cor) ? string.Empty : cor.Trim();
        AtualizarDataModificacao();
    }

    /// <summary>
    /// Altera o hodômetro do veículo.
    /// </summary>
    public void AlterarHodometro(Hodometro quilometragem)
    {
        if (quilometragem is null)
            throw new DomainException("A quilometragem deve ser informada.");

        Hodometro = quilometragem;
        AtualizarDataModificacao();
    }

    /// <summary>
    /// Reativa o veículo caso ele tenha sido desativado logicamente.
    /// </summary>
    public void Ativar()
    {
        Reativar();
    }

    /// <summary>
    /// Desativa logicamente o veículo.
    /// </summary>
    public void Desativar()
    {
        if (EstaExcluida())
            return;

        Excluir();
    }
}
