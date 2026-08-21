using Ofichina.Application.Validators.Pecas;
using Ofichina.Contracts.Requests.Pecas;

namespace Ofichina.UnitTests.Application.UseCases.Pecas.Validators;

public sealed class UpdatePecaRequestValidatorTests
{
    // ============================================================  
    // SUCESSO  
    // ============================================================  

    [Fact]
    public void Deve_Aceitar_Requisicao_Valida_Com_Descricao()
    {
        var validator = new UpdatePecaRequestValidator();
        var request = CriarRequisicao();

        var result = validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Deve_Aceitar_Requisicao_Valida_Sem_Descricao()
    {
        // Descricao nula -> a regra de MaximumLength é ignorada (When == false).  
        var validator = new UpdatePecaRequestValidator();
        var request = CriarRequisicao(descricao: null);

        var result = validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Deve_Aceitar_QuantidadeEstoque_Zero()
    {
        // Cobre o limite GreaterThanOrEqualTo(0).  
        var validator = new UpdatePecaRequestValidator();
        var request = CriarRequisicao(quantidadeEstoque: 0);

        var result = validator.Validate(request);

        Assert.True(result.IsValid);
    }

    // ============================================================  
    // PecaId  
    // ============================================================  

    [Fact]
    public void Deve_Rejeitar_PecaId_Vazio()
    {
        var validator = new UpdatePecaRequestValidator();
        var request = CriarRequisicao(pecaId: Guid.Empty);

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(UpdatePecaRequest.PecaId));
    }

    // ============================================================  
    // Nome  
    // ============================================================  

    [Fact]
    public void Deve_Rejeitar_Nome_Vazio()
    {
        var validator = new UpdatePecaRequestValidator();
        var request = CriarRequisicao(nome: string.Empty);

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(UpdatePecaRequest.Nome));
    }

    [Fact]
    public void Deve_Rejeitar_Nome_Muito_Longo()
    {
        var validator = new UpdatePecaRequestValidator();
        var request = CriarRequisicao(nome: new string('x', 151));

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(UpdatePecaRequest.Nome));
    }

    // ============================================================  
    // Descricao  
    // ============================================================  

    [Fact]
    public void Deve_Rejeitar_Descricao_Muito_Longa()
    {
        // Descricao preenchida (When == true) e excedendo 500 caracteres.  
        var validator = new UpdatePecaRequestValidator();
        var request = CriarRequisicao(descricao: new string('x', 501));

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(UpdatePecaRequest.Descricao));
    }

    // ============================================================  
    // Codigo  
    // ============================================================  

    [Fact]
    public void Deve_Rejeitar_Codigo_Vazio()
    {
        var validator = new UpdatePecaRequestValidator();
        var request = CriarRequisicao(codigo: string.Empty);

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(UpdatePecaRequest.Codigo));
    }

    [Fact]
    public void Deve_Rejeitar_Codigo_Muito_Longo()
    {
        var validator = new UpdatePecaRequestValidator();
        var request = CriarRequisicao(codigo: new string('x', 51));

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(UpdatePecaRequest.Codigo));
    }

    // ============================================================  
    // Valor  
    // ============================================================  

    [Fact]
    public void Deve_Rejeitar_Valor_Zero()
    {
        var validator = new UpdatePecaRequestValidator();
        var request = CriarRequisicao(valor: 0m);

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(UpdatePecaRequest.Valor));
    }

    [Fact]
    public void Deve_Rejeitar_Valor_Negativo()
    {
        var validator = new UpdatePecaRequestValidator();
        var request = CriarRequisicao(valor: -1m);

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(UpdatePecaRequest.Valor));
    }

    // ============================================================  
    // QuantidadeEstoque  
    // ============================================================  

    [Fact]
    public void Deve_Rejeitar_QuantidadeEstoque_Negativa()
    {
        var validator = new UpdatePecaRequestValidator();
        var request = CriarRequisicao(quantidadeEstoque: -1);

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(UpdatePecaRequest.QuantidadeEstoque));
    }

    // ============================================================  
    // HELPERS  
    // ============================================================  

    private static UpdatePecaRequest CriarRequisicao(
        Guid? pecaId = null,
        string nome = "Pastilha de freio",
        string? descricao = "Pastilha dianteira",
        string codigo = "PF-1234",
        decimal valor = 149.90m,
        int quantidadeEstoque = 10)
    {
        return new UpdatePecaRequest
        {
            PecaId = pecaId ?? Guid.NewGuid(),
            Nome = nome,
            Descricao = descricao,
            Codigo = codigo,
            Valor = valor,
            QuantidadeEstoque = quantidadeEstoque
        };
    }
}