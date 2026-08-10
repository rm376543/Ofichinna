using Ofichina.Application.Validators.OrdensServico;
using Ofichina.Contracts.Requests.OrdensServico;

namespace Ofichina.UnitTests.Application.OrdensServico;

public sealed class CreateOrdemServicoRequestValidatorTests
{
    [Fact]
    public void Deve_Aceitar_Requisicao_Valida_Sem_Servicos()
    {
        var validator = new CreateOrdemServicoRequestValidator();
        var request = new CreateOrdemServicoRequest
        {
            PessoaId = Guid.NewGuid(),
            VeiculoId = Guid.NewGuid(),
            ConsultorId = Guid.NewGuid(),
            Hodometro = 77290,
            ProblemaRelatado = "Barulhos durante a aceleração",
            Observacoes = "carro de dev"
        };

        var result = validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Deve_Rejeitar_Requisicao_Com_Campos_Obrigatorios_Em_Branco()
    {
        var validator = new CreateOrdemServicoRequestValidator();
        var request = new CreateOrdemServicoRequest();

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(CreateOrdemServicoRequest.PessoaId));
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(CreateOrdemServicoRequest.VeiculoId));
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(CreateOrdemServicoRequest.ConsultorId));
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(CreateOrdemServicoRequest.ProblemaRelatado));
    }
}