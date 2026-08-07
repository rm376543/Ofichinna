using Ofichina.Contracts.Responses.OrdemServico;
using Ofichina.Contracts.Responses.OrdensServico;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Common;
using Ofichina.Domain.Entities;

namespace Ofichina.Application.UseCases.OrdensServico.Mappings;

public static class OrdemServicoResponseMappingExtensions
{
    public static OrdemServicoResponse ToResponse(this OrdemServico ordemServico)
    {
        ArgumentNullException.ThrowIfNull(ordemServico);

        return new OrdemServicoResponse
        {
            OrdemServicoId = ordemServico.Id,
            PessoaId = ordemServico.PessoaId,
            VeiculoId = ordemServico.VeiculoId,
            FuncionarioId = ordemServico.FuncionarioId,
            HodometroEntrada = ordemServico.HodometroEntrada,
            ProblemaRelatado = ordemServico.ProblemaRelatado,
            Status = ordemServico.Status.ToUpperSnakeCase(),
            DataAbertura = ordemServico.DataAbertura,
            DataFinalizacao = ordemServico.DataFinalizacao,
            Observacao = ordemServico.Observacao,
            ValorTotal = ordemServico.ValorTotal,
            CreatedAt = ordemServico.CreatedAt,
            UpdatedAt = ordemServico.UpdatedAt,
            DeletedAt = ordemServico.DeletedAt,
            Servicos = MapearServicos(ordemServico)
        };
    }

    private static ICollection<OrdemServicoItensResponse> MapearServicos(OrdemServico ordemServico)
    {
        return ordemServico.Servicos
            .Where(x => !x.EstaExcluida())
            .GroupBy(x => new
            {
                x.ServicoId,
                Nome = x.Servico?.Nome ?? string.Empty,
                Valor = x.Servico?.Valor ?? 0
            })
            .Select(g => new OrdemServicoItensResponse
            {
                OrdemServicoId = ordemServico.Id,
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
                                ValorUnitario = p.Peca?.Valor ?? 0,
                                ValorTotal = (p.Peca?.Valor ?? 0) * p.Quantidade
                            })
                            .ToList(),
                        ValorTotal = g.Key.Valor + g.Sum(p => (p.Peca?.Valor ?? 0) * p.Quantidade)
                    }
                ]
            })
            .ToList();
    }
}
