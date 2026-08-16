using System.Reflection;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Extension;
using Ofichina.Contracts.Requests.Orcamento;
using Ofichina.Contracts.Responses.Authentication;
using Ofichina.Contracts.Responses.Orcamento;
using Ofichina.Contracts.Responses.OrdemServico;
using Ofichina.Contracts.Responses.OrdensServico;
using Ofichina.Contracts.Responses.Servicos;
using Ofichina.Contracts.Responses.Veiculo;
using Ofichina.Contracts.Specifications;

namespace Ofichina.UnitTests.Contracts;

public sealed class ContractsCoverageTests
{
    [Fact]
    public void TiposPublicosComConstrutorPadrao_Devem_Permitir_Atribuicao_De_Propriedades()
    {
        var assembly = typeof(ApiResponse).Assembly;

        foreach (var type in assembly
                     .GetExportedTypes()
                     .Where(type => type.IsClass && !type.IsAbstract && !type.ContainsGenericParameters))
        {
            var constructor = type.GetConstructor(Type.EmptyTypes);

            if (constructor is null)
            {
                continue;
            }

            var instance = Activator.CreateInstance(type);

            Assert.NotNull(instance);

            foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (!property.CanWrite || property.GetIndexParameters().Length > 0)
                {
                    continue;
                }

                var value = CreateValue(property.PropertyType);

                if (value is null && property.PropertyType.IsValueType && Nullable.GetUnderlyingType(property.PropertyType) is null)
                {
                    continue;
                }

                property.SetValue(instance, value);

                Assert.Equal(value, property.GetValue(instance));
            }
        }
    }

    [Fact]
    public void ApiResponse_Deve_Permitir_ListaDeErros_Em_Sucesso_E_Falha()
    {
        var errors = new[] { "Erro 1", "Erro 2" };

        var failure = ApiResponse.FailureResponse(errors);

        Assert.False(failure.Success);
        Assert.Null(failure.Message);
        Assert.Equal(errors, failure.Errors);

        var genericFailure = ApiResponse<string>.FailureResponse(errors);

        Assert.False(genericFailure.Success);
        Assert.Null(genericFailure.Message);
        Assert.Equal(errors, genericFailure.Errors);
        Assert.Null(genericFailure.Data);
    }

    [Fact]
    public void ApiResponseGenerico_Deve_Permitir_Instanciacao_Padrao()
    {
        var response = new ApiResponse<string>();

        Assert.False(response.Success);
        Assert.Null(response.Message);
        Assert.Null(response.Data);
        Assert.Empty(response.Errors);
    }

    [Fact]
    public void Result_Deve_Expor_Estados_De_Sucesso_E_Falha()
    {
        var success = Result.Success();

        Assert.True(success.IsSuccess);
        Assert.Null(success.Error);
        Assert.Empty(success.Errors);

        var failure = Result.Failure("Falha geral");

        Assert.False(failure.IsSuccess);
        Assert.Equal("Falha geral", failure.Error);
        Assert.Empty(failure.Errors);

        var errors = new[] { "E1", "E2" };
        var failureWithErrors = Result.Failure(errors);

        Assert.False(failureWithErrors.IsSuccess);
        Assert.Null(failureWithErrors.Error);
        Assert.Equal(errors, failureWithErrors.Errors);
    }

    [Fact]
    public void ResultGenerico_Deve_Expor_Valor_E_Erros()
    {
        var success = Result.Success("valor");

        Assert.True(success.IsSuccess);
        Assert.Equal("valor", success.Value);
        Assert.Null(success.Error);
        Assert.Empty(success.Errors);

        var genericSuccess = Result<string>.Success("valor");

        Assert.True(genericSuccess.IsSuccess);
        Assert.Equal("valor", genericSuccess.Value);
        Assert.Null(genericSuccess.Error);
        Assert.Empty(genericSuccess.Errors);

        var failure = Result.Failure<int>("Falha generica");

        Assert.False(failure.IsSuccess);
        Assert.Equal(0, failure.Value);
        Assert.Equal("Falha generica", failure.Error);
        Assert.Empty(failure.Errors);

        var errors = new[] { "E1", "E2" };
        var failureWithErrors = Result.Failure<int>(errors);

        Assert.False(failureWithErrors.IsSuccess);
        Assert.Equal(0, failureWithErrors.Value);
        Assert.Null(failureWithErrors.Error);
        Assert.Equal(errors, failureWithErrors.Errors);

        var genericFailure = Result<string>.Failure("Falha generica");

        Assert.False(genericFailure.IsSuccess);
        Assert.Null(genericFailure.Value);
        Assert.Equal("Falha generica", genericFailure.Error);
        Assert.Empty(genericFailure.Errors);

        var genericFailureWithErrors = Result<string>.Failure(errors);

        Assert.False(genericFailureWithErrors.IsSuccess);
        Assert.Null(genericFailureWithErrors.Value);
        Assert.Null(genericFailureWithErrors.Error);
        Assert.Equal(errors, genericFailureWithErrors.Errors);
    }

    [Fact]
    public void Pagination_Deve_Normalizar_Valores_Invalidos_E_Calcular_Skip()
    {
        var pagination = new Pagination(0, -5);

        Assert.Equal(1, pagination.PageNumber);
        Assert.Equal(10, pagination.PageSize);
        Assert.Equal(0, pagination.GetSkip());

        pagination.PageNumber = 3;
        pagination.PageSize = 25;

        Assert.Equal(3, pagination.PageNumber);
        Assert.Equal(25, pagination.PageSize);
        Assert.Equal(50, pagination.GetSkip());
    }

    [Fact]
    public void OrcamentoRequests_Deve_Atribuir_Valores_Do_Construtor()
    {
        var orcamentoId = Guid.Parse("11111111-2222-3333-4444-555555555555");

        var aprovar = new AprovarOrcamentoRequest(orcamentoId);

        Assert.Equal(orcamentoId, aprovar.OrcamentoId);

        var reprovar = new ReprovarOrcamentoRequest(orcamentoId, "Cliente não aprovou o valor");

        Assert.Equal(orcamentoId, reprovar.OrcamentoId);
        Assert.Equal("Cliente não aprovou o valor", reprovar.Motivo);
    }

    [Fact]
    public void PagedResponseExtensions_Deve_Calcular_Paginacao_E_Mapear_Resultados()
    {
        var items = new[] { 10, 20, 30, 40, 50 };

        var response = items.ToPagedResponse(totalCount: 12, pageNumber: 2, pageSize: 5);

        Assert.Equal(items, response.Items);
        Assert.Equal(2, response.PageNumber);
        Assert.Equal(5, response.PageSize);
        Assert.Equal(12, response.TotalCount);
        Assert.Equal(3, response.TotalPages);
        Assert.True(response.HasNextPage);
        Assert.True(response.HasPreviousPage);
        Assert.Equal(5, response.ItemCount);
        Assert.Equal(6, response.FirstItemIndex);
        Assert.Equal(10, response.LastItemIndex);
        Assert.False(response.IsEmpty);

        var mapped = response.ToPagedResponse(item => item.ToString());

        Assert.Equal(response.Items.Select(item => item.ToString()), mapped.Items);
        Assert.Equal(response.PageNumber, mapped.PageNumber);
        Assert.Equal(response.PageSize, mapped.PageSize);
        Assert.Equal(response.TotalCount, mapped.TotalCount);
        Assert.Equal(response.TotalPages, mapped.TotalPages);
        Assert.Equal(response.HasNextPage, mapped.HasNextPage);
        Assert.Equal(response.HasPreviousPage, mapped.HasPreviousPage);
    }

    [Fact]
    public void PagedResponse_Deve_Expor_Indices_Zero_Quando_Estiver_Vazio()
    {
        var response = new PagedResponse<string>
        {
            PageNumber = 1,
            PageSize = 10
        };

        Assert.Empty(response.Items);
        Assert.Equal(0, response.ItemCount);
        Assert.Equal(0, response.FirstItemIndex);
        Assert.Equal(0, response.LastItemIndex);
        Assert.True(response.IsEmpty);
    }

    [Fact]
    public void PagedResponseExtensions_Deve_Tratar_PageSize_Invalido_E_Argumentos_Nulos()
    {
        var response = new[] { 1, 2 }.ToPagedResponse(totalCount: 0, pageNumber: 1, pageSize: 0);

        Assert.Equal(0, response.TotalPages);
        Assert.False(response.HasNextPage);
        Assert.False(response.HasPreviousPage);

        IEnumerable<int>? items = null;

        Assert.Throws<ArgumentNullException>(() => items!.ToPagedResponse(1, 1, 1));
        Assert.Throws<ArgumentNullException>(() => response.ToPagedResponse<int, string>(null!));
    }

    [Fact]
    public void DateTimeExtensions_Deve_Formatar_Datas_E_Tratar_Nulos()
    {
        var date = new DateTime(2026, 08, 16, 13, 45, 00, DateTimeKind.Utc);
        var dateOnly = new DateOnly(2026, 08, 16);

        Assert.Equal("16/08/2026", date.ToDateString());
        Assert.Equal("2026-08-16", date.ToDateString("yyyy-MM-dd"));
        Assert.Equal("16/08/2026", dateOnly.ToDateString());
        Assert.Equal("2026-08-16", dateOnly.ToDateString("yyyy-MM-dd"));

        DateTime? nullableDate = null;
        DateOnly? nullableDateOnly = null;

        Assert.Null(nullableDate.ToDateString());
        Assert.Null(nullableDateOnly.ToDateString());

        DateTime? nullableDateWithValue = date;
        DateOnly? nullableDateOnlyWithValue = dateOnly;

        Assert.Equal("16/08/2026", nullableDateWithValue.ToDateString());
        Assert.Equal("16/08/2026", nullableDateOnlyWithValue.ToDateString());
    }

    [Fact]
    public void Specification_Deve_Permitir_Configuracao_De_Criterios_E_Includes()
    {
        var specification = new TestSpecification();

        Assert.Null(specification.Criteria);
        Assert.Empty(specification.Includes);
        Assert.Null(specification.OrderBy);
        Assert.Null(specification.OrderByDescending);
        Assert.Null(specification.Take);
        Assert.Null(specification.Skip);
        Assert.False(specification.IsPagingEnabled);

        specification.Configure();

        Assert.NotNull(specification.Criteria);
        Assert.Single(specification.Includes);
        Assert.NotNull(specification.OrderBy);
        Assert.NotNull(specification.OrderByDescending);
        Assert.Equal(15, specification.Take);
        Assert.Equal(5, specification.Skip);
        Assert.True(specification.IsPagingEnabled);
    }

    [Fact]
    public void VeiculoResponse_Deve_Validar_E_Atribuir_Valores()
    {
        var response = new VeiculoResponse(
            placa: "ABC1D23",
            marca: "Ford",
            modelo: "Ka",
            anoFabricacao: 2020,
            cor: "Prata",
            hodometro: 12345,
            hodometroFormatado: "12.345 km");

        Assert.Equal("ABC1D23", response.Placa);
        Assert.Equal("Ford", response.Marca);
        Assert.Equal("Ka", response.Modelo);
        Assert.Equal(2020, response.AnoFabricacao);
        Assert.Equal("Prata", response.Cor);
        Assert.Equal(12345, response.Hodometro);
        Assert.Equal("12.345 km", response.HodometroFormatado);
    }

    [Theory]
    [MemberData(nameof(VeiculoInvalidoCases))]
    public void VeiculoResponse_Deve_Lancar_Excecao_Quando_Dados_Sao_Invalidos(Func<VeiculoResponse> factory, string mensagem)
    {
        var exception = Assert.Throws<ArgumentException>(factory);

        Assert.Equal(mensagem, exception.Message);
    }

    public static IEnumerable<object[]> VeiculoInvalidoCases()
    {
        yield return [
            () => new VeiculoResponse("", "Ford", "Ka", 2020, "Prata", 12345, "12.345 km"),
            "Placa inválida."
        ];

        yield return [
            () => new VeiculoResponse("ABC1D23", "", "Ka", 2020, "Prata", 12345, "12.345 km"),
            "Marca inválida."
        ];

        yield return [
            () => new VeiculoResponse("ABC1D23", "Ford", "Ka", 2020, "", 12345, "12.345 km"),
            "Cor inválida."
        ];

        yield return [
            () => new VeiculoResponse("ABC1D23", "Ford", "", 2020, "Prata", 12345, "12.345 km"),
            "Modelo inválido."
        ];

        yield return [
            () => new VeiculoResponse("ABC1D23", "Ford", "Ka", 0, "Prata", 12345, "12.345 km"),
            "Ano de fabricação inválido."
        ];

        yield return [
            () => new VeiculoResponse("ABC1D23", "Ford", "Ka", 2020, "Prata", -1, "12.345 km"),
            "Hodômetro inválido."
        ];
    }

    private sealed class TestSpecification : Specification<string>
    {
        public void Configure()
        {
            Criteria = value => value.Length > 0;
            AddInclude(value => value.Length);
            OrderBy = value => value.Length;
            OrderByDescending = value => value.Length;
            Take = 15;
            Skip = 5;
            IsPagingEnabled = true;
        }
    }

    private static object? CreateValue(Type type)
    {
        var nullableType = Nullable.GetUnderlyingType(type);

        if (nullableType is not null)
        {
            return CreateValue(nullableType);
        }

        if (type == typeof(string))
        {
            return "valor";
        }

        if (type == typeof(Guid))
        {
            return Guid.NewGuid();
        }

        if (type == typeof(DateTime))
        {
            return new DateTime(2026, 08, 16, 13, 45, 00, DateTimeKind.Utc);
        }

        if (type == typeof(DateOnly))
        {
            return new DateOnly(2026, 08, 16);
        }

        if (type == typeof(bool))
        {
            return true;
        }

        if (type == typeof(decimal))
        {
            return 10.5m;
        }

        if (type == typeof(int))
        {
            return 42;
        }

        if (type == typeof(long))
        {
            return 42L;
        }

        if (type == typeof(double))
        {
            return 42.0d;
        }

        if (type == typeof(float))
        {
            return 42.0f;
        }

        if (type.IsArray)
        {
            return Array.CreateInstance(type.GetElementType()!, 0);
        }

        if (type.IsGenericType)
        {
            var genericDefinition = type.GetGenericTypeDefinition();
            var genericArguments = type.GetGenericArguments();

            if (genericDefinition == typeof(IEnumerable<>) ||
                genericDefinition == typeof(IReadOnlyCollection<>) ||
                genericDefinition == typeof(ICollection<>) ||
                genericDefinition == typeof(IReadOnlyList<>) ||
                genericDefinition == typeof(IList<>))
            {
                var listType = typeof(List<>).MakeGenericType(genericArguments[0]);
                return Activator.CreateInstance(listType);
            }
        }

        var constructor = type.GetConstructor(Type.EmptyTypes);
        return constructor is null ? null : Activator.CreateInstance(type);
    }
}