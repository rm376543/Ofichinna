using Ofichina.Application.Validators.OrdensServico;
using Ofichina.Contracts.Requests.ItensServico;

namespace Ofichina.UnitTests.Application.UseCases.OrdensServico.Validators;

public sealed class CreateItemServicoRequestValidatorTests
{
    // ============================================================  
    // SUCESSO  
    // ============================================================  

    [Fact]
    public void Deve_Aceitar_Requisicao_Valida()
    {
        var validator = new CreateItemServicoRequestValidator();
        var request = CriarRequisicao();

        var result = validator.Validate(request);

        Assert.True(result.IsValid);
    }

    // ============================================================  
    // OrdemServicoId  
    // ============================================================  

    [Fact]
    public void Deve_Rejeitar_OrdemServicoId_Vazio()
    {
        var validator = new CreateItemServicoRequestValidator();
        var request = CriarRequisicao(ordemServicoId: Guid.Empty);

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(CreateItemServicoRequest.OrdemServicoId));
    }

    // ============================================================  
    // ServicoId  
    // ============================================================  

    [Fact]
    public void Deve_Rejeitar_ServicoId_Vazio()
    {
        var validator = new CreateItemServicoRequestValidator();
        var request = CriarRequisicao(servicoId: Guid.Empty);

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(CreateItemServicoRequest.ServicoId));
    }

    // ============================================================  
    // PecaId  
    // ============================================================  

    [Fact]
    public void Deve_Rejeitar_PecaId_Vazio()
    {
        var validator = new CreateItemServicoRequestValidator();
        var request = CriarRequisicao(pecaId: Guid.Empty);

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(CreateItemServicoRequest.PecaId));
    }

    // ============================================================  
    // Quantidade  
    // ============================================================  

    [Fact]
    public void Deve_Rejeitar_Quantidade_Zero()
    {
        // Cobre o limite estrito do GreaterThan(0) (0 é inválido).  
        var validator = new CreateItemServicoRequestValidator();
        var request = CriarRequisicao(quantidade: 0);

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(CreateItemServicoRequest.Quantidade));
    }

    [Fact]
    public void Deve_Rejeitar_Quantidade_Negativa()
    {
        var validator = new CreateItemServicoRequestValidator();
        var request = CriarRequisicao(quantidade: -1);

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(CreateItemServicoRequest.Quantidade));
    }

    // ============================================================  
    // HELPERS  
    // ============================================================  

    private static CreateItemServicoRequest CriarRequisicao(
        Guid? ordemServicoId = null,
        Guid? servicoId = null,
        Guid? pecaId = null,
        int quantidade = 1)
    {
        return new CreateItemServicoRequest
        {
            OrdemServicoId = ordemServicoId ?? Guid.NewGuid(),
            ServicoId = servicoId ?? Guid.NewGuid(),
            PecaId = pecaId ?? Guid.NewGuid(),
            Quantidade = quantidade
        };
    }
}