# TestInfrastructure

Quando usar:

- Faker / TestDataFactory: gerar dados de preenchimento irrelevantes para o cenário. Ex.: criar Pessoa, Veículo, Serviço, Peça com valores válidos e sem se preocupar com valores específicos.
- Builder: montar cenários complexos de agregados (Orcamento, OrdemServico) que dependem de transições de estado.
- MockFactory: apenas para mocks com configuração repetida entre vários testes, retornando `Mock<T>` para permitir `Verify`.

Exemplo de uso:

- TestDataFactory.Pessoas.Criar(p => p.AlterarNome("Nome Específico"));
- var orcamento = TestDataFactory.Orcamentos.Builder().ComItens(item1, item2).Aprovado().Build();
- var repoMock = MockFactory.OrcamentoRepository.ComGetById(orcamento);

Overrides:
- Se um valor é essencial ao teste, forneça-o explicitamente no teste via override (ex.: placa inválida, desconto zero). Não esconda valores essenciais na factory.
