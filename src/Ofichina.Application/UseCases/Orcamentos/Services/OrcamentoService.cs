using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Application.Abstractions.Interfaces.Service;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Extension;
using Ofichina.Contracts.Responses.Orcamento;
using Ofichina.Domain.Common;

namespace Ofichina.Application.UseCases.Orcamentos.Services;

/// <summary>
/// Serviço responsável por montar a listagem paginada de orçamentos.
/// </summary>
public sealed class OrcamentoService : IOrcamentoService
{
    private readonly IOrcamentoRepository _orcamentoRepository;
    private readonly IPessoaRepository _pessoaRepository;

    public OrcamentoService(
        IOrcamentoRepository orcamentoRepository,
        IPessoaRepository pessoaRepository)
    {
        _orcamentoRepository = orcamentoRepository;
        _pessoaRepository = pessoaRepository;
    }

    public async Task<PagedResponse<OrcamentoDetalheResponse>> GetAllPagedAsync(Pagination pagination, CancellationToken cancellationToken = default)
    {
        var orcamentos = await _orcamentoRepository.GetPagedAsync(pagination, cancellationToken);

        var pessoasIds = orcamentos.Items
            .SelectMany(orcamento => new[] { orcamento.PessoaId, orcamento.ConsultorId, orcamento.MecanicoId })
            .Where(id => id != Guid.Empty)
            .Distinct()
            .ToArray();

        var pessoas = await _pessoaRepository.GetByIdsAsync(pessoasIds, cancellationToken);
        var nomesPorId = pessoas.ToDictionary(pessoa => pessoa.Id, pessoa => pessoa.Nome);

        return orcamentos.ToPagedResponse(orcamento => new OrcamentoDetalheResponse
        {
            OrcamentoId = orcamento.Id,
            Cliente = ObterNome(orcamento.PessoaId, nomesPorId),
            Consultor = ObterNome(orcamento.ConsultorId, nomesPorId),
            Mecanico = ObterNome(orcamento.MecanicoId, nomesPorId),
            Status = orcamento.Status.ToUpperSnakeCase(),
            DataCriacao = orcamento.DataCriacao.ToString("dd/MM/yyyy"),
            DataValidade = orcamento.DataValidade.ToString("dd/MM/yyyy"),
            Desconto = orcamento.Desconto,
            ValorTotal = orcamento.ValorTotal,
            ValorTotalDesconto = orcamento.ValorTotalDesconto,
            CreatedAt = orcamento.CreatedAt.ToDateString(),
            UpdatedAt = orcamento.UpdatedAt?.ToDateString(),
            DeletedAt = orcamento.DeletedAt?.ToDateString()
        });
    }

    private static string ObterNome(Guid pessoaId, Dictionary<Guid, string> nomesPorId)
    {
        return nomesPorId.TryGetValue(pessoaId, out var nome)
            ? nome
            : "Nome não encontrado";
    }
}
