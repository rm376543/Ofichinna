using Ofichina.Application.Validators.OrdensServico;
using Ofichina.Contracts.Requests.OrdensServico;

namespace Ofichina.UnitTests.Application.UseCases.OrdensServico.Validators;

public sealed class UpdateOrdemServicoRequestValidatorTests
{
    // ============================================================  
    // SUCESSO  
    // ============================================================  

    [Fact]
    public void Deve_Aceitar_Requisicao_Valida_Com_Observacoes()
    {
        var validator = new UpdateOrdemServicoRequestValidator();
        var request = CriarRequisicao();

        var result = validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Deve_Aceitar_Requisicao_Valida_Sem_Observacoes()
    {
        // Observacoes nulo -> a regra de MaximumLength é ignorada (When == false).  
        var validator = new UpdateOrdemServicoRequestValidator();
        var request = CriarRequisicao(observacoes: null);

        var result = validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Deve_Aceitar_Hodometro_Zero()
    {
        // Cobre o limite GreaterThanOrEqualTo(0).  
        var validator = new UpdateOrdemServicoRequestValidator();
        var request = CriarRequisicao(hodometro: 0);

        var result = validator.Validate(request);

        Assert.True(result.IsValid);
    }

    // ============================================================  
    // OrdemServicoId  
    // ============================================================  

    [Fact]
    public void Deve_Rejeitar_OrdemServicoId_Vazio()
    {
        var validator = new UpdateOrdemServicoRequestValidator();
        var request = CriarRequisicao(ordemServicoId: Guid.Empty);

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(UpdateOrdemServicoRequest.OrdemServicoId));
    }

    // ============================================================  
    // PessoaId  
    // ============================================================  

    [Fact]
    public void Deve_Rejeitar_PessoaId_Vazio()
    {
        var validator = new UpdateOrdemServicoRequestValidator();
        var request = CriarRequisicao(pessoaId: Guid.Empty);

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(UpdateOrdemServicoRequest.PessoaId));
    }

    // ============================================================  
    // VeiculoId  
    // ============================================================  

    [Fact]
    public void Deve_Rejeitar_VeiculoId_Vazio()
    {
        var validator = new UpdateOrdemServicoRequestValidator();
        var request = CriarRequisicao(veiculoId: Guid.Empty);

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(UpdateOrdemServicoRequest.VeiculoId));
    }

    // ============================================================  
    // ConsultorId  
    // ============================================================  

    [Fact]
    public void Deve_Rejeitar_ConsultorId_Vazio()
    {
        var validator = new UpdateOrdemServicoRequestValidator();
        var request = CriarRequisicao(consultorId: Guid.Empty);

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(UpdateOrdemServicoRequest.ConsultorId));
    }

    // ============================================================  
    // Hodometro  
    // ============================================================  

    [Fact]
    public void Deve_Rejeitar_Hodometro_Negativo()
    {
        var validator = new UpdateOrdemServicoRequestValidator();
        var request = CriarRequisicao(hodometro: -1);

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(UpdateOrdemServicoRequest.Hodometro));
    }

    // ============================================================  
    // ProblemaRelatado  
    // ============================================================  

    [Fact]
    public void Deve_Rejeitar_ProblemaRelatado_Vazio()
    {
        var validator = new UpdateOrdemServicoRequestValidator();
        var request = CriarRequisicao(problemaRelatado: string.Empty);

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(UpdateOrdemServicoRequest.ProblemaRelatado));
    }

    [Fact]
    public void Deve_Rejeitar_ProblemaRelatado_Muito_Longo()
    {
        var validator = new UpdateOrdemServicoRequestValidator();
        var request = CriarRequisicao(problemaRelatado: new string('x', 501));

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(UpdateOrdemServicoRequest.ProblemaRelatado));
    }

    // ============================================================  
    // Observacoes  
    // ============================================================  

    [Fact]
    public void Deve_Rejeitar_Observacoes_Muito_Longa()
    {
        // Observacoes preenchida (When == true) e excedendo 1000 caracteres.  
        var validator = new UpdateOrdemServicoRequestValidator();
        var request = CriarRequisicao(observacoes: new string('x', 1001));

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(UpdateOrdemServicoRequest.Observacoes));
    }

    // ============================================================  
    // HELPERS  
    // ============================================================  

    private static UpdateOrdemServicoRequest CriarRequisicao(
        Guid? ordemServicoId = null,
        Guid? pessoaId = null,
        Guid? veiculoId = null,
        Guid? consultorId = null,
        int hodometro = 77_290,
        string problemaRelatado = "Barulhos durante a aceleração",
        string? observacoes = "Carro de desenvolvedor")
    {
        return new UpdateOrdemServicoRequest
        {
            OrdemServicoId = ordemServicoId ?? Guid.NewGuid(),
            PessoaId = pessoaId ?? Guid.NewGuid(),
            VeiculoId = veiculoId ?? Guid.NewGuid(),
            ConsultorId = consultorId ?? Guid.NewGuid(),
            Hodometro = hodometro,
            ProblemaRelatado = problemaRelatado,
            Observacoes = observacoes
        };
    }
}