using Ofichina.Application.UseCases.Orcamentos.Commands;
using Ofichina.Contracts.Common;

namespace Ofichina.Application.Abstractions.Interfaces;

/// <summary>
/// Serviço responsável pela criação de orçamentos.
/// </summary>
public interface ICreateOrcamentoService
{
    Task<Result> CreateAsync(CreateOrcamentoCommand command, CancellationToken cancellationToken = default);
}

/// <summary>
/// Contrato para seleção automática de mecânico disponível.
/// </summary>
public interface IMecanicoDisponibilidadeService
{
    Task<Guid?> ObterMecanicoDisponivelAsync(CancellationToken cancellationToken = default);
}
