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
            MecanicoDiagnosticoId = orcamento.MecanicoDiagnosticoId,
            ResponsavelId = orcamento.ResponsavelId,
            DataValidade = orcamento.DataValidade,
            Desconto = orcamento.Desconto,
            Observacoes = orcamento.Observacoes,
            Status = orcamento.Status.ToUpperSnakeCase(),
            DataCriacao = orcamento.DataCriacao,
            ValorTotal = orcamento.ValorTotal,
            CreatedAt = orcamento.CreatedAt,
            UpdatedAt = orcamento.UpdatedAt,
            DeletedAt = orcamento.DeletedAt,
            ItensServico = MapearItensServico(orcamento.ItensServico)
        };
    }

    private static ICollection<OrcamentoItemResponse> MapearItensServico(IEnumerable<ItemServico> servicos)
    {
        return servicos
            .Where(x => !x.EstaExcluida())
            .GroupBy(x => new { x.ServicoId, Nome = x.Servico?.Nome ?? string.Empty, Valor = x.Servico?.Valor ?? 0m })
            .Select(g => new OrcamentoItemResponse
            {
                OrcamentoId = g.First().OrcamentoId ?? Guid.Empty,
                Servicos =
                [
                    new ServicoItemResponse
                    {
                        ServicoId = g.Key.ServicoId,
                        Descricao = g.Key.Nome,
                        ValorServico = g.Key.Valor,
                        Pecas = g
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
                        ValorTotal = g.Key.Valor + g.Sum(p => (p.Peca?.Valor ?? 0m) * p.Quantidade)
                    }
                ]
            })
            .ToList();
    }
}
