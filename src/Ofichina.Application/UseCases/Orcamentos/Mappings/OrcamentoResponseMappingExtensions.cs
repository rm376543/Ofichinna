using Ofichina.Contracts.Extension;
using Ofichina.Contracts.Responses.Orcamento;
using Ofichina.Contracts.Responses.OrdemServico;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Common;
using Ofichina.Domain.Entities;

namespace Ofichina.Application.UseCases.Orcamentos.Mappings;

public static class OrcamentoResponseMappingExtensions
{
    public static OrcamentoResponse ToResponse(this Orcamento orcamento)
    {
        ArgumentNullException.ThrowIfNull(orcamento);

        return new OrcamentoResponse
        {
            OrcamentoId = orcamento.Id,
            PessoaId = orcamento.PessoaId,
            VeiculoId = orcamento.VeiculoId,
            AgendamentoId = orcamento.AgendamentoId,
            MecanicoId = orcamento.MecanicoId,
            ConsultorId = orcamento.ConsultorId,
            DataValidade = DateOnly.FromDateTime(orcamento.DataValidade),
            Desconto = orcamento.Desconto,
            DescontoEmDinheiro = orcamento.DescontoEmDinheiro,
            ValorDesconto = orcamento.ValorDesconto,
            Observacoes = orcamento.Observacoes,
            Status = orcamento.Status.ToUpperSnakeCase(),
            DataCriacao = orcamento.DataCriacao,
            ValorTotal = orcamento.ValorTotal,
            ValorTotalDesconto = orcamento.ValorTotalDesconto,
            CreatedAt = orcamento.CreatedAt.ToDateString(),
            UpdatedAt = orcamento.UpdatedAt?.ToDateString(),
            DeletedAt = orcamento.DeletedAt?.ToDateString(),
            ItensServico = MapearItensServico(orcamento.ItensServico)
        };
    }

    private static ICollection<OrcamentoItemResponse> MapearItensServico(IEnumerable<ItemServico> servicos)
    {
        return [
            new OrcamentoItemResponse
            {
                OrcamentoId = servicos.FirstOrDefault()?.OrcamentoId ?? Guid.Empty,
                Servicos = servicos
                    .Where(x => !x.EstaExcluida())
                    .GroupBy(x => new
                    {
                        x.ServicoId,
                        Nome = x.Servico?.Nome ?? string.Empty,
                        Valor = x.Servico?.Valor ?? 0m
                    })
                    .Select(servico => new ServicoItemResponse
                    {
                        ServicoId = servico.Key.ServicoId,
                        Descricao = servico.Key.Nome,
                        ValorServico = servico.Key.Valor,
                        Pecas = servico
                            .Where(p => p.PecaId.HasValue)
                            .Select(p => new PecaItemResponse
                            {
                                PecaId = p.PecaId!.Value,
                                Descricao = p.Peca?.Nome ?? string.Empty,
                                Quantidade = p.Quantidade,
                                ValorUnitario = p.Peca?.Valor ?? 0m,
                                ValorTotal = (p.Peca?.Valor ?? 0m) * p.Quantidade
                            })
                            .ToList(),
                        ValorTotal = servico.Key.Valor + servico.Sum(p => (p.Peca?.Valor ?? 0m) * p.Quantidade)
                    })
                    .ToList()
            }
        ];
    }
}
