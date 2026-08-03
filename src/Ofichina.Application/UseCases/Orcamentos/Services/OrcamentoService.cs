using Ofichina.Application.Abstractions.Interfaces;
using Ofichina.Contracts;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Orcamento;

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

    public async Task<PagedResponse<OrcamentoSimplesResponse>> GetAllPagedAsync(Pagination pagination, CancellationToken cancellationToken = default)
    {
        var orcamentos = await _orcamentoRepository.GetPagedAsync(pagination, cancellationToken);

        var pessoasIds = orcamentos.Items
            .SelectMany(orcamento => new[] { orcamento.PessoaId, orcamento.ResponsavelId, orcamento.MecanicoDiagnosticoId })
            .Where(id => id != Guid.Empty)
            .Distinct()
            .ToArray();

        var pessoas = await _pessoaRepository.GetByIdsAsync(pessoasIds, cancellationToken);
        var nomesPorId = pessoas.ToDictionary(pessoa => pessoa.Id, pessoa => pessoa.Nome);

        return orcamentos.ToPagedResponse(orcamento => new OrcamentoSimplesResponse
        {
            Id = orcamento.Id,
            Cliente = ObterNome(orcamento.PessoaId, nomesPorId),
            Responsavel = ObterNome(orcamento.ResponsavelId, nomesPorId),
            MecanicoDiagnostico = ObterNome(orcamento.MecanicoDiagnosticoId, nomesPorId),
            Status = orcamento.Status.ToString(),
            DataCriacao = orcamento.DataCriacao.ToString("dd/MM/yyyy"),
            DataValidade = orcamento.DataValidade.ToString("dd/MM/yyyy"),
            Desconto = orcamento.Desconto,
            ValorTotal = orcamento.Servicos.Sum(x => x.ValorTotal).ToString(),
            CreatedAt = orcamento.CreatedAt,
            UpdatedAt = orcamento.UpdatedAt,
            DeletedAt = orcamento.DeletedAt
        });
    }

    private static string ObterNome(Guid pessoaId, IReadOnlyDictionary<Guid, string> nomesPorId)
    {
        return nomesPorId.TryGetValue(pessoaId, out var nome)
            ? nome
            : "Nome não encontrado";
    }
}
