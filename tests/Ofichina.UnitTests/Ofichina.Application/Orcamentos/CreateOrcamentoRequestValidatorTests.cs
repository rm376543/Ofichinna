using Ofichina.Application.Validators.Orcamento;
using Ofichina.Contracts.Requests.Orcamento;

namespace Ofichina.UnitTests.Application.Orcamentos;

public sealed class CreateOrcamentoRequestValidatorTests
{
    [Fact]
    public void Deve_Aceitar_Requisicao_Valida()
    {
        var validator = new CreateOrcamentoRequestValidator();
        var request = CriarRequisicaoValida();

        var result = validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Deve_Aceitar_Requisicao_Sem_Itens()
    {
        var validator = new CreateOrcamentoRequestValidator();
        var request = new CreateOrcamentoRequest
        {
            PessoaId = Guid.NewGuid(),
            VeiculoId = Guid.NewGuid(),
            AgendamentoId = Guid.NewGuid(),
            ResponsavelId = Guid.NewGuid(),
            MecanicoDiagnosticoId = Guid.NewGuid(),
            DataValidade = DateTime.UtcNow.AddDays(10),
            Desconto = 10,
            Observacoes = "Orçamento sem itens"
        };

        var result = validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Deve_Rejeitar_Requisicao_Sem_Checklist()
    {
        var validator = new CreateOrcamentoRequestValidator();
        var request = new CreateOrcamentoRequest
        {
            PessoaId = Guid.NewGuid(),
            VeiculoId = Guid.NewGuid(),
            AgendamentoId = Guid.Empty,
            ResponsavelId = Guid.NewGuid(),
            MecanicoDiagnosticoId = Guid.NewGuid(),
            DataValidade = DateTime.UtcNow.AddDays(10),
            Desconto = 10,
            Observacoes = "Orçamento inicial"
        };

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(CreateOrcamentoRequest.AgendamentoId));
    }

    private static CreateOrcamentoRequest CriarRequisicaoValida()
    {
        return new CreateOrcamentoRequest
        {
            PessoaId = Guid.NewGuid(),
            VeiculoId = Guid.NewGuid(),
            AgendamentoId = Guid.NewGuid(),
            ResponsavelId = Guid.NewGuid(),
            MecanicoDiagnosticoId = Guid.NewGuid(),
            DataValidade = DateTime.UtcNow.AddDays(10),
            Desconto = 10,
            Observacoes = "Orçamento inicial"
        };
    }
}
