using Ofichina.Contracts.Common;

namespace Ofichina.UnitTests.Contracts;

public class ApiResponseTests
{
    [Fact]
    public void SuccessResponse_DeveCriarRespostaComSucesso()
    {
        var response = ApiResponse.SuccessResponse("Operação realizada com sucesso.");

        Assert.True(response.Success);
        Assert.Equal("Operação realizada com sucesso.", response.Message);
        Assert.Empty(response.Errors);
    }

    [Fact]
    public void FailureResponse_DeveCriarRespostaComErro()
    {
        var response = ApiResponse.FailureResponse("Algo deu errado.");

        Assert.False(response.Success);
        Assert.Equal("Algo deu errado.", response.Message);
        Assert.Empty(response.Errors);
    }

    [Fact]
    public void SuccessResponse_Generic_DeveCriarRespostaComDados()
    {
        var response = ApiResponse<string>.SuccessResponse("dados", "Tudo certo");

        Assert.True(response.Success);
        Assert.Equal("Tudo certo", response.Message);
        Assert.Equal("dados", response.Data);
        Assert.Empty(response.Errors);
    }

    [Fact]
    public void FailureResponse_Generic_DeveCriarRespostaComErro()
    {
        var response = ApiResponse<int>.FailureResponse("Falha na operação");

        Assert.False(response.Success);
        Assert.Equal("Falha na operação", response.Message);
        Assert.Equal(0, response.Data);
        Assert.Empty(response.Errors);
    }
}