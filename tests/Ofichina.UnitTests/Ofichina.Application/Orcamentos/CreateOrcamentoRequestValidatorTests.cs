using Ofichina.Application.Validators.Orcamento;
using Ofichina.Contracts.Requests.Orcamento;
using Ofichina.Contracts.Requests.Orcamentos;

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
    public void Deve_Rejeitar_Requisicao_Sem_Itens()
    {
        var validator = new CreateOrcamentoRequestValidator();
        var request = new CreateOrcamentoRequest
        {
            PessoaId = Guid.NewGuid(),
            VeiculoId = Guid.NewGuid(),
            ResponsavelId = Guid.NewGuid(),
            MecanicoDiagnosticoId = Guid.NewGuid(),
            DataValidade = DateTime.UtcNow.AddDays(10),
            Desconto = 10,
            Observacoes = "Orçamento sem itens",
            Servicos = []
        };

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.ErrorMessage.Contains("ao menos um serviço", StringComparison.OrdinalIgnoreCase));
    }

    private static CreateOrcamentoRequest CriarRequisicaoValida()
    {
        return new CreateOrcamentoRequest
        {
            PessoaId = Guid.NewGuid(),
            VeiculoId = Guid.NewGuid(),
            ResponsavelId = Guid.NewGuid(),
            MecanicoDiagnosticoId = Guid.NewGuid(),
            DataValidade = DateTime.UtcNow.AddDays(10),
            Desconto = 10,
            Observacoes = "Orçamento inicial",
            Servicos =
            [
                new CreateOrcamentoServicoRequest
                {
                    ServicoId = Guid.NewGuid(),
                    Pecas =
                    [
                        new CreateOrcamentoServicoPecaRequest
                        {
                            PecaId = Guid.NewGuid(),
                            Quantidade = 1
                        }
                    ]
                }
            ]
        };
    }
}
