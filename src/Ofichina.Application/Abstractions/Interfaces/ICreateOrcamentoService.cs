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
