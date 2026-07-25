using Ofichina.Application.Abstractions;
using Ofichina.Application.UseCases.Veiculos.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Pessoa;
using Ofichina.Contracts.Responses.Veiculo;

namespace Ofichina.Application.UseCases.Veiculos.Handlers;

/// <summary>
/// Handler para listar veículos.
/// </summary>
public sealed class GetAllVeiculosByPessoaIdQueryHandler
    : IQueryHandler<GetVeiculosByPessoaIdQuery, Result<PessoaVeiculoResponse>>
{
    private readonly IPessoaRepository _pessoaRepository;
    private readonly ILogger<GetAllVeiculosByPessoaIdQueryHandler> _logger;

    public GetAllVeiculosByPessoaIdQueryHandler(
        IPessoaRepository pessoaRepository,
        ILogger<GetAllVeiculosByPessoaIdQueryHandler> logger)
    {
        _pessoaRepository = pessoaRepository;
        _logger = logger;
    }

    public async Task<Result<PessoaVeiculoResponse>> HandleAsync(
        GetVeiculosByPessoaIdQuery query,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var pessoa = await _pessoaRepository.GetByIdWithVeiculosAsync(query.PessoaId, cancellationToken);

            if (pessoa is null)
                return Result.Failure<PessoaVeiculoResponse>("Nenhum veículo encontrado para a pessoa.");

            var itens = pessoa.Veiculos.Where(v => !v.EstaExcluida()).ToList();

            if (itens.Count == 0)
                return Result.Failure<PessoaVeiculoResponse>("Nenhum veículo encontrado para a pessoa.");

            var response = new PessoaVeiculoResponse
            {
                Id = pessoa.Id,
                Nome = pessoa.Nome,
                Documento = pessoa.Documento.ToString(),
                Telefone = pessoa.Telefone.ToString(),
                Logradouro = pessoa.Endereco.Logradouro,
                Numero = pessoa.Endereco.Numero,
                Complemento = pessoa.Endereco.Complemento,
                Bairro = pessoa.Endereco.Bairro,
                Cidade = pessoa.Endereco.Cidade,
                Estado = pessoa.Endereco.Estado,
                Cep = pessoa.Endereco.Cep.ToString(),
                UsuarioId = pessoa.UsuarioId,
                Veiculo = itens.Select(v => new VeiculoResponse
                {
                    Id = v.Id,
                    Placa = v.Placa.ToString(),
                    Marca = v.Marca,
                    Modelo = v.Modelo,
                    AnoFabricacao = v.AnoFabricacao,
                    Cor = v.Cor,
                    Observacoes = v.Observacoes,
                    Hodometro = v.Hodometro.Valor,
                    HodometroFormatado = v.Hodometro.ToString(),
                    Ativo = !v.EstaExcluida(),
                    CreatedAt = v.CreatedAt,
                    UpdatedAt = v.UpdatedAt,
                    DeletedAt = v.DeletedAt
                }).ToList()
            };

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter veículos por Id da pessoa.");
            return Result.Failure<PessoaVeiculoResponse>("Não foi possível obter os veículos.");
        }
    }
}




