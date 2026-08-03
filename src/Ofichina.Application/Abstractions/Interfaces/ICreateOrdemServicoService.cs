using Ofichina.Contracts.Common;
using Ofichina.Application.UseCases.OrdensServico.Commands;

namespace Ofichina.Application.Abstractions.Interfaces;

/// <summary>
/// Serviço responsável pela criação de ordens de serviço.
/// </summary>
public interface ICreateOrdemServicoService
{
    /// <summary>
    /// Cria uma ordem de serviço e retorna o identificador gerado.
    /// </summary>
    Task<Result> CreateAsync(CreateOrdemServicoCommand command, CancellationToken cancellationToken = default);
}