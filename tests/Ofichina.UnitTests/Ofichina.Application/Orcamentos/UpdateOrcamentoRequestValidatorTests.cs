using Ofichina.Application.Validators.Orcamento;
using Ofichina.Contracts.Requests.Orcamento;

namespace Ofichina.UnitTests.Application.Orcamentos;

public sealed class UpdateOrcamentoRequestValidatorTests
{
    [Fact]
    public void Deve_Aceitar_Requisicao_Valida()
    {
        var validator = new UpdateOrcamentoRequestValidator();
        var request = CriarRequisicaoValida();

        var result = validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Deve_Rejeitar_Requisicao_Com_Identificador_Vazio()
    {
        var validator = new UpdateOrcamentoRequestValidator();
        var request = new UpdateOrcamentoRequest
        {
            OrcamentoId = Guid.Empty,
            PessoaId = Guid.NewGuid(),
            VeiculoId = Guid.NewGuid(),
            ConsultorId = Guid.NewGuid(),
            MecanicoId = Guid.NewGuid(),
            DataValidade = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
            Observacoes = "Orçamento atualizado"
        };

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(UpdateOrcamentoRequest.OrcamentoId));
    }

    private static UpdateOrcamentoRequest CriarRequisicaoValida()
    {
        return new UpdateOrcamentoRequest
        {
            OrcamentoId = Guid.NewGuid(),
            PessoaId = Guid.NewGuid(),
            VeiculoId = Guid.NewGuid(),
            ConsultorId = Guid.NewGuid(),
            MecanicoId = Guid.NewGuid(),
            DataValidade = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
            Observacoes = "Orçamento atualizado"
        };
    }
}
