using Microsoft.Extensions.Logging;
using Ofichina.Application.Abstractions;
using Ofichina.Application.Abstractions.Interfaces;
using Ofichina.Application.UseCases.Veiculos.Queries;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses;
using Ofichina.Contracts.Responses.Veiculo;
using Ofichina.Domain.Common;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Exceptions;

namespace Ofichina.Application.UseCases.Veiculos.Handlers
{
    /// <summary>
    /// Handler para obter veículos por Id da pessoa.
    /// </summary>
    public sealed class GetVeiculosByPessoaIdQueryHandler
    : IQueryHandler<GetVeiculosByPessoaIdQuery, Result<PagedResponse<VeiculoListResponse>>>
    {
        private readonly IVeiculoRepository _veiculoRepository;
        private readonly ILogger<GetVeiculosByPessoaIdQueryHandler> _logger;

        public GetVeiculosByPessoaIdQueryHandler(
            IVeiculoRepository veiculoRepository,
            ILogger<GetVeiculosByPessoaIdQueryHandler> logger)
        {
            _veiculoRepository = veiculoRepository;
            _logger = logger;
        }

        public async Task<Result<PagedResponse<VeiculoListResponse>>> HandleAsync(
            GetVeiculosByPessoaIdQuery query,
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Iniciando a obtenção de veículos para a pessoa com Id: {PessoaId}", query.PessoaId);
                var pagination = new Pagination(query.PageNumber, query.PageSize);

                var veiculo = await _veiculoRepository.GetVeiclesPagedByPessoaIdAsync(query.PessoaId, pagination, cancellationToken);

                var resultado = veiculo.Items
                .Select(Mapear)
                .ToList();

                _logger.LogInformation("Obtenção de veículos para a pessoa com Id: {PessoaId} concluída com sucesso.", query.PessoaId);
                var pagedResponse = new PagedResponse<VeiculoListResponse>
                {
                    Items = resultado,
                    PageNumber = veiculo.PageNumber,
                    PageSize = veiculo.PageSize,
                    TotalCount = veiculo.TotalCount,
                    TotalPages = veiculo.TotalPages,
                    HasNextPage = veiculo.HasNextPage,
                    HasPreviousPage = veiculo.HasPreviousPage
                };
                return Result.Success(pagedResponse);
            }
            catch (DomainException ex)
            {
                _logger.LogError(ex, "Erro de domínio ao obter veículos por Id da pessoa.");
                return Result<PagedResponse<VeiculoListResponse>>.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao obter veículos por Id da pessoa.");
                return Result<PagedResponse<VeiculoListResponse>>.Failure("Ocorreu um erro inesperado.");
            }
        }

        private static VeiculoListResponse Mapear(Veiculo veiculo)
        {
            return new VeiculoListResponse
            {
                Id = veiculo.Id,
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
}
