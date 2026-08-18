using Ofichina.Application.Exceptions;

namespace Ofichina.UnitTests.Application.Exceptions;

public sealed class BusinessExceptionTests
{
    [Fact]
    public void Construtor_Deve_Criar_Excecao_Com_Mensagem()
    {
        const string mensagem = "Ocorreu um erro de negócio.";

        var exception = new BusinessException(mensagem);

        Assert.Equal(mensagem, exception.Message);
        Assert.Null(exception.InnerException);
    }

    [Fact]
    public void Construtor_Com_InnerException_Deve_Criar_Excecao_Com_Mensagem_E_InnerException()
    {
        const string mensagem = "Ocorreu um erro de negócio.";
        var innerException = new InvalidOperationException(
            "Erro interno.");

        var exception = new BusinessException(
            mensagem,
            innerException);

        Assert.Equal(mensagem, exception.Message);
        Assert.Same(innerException, exception.InnerException);
    }
}