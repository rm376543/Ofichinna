using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Veiculos.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Veiculo;
using Ofichina.Domain.Interfaces;

namespace Ofichina.Application.UseCases.Veiculos.Handlers;

/// <summary>
/// Handler para listar veÃ­culos.
/// </summary>
public sealed class GetVeiculosQueryHandler : IQueryHandler<GetVeiculosQuery, Result<IReadOnlyCollection<VeiculoResponse>>>
{
    private readonly IVeiculoRepository _veiculoRepository;
    private readonly ILogger<GetVeiculosQueryHandler> _logger;

    public GetVeiculosQueryHandler(
        IVeiculoRepository veiculoRepository,
        ILogger<GetVeiculosQueryHandler> logger)
    {
        _veiculoRepository = veiculoRepository;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyCollection<VeiculoResponse>>> HandleAsync(GetVeiculosQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var veiculos = await _veiculoRepository.GetPagedWithPessoaAsync(query.Pagination, cancellationToken);

            var resultado = veiculos.Items
                .Select(Mapear)
                .ToList();

            return Result.Success<IReadOnlyCollection<VeiculoResponse>>(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar veÃ­culos.");
            return Result.Failure<IReadOnlyCollection<VeiculoResponse>>("NÃ£o foi possÃ­vel obter os veÃ­culos.");
        }
    }

    private static VeiculoResponse Mapear(Domain.Entities.Veiculo veiculo)
    {
        return new VeiculoResponse
        {
            Id = veiculo.Id,
            Pessoa = new VeiculoPessoaResponse
            {
                Nome = veiculo.Pessoa.Nome,
                Documento = veiculo.Pessoa.Documento.ToString(),
                Telefone = veiculo.Pessoa.Telefone.ToString(),
                Ativo = !veiculo.Pessoa.EstaExcluida(),
                CreatedAt = veiculo.Pessoa.CreatedAt,
                UpdatedAt = veiculo.Pessoa.UpdatedAt,
                DeletedAt = veiculo.Pessoa.DeletedAt
            },
            Placa = veiculo.Placa.ToString(),
            Marca = veiculo.Marca,
            Modelo = veiculo.Modelo,
            AnoFabricacao = veiculo.AnoFabricacao,
            Cor = veiculo.Cor,
            Observacoes = veiculo.Observacoes,
            Hodometro = veiculo.Hodometro.Valor,
            HodometroFormatada = veiculo.Hodometro.ToString(),
            Ativo = !veiculo.EstaExcluida(),
            CreatedAt = veiculo.CreatedAt,
            UpdatedAt = veiculo.UpdatedAt,
            DeletedAt = veiculo.DeletedAt
        };
    }
}
