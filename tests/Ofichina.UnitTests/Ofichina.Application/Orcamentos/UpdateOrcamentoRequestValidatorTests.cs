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
        var request = CriarRequisicaoValida();
        request.Id = Guid.Empty;

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(UpdateOrcamentoRequest.Id));
    }

    private static UpdateOrcamentoRequest CriarRequisicaoValida()
    {
        return new UpdateOrcamentoRequest
        {
            Id = Guid.NewGuid(),
            PessoaId = Guid.NewGuid(),
            VeiculoId = Guid.NewGuid(),
            ResponsavelId = Guid.NewGuid(),
            MecanicoDiagnosticoId = Guid.NewGuid(),
            DataValidade = DateTime.UtcNow.AddDays(5),
            Desconto = 5,
            Observacoes = "Orçamento atualizado",
            ItensServico =
            [
                new OrcamentoItemServicoRequest
                {
                    ServicoId = Guid.NewGuid(),
                    PecaId = Guid.NewGuid(),
                    Quantidade = 1
                }
            ]
        };
    }
}
