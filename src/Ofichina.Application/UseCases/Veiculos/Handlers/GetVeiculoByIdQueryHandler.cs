using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Veiculos.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Veiculo;
using Ofichina.Domain.Interfaces;

namespace Ofichina.Application.UseCases.Veiculos.Handlers;

/// <summary>
/// Handler para obter um veÃ­culo por Id.
/// </summary>
public sealed class GetVeiculoByIdQueryHandler : IQueryHandler<GetVeiculoByIdQuery, Result<VeiculoResponse>>
{
    private readonly IVeiculoRepository _veiculoRepository;
    private readonly ILogger<GetVeiculoByIdQueryHandler> _logger;

    public GetVeiculoByIdQueryHandler(
        IVeiculoRepository veiculoRepository,
        ILogger<GetVeiculoByIdQueryHandler> logger)
    {
        _veiculoRepository = veiculoRepository;
        _logger = logger;
    }

    public async Task<Result<VeiculoResponse>> HandleAsync(GetVeiculoByIdQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var veiculo = await _veiculoRepository.GetByIdWithPessoaAsync(query.Id, cancellationToken);

            if (veiculo is null || veiculo.EstaExcluida())
                return Result.Failure<VeiculoResponse>("VeÃ­culo nÃ£o encontrado.");

            return Result.Success(Mapear(veiculo));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter veÃ­culo por Id. VeiculoId: {VeiculoId}", query.Id);
            return Result.Failure<VeiculoResponse>("NÃ£o foi possÃ­vel obter o veÃ­culo.");
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
