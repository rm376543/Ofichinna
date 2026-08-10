using Ofichina.Application.Abstractions;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Application.UseCases.Veiculos.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Pessoa;
using Ofichina.Contracts.Responses.Veiculo;

namespace Ofichina.Application.UseCases.Veiculos.Handlers;

/// <summary>
/// Handler para obter os veículos vinculados a uma pessoa.
/// </summary>
public sealed class GetVeiculosByPessoaIdQueryHandler
    : IQueryHandler<GetVeiculosByPessoaIdQuery, Result<PessoaVeiculoResponse>>
{
    private readonly ILogger<GetVeiculosByPessoaIdQueryHandler> _logger;
    private readonly IPessoaRepository _pessoaRepository;

    public GetVeiculosByPessoaIdQueryHandler(
        ILogger<GetVeiculosByPessoaIdQueryHandler> logger,
        IPessoaRepository pessoaRepository)
    {
        _logger = logger;
        _pessoaRepository = pessoaRepository;
    }

    public async Task<Result<PessoaVeiculoResponse>> HandleAsync(
        GetVeiculosByPessoaIdQuery query,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var pessoa = await _pessoaRepository.GetByIdAsync(query.PessoaId, includeVeiculos: true, cancellationToken);

            if (pessoa is null)
            {
                _logger.LogWarning("Pessoa com Id: {PessoaId} não encontrada.", query.PessoaId);
                return Result.Failure<PessoaVeiculoResponse>($"Pessoa com Id: {query.PessoaId} não encontrada.");
            }

            var response = new PessoaVeiculoResponse
            {
                PessoaId = pessoa.Id,
                Nome = pessoa.Nome,
                Telefone = pessoa.Telefone.Value,
                Veiculo = pessoa.Veiculos.Select(v => new VeiculoResponse
                {
                    VeiculoId = v.Id,
                    Placa = v.Placa.ToString(),
                    Marca = v.Marca,
                    Modelo = v.Modelo,
                    AnoFabricacao = v.AnoFabricacao,
                    Cor = v.Cor,
                    Hodometro = v.Hodometro.Valor,
                    HodometroFormatado = v.Hodometro.ToString(),
                }).ToList()
            };

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Ocorreu um erro ao processar a solicitação para a pessoa com Id: {PessoaId}",
                query.PessoaId);

            return Result.Failure<PessoaVeiculoResponse>("Ocorreu um erro ao processar a solicitação.");
        }
    }
}