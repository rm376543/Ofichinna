using Ofichina.Domain.Common;
using Ofichina.Application.Abstractions.Interfaces;
using Ofichina.Contracts;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.OrdemServico;

namespace Ofichina.Application.UseCases.OrdensServico.Services;

/// <summary>
/// Serviço responsável por montar a listagem paginada de ordens de serviço.
/// </summary>
public sealed class OrdemServicoService : IOrdemServicoService
{
    private readonly IOrdemServicoRepository _ordemServicoRepository;
    private readonly IPessoaRepository _pessoaRepository;

    public OrdemServicoService(
        IOrdemServicoRepository ordemServicoRepository,
        IPessoaRepository pessoaRepository)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _pessoaRepository = pessoaRepository;
    }

    public async Task<PagedResponse<OrdemServicoSimplesResponse>> GetAllPagedAsync(
        Pagination pagination,
        CancellationToken cancellationToken = default)
    {
        var ordensServico = await _ordemServicoRepository.GetPagedAsync(
            pagination,
            cancellationToken);

        var pessoasIds = ordensServico.Items
            .SelectMany(ordem => new[] { ordem.PessoaId, ordem.FuncionarioId })
            .Where(id => id != Guid.Empty)
            .Distinct()
            .ToArray();

        var pessoas = await _pessoaRepository.GetByIdsAsync(pessoasIds, cancellationToken);
        var nomesPorId = pessoas.ToDictionary(pessoa => pessoa.Id, pessoa => pessoa.Nome);

        return ordensServico.ToPagedResponse(ordem => new OrdemServicoSimplesResponse
        {
            OrdemServicoSimplesId = ordem.Id,
            Cliente = ObterNome(ordem.PessoaId, nomesPorId),
            Funcionario = ObterNome(ordem.FuncionarioId, nomesPorId),
            ProblemaRelatado = ordem.ProblemaRelatado,
            Status = ordem.Status.ToUpperSnakeCase(),
            DataAbetura = ordem.DataAbertura.ToString("dd/MM/yyyy"),
            DataFinalizacao = ordem.DataFinalizacao?.ToString("dd/MM/yyyy") ?? "",
            Observacao = ordem.Observacao,
            ValorTotal = ordem.ValorTotal.ToString("C"),
            CreatedAt = ordem.CreatedAt,
            UpdatedAt = ordem.UpdatedAt,
            DeletedAt = ordem.DeletedAt
        });
    }

    private static string ObterNome(Guid pessoaId, IReadOnlyDictionary<Guid, string> nomesPorId)
    {
        return nomesPorId.TryGetValue(pessoaId, out var nome)
            ? nome
            : "Nome não encontrado";
    }
}