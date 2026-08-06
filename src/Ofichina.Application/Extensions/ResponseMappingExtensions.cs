using Ofichina.Contracts.Responses.Agendamento;
using Ofichina.Contracts.Responses.Orcamento;
using Ofichina.Contracts.Responses.OrdemServico;
using Ofichina.Contracts.Responses.OrdensServico;
using Ofichina.Contracts.Responses.Pessoa;
using Ofichina.Contracts.Responses.Servicos;
using Ofichina.Contracts.Responses.Veiculo;
using Ofichina.Domain.Common;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;

namespace Ofichina.Application.Extensions;

/// <summary>
/// Extensões para mapeamento de entidades do domínio para objetos de resposta (DTOs).
/// </summary>
public static class ResponseMappingExtensions
{
    /// <summary>
    /// Converte uma instância de Pessoa para um objeto de resposta PessoaResponse.
    /// </summary>
    /// <param name="pessoa"></param>
    /// <returns>Um objeto PessoaResponse contendo as informações da pessoa.</returns>
    public static PessoaResponse ToResponse(this Pessoa pessoa)
    {
        ArgumentNullException.ThrowIfNull(pessoa);

        return new PessoaResponse
        {
            Id = pessoa.Id,
            Nome = pessoa.Nome,
            Documento = pessoa.Documento.ToString(),
            Telefone = pessoa.Telefone.ToString(),
            Logradouro = pessoa.Endereco.Logradouro,
            Numero = pessoa.Endereco.Numero,
            Complemento = pessoa.Endereco.Complemento,
            Bairro = pessoa.Endereco.Bairro,
            Cidade = pessoa.Endereco.Cidade,
            Estado = pessoa.Endereco.Estado,
            Cep = pessoa.Endereco.Cep.ToString(),
            UsuarioId = pessoa.UsuarioId,
            CreatedAt = pessoa.CreatedAt,
            UpdatedAt = pessoa.UpdatedAt,
            DeletedAt = pessoa.DeletedAt
        };
    }

    /// <summary>
    /// Converte uma instância de Agendamento para um objeto de resposta AgendamentoResponse.
    /// </summary>
    /// <param name="agendamento"></param>
    /// <returns>Um objeto AgendamentoResponse contendo as informações do agendamento.</returns>
    public static AgendamentoResponse ToResponse(this Agendamento agendamento)
    {
        ArgumentNullException.ThrowIfNull(agendamento);

        return new AgendamentoResponse
        {
            Id = agendamento.Id,
            PessoaId = agendamento.ClientePessoaId,
            ClienteNome = agendamento.Cliente.Nome,
            DiaDisponibilidadeId = agendamento.DiaDisponibilidadeId,
            HorarioConsultorId = agendamento.HorarioConsultorId,
            ConsultorPessoaId = agendamento.ConsultorPessoaId,
            ConsultorNome = agendamento.HorarioConsultor?.Pessoa?.Nome ?? string.Empty,
            VeiculoId = agendamento.VeiculoId,
            VeiculoPlaca = agendamento.Veiculo.Placa.Numero,
            VeiculoDescricao = $"{agendamento.Veiculo.Marca} {agendamento.Veiculo.Modelo} {agendamento.Veiculo.AnoFabricacao}",
            Status = agendamento.Status.ToUpperSnakeCase(),
            Descricao = agendamento.Descricao,
            CreatedAt = agendamento.CreatedAt,
            UpdatedAt = agendamento.UpdatedAt,
            DeletedAt = agendamento.DeletedAt
        };
    }

    /// <summary>
    /// Converte uma instância de Agendamento para um objeto de resposta AgendamentoResponse, utilizando informações adicionais de Pessoa, Consultor e Veículo.
    /// </summary>
    /// <param name="agendamento"></param>
    /// <param name="pessoa"></param>
    /// <param name="consultor"></param>
    /// <param name="veiculo"></param>
    /// <returns>Um objeto AgendamentoResponse contendo as informações do agendamento e detalhes adicionais de Pessoa, Consultor e Veículo.</returns>
    public static AgendamentoResponse ToResponse(this Agendamento agendamento, Pessoa pessoa, Pessoa consultor, Veiculo veiculo)
    {
        ArgumentNullException.ThrowIfNull(agendamento);
        ArgumentNullException.ThrowIfNull(pessoa);
        ArgumentNullException.ThrowIfNull(consultor);
        ArgumentNullException.ThrowIfNull(veiculo);

        return new AgendamentoResponse
        {
            Id = agendamento.Id,
            PessoaId = agendamento.ClientePessoaId,
            ClienteNome = pessoa.Nome,
            DiaDisponibilidadeId = agendamento.DiaDisponibilidadeId,
            HorarioConsultorId = agendamento.HorarioConsultorId,
            ConsultorPessoaId = agendamento.ConsultorPessoaId,
            ConsultorNome = consultor.Nome,
            VeiculoId = agendamento.VeiculoId,
            VeiculoPlaca = veiculo.Placa.Numero,
            VeiculoDescricao = $"{veiculo.Marca} {veiculo.Modelo} {veiculo.AnoFabricacao}",
            Status = agendamento.Status.ToUpperSnakeCase(),
            Descricao = agendamento.Descricao,
            CreatedAt = agendamento.CreatedAt,
            UpdatedAt = agendamento.UpdatedAt,
            DeletedAt = agendamento.DeletedAt
        };
    }

    /// <summary>
    /// Converte uma instância de Veiculo para um objeto de resposta VeiculoResponse.
    /// </summary>
    /// <param name="veiculo"></param>
    /// <returns>Um objeto VeiculoResponse contendo as informações do veículo.</returns>
    public static VeiculoResponse ToResponse(this Veiculo veiculo)
    {
        ArgumentNullException.ThrowIfNull(veiculo);

        return new VeiculoResponse
        {
            Id = veiculo.Id,
            Placa = veiculo.Placa.ToString(),
            Marca = veiculo.Marca,
            Modelo = veiculo.Modelo,
            AnoFabricacao = veiculo.AnoFabricacao,
            Cor = veiculo.Cor,
            Hodometro = veiculo.Hodometro.Valor,
            HodometroFormatado = veiculo.Hodometro.ToString(),
            CreatedAt = veiculo.CreatedAt,
            UpdatedAt = veiculo.UpdatedAt,
            DeletedAt = veiculo.DeletedAt
        };
    }

    /// <summary>
    /// Converte uma instância de Servico para um objeto de resposta ServicoResponse.
    /// </summary>
    /// <param name="servico"></param>
    /// <returns>Um objeto ServicoResponse contendo as informações do serviço.</returns>
    public static ServicoResponse ToResponse(this Servico servico)
    {
        ArgumentNullException.ThrowIfNull(servico);

        return new ServicoResponse
        {
            Id = servico.Id,
            Nome = servico.Nome,
            Descricao = servico.Descricao,
            Valor = servico.Valor,
            Ativo = !servico.EstaExcluida(),
            CreatedAt = servico.CreatedAt,
            UpdatedAt = servico.UpdatedAt,
            DeletedAt = servico.DeletedAt
        };
    }

    /// <summary>
    /// Converte uma instância de OrdemServico para um objeto de resposta OrdemServicoResponse.
    /// </summary>
    /// <param name="ordemServico"></param>
    /// <returns>Um objeto OrdemServicoResponse contendo as informações da ordem de serviço.</returns>
    public static OrdemServicoResponse ToResponse(this OrdemServico ordemServico)
    {
        ArgumentNullException.ThrowIfNull(ordemServico);

        return new OrdemServicoResponse
        {
            Id = ordemServico.Id,
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

    /// <summary>
    /// Converte uma instância de Orcamento para um objeto de resposta OrcamentoResponse.
    /// </summary>
    /// <param name="orcamento"></param>
    /// <returns>Um objeto OrcamentoResponse contendo as informações do orçamento.</returns>
    public static OrcamentoResponse ToResponse(this Orcamento orcamento)
    {
        ArgumentNullException.ThrowIfNull(orcamento);

        return new OrcamentoResponse
        {
            Id = orcamento.Id,
            PessoaId = orcamento.PessoaId,
            VeiculoId = orcamento.VeiculoId,
            ChecklistId = orcamento.ChecklistId,
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
            Checklist = orcamento.Checklist is null ? null : ToResponse(orcamento.Checklist),
            ItensServico = MapearItensServico(orcamento.ItensServico)
        };
    }

    /// <summary>
    /// Converte uma instância de Checklist para um objeto de resposta ChecklistResponse.
    /// </summary>
    /// <param name="checklist"></param>
    /// <returns>Um objeto ChecklistResponse contendo as informações do checklist.</returns>
    private static ChecklistResponse ToResponse(this Checklist checklist)
    {
        return new ChecklistResponse
        {
            Id = checklist.Id,
            VeiculoId = checklist.VeiculoId,
            PessoaId = checklist.PessoaId,
            HodometroEntrada = checklist.HodometroEntrada,
            ItensVerificados = checklist.ItensVerificados,
            Observacoes = checklist.Observacoes,
            Finalizado = checklist.Finalizado,
            CreatedAt = checklist.CreatedAt,
            UpdatedAt = checklist.UpdatedAt,
            DeletedAt = checklist.DeletedAt
        };
    }

    /// <summary>
    /// Converte os serviços de uma OrdemServico em uma coleção de objetos de resposta OrdemServicoItensResponse.
    /// </summary>
    /// <param name="ordemServico"></param>
    /// <returns>Uma coleção de objetos OrdemServicoItensResponse contendo as informações dos serviços da ordem de serviço.</returns>

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

    /// <summary>
    /// Converte os itens de um Orcamento em uma coleção de objetos de resposta OrcamentoItemResponse.
    /// </summary>
    /// <param name="servicos"></param>
    /// <returns>Uma coleção de objetos OrcamentoItemResponse contendo as informações dos serviços do orçamento.</returns>
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