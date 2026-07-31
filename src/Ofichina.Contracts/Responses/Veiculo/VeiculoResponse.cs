using Ofichina.Contracts.Common;

namespace Ofichina.Contracts.Responses.Veiculo;

/// <summary>
/// Resposta resumida de veículo, sem dados da pessoa.
/// </summary>
public sealed class VeiculoResponse : BaseEntity
{
    /// <summary>
    /// Número da placa do veículo.
    /// </summary>
    public string Placa { get; set; } = string.Empty;

    /// <summary>
    /// Marca do veículo.
    /// </summary>
    public string Marca { get; set; } = string.Empty;

    /// <summary>
    /// Modelo do veículo.
    /// </summary>
    public string Modelo { get; set; } = string.Empty;

    /// <summary>
    /// Ano de fabricação do veículo.
    /// </summary>
    public int AnoFabricacao { get; set; }

    /// <summary>
    /// Cor do veículo.
    /// </summary>
    public string Cor { get; set; } = string.Empty;

    /// <summary>
    /// Hodômetro do veículo.
    /// </summary>
    public int Hodometro { get; set; }

    /// <summary>
    /// Hodômetro formatado do veículo.
    /// </summary>
    public string HodometroFormatado { get; set; } = string.Empty;

    public VeiculoResponse()
    {

    }

    /// <summary>
    /// Construtor da classe VeiculoResponse.
    /// </summary>
    /// <param name="placa"></param>
    /// <param name="marca"></param>
    /// <param name="modelo"></param>
    /// <param name="anoFabricacao"></param>
    /// <param name="cor"></param>
    /// <param name="hodometro"></param>
    /// <param name="hodometroFormatado"></param>
    public VeiculoResponse(
        string placa,
        string marca,
        string modelo,
        int anoFabricacao,
        string cor,
        int hodometro,
        string hodometroFormatado
    )
    {
        Placa = placa;
        Marca = marca;
        Modelo = modelo;
        AnoFabricacao = anoFabricacao;
        Cor = cor;
        Hodometro = hodometro;
        HodometroFormatado = hodometroFormatado;

        ValidaVeiculo();
    }

    /// <summary>
    /// Valida os dados do veículo, lançando exceções caso algum campo seja inválido.
    /// </summary>
    /// <exception cref="ArgumentException"></exception>
    private void ValidaVeiculo()
    {
        if (string.IsNullOrEmpty(Placa))
        {
            throw new ArgumentException("Placa inválida.");
        }

        if (string.IsNullOrEmpty(Marca))
        {
            throw new ArgumentException("Marca inválida.");
        }

        if (string.IsNullOrEmpty(Cor))
        {
            throw new ArgumentException("Cor inválida.");
        }

        if (string.IsNullOrEmpty(Modelo))
        {
            throw new ArgumentException("Modelo inválido.");
        }

        if (AnoFabricacao <= 0)
        {
            throw new ArgumentException("Ano de fabricação inválido.");
        }

        if (Hodometro < 0)
        {
            throw new ArgumentException("Hodômetro inválido.");
        }

    }
}