# Documentação do ReportGenerator

## Objetivo

Este documento descreve como utilizar o **ReportGenerator** para gerar e visualizar relatórios detalhados de cobertura de testes no projeto Ofichinna.

O ReportGenerator transforma os arquivos de cobertura gerados pelo Coverlet em relatórios HTML interativos, facilitando a análise de quais linhas de código foram cobertas pelos testes.

---

## Pré-requisitos

Antes de utilizar o ReportGenerator, certifique-se de que:

1. **.NET SDK** está instalado (versão 6.0 ou superior)
   - Verifique com: `dotnet --version`

2. **Coverlet.collector** está configurado nos projetos de teste
   - Já configurado em `tests/Ofichina.UnitTests/Ofichina.UnitTests.csproj`

3. **Testes executados com coleta de cobertura**
   - Os testes devem ser executados com o parâmetro `--collect:"XPlat Code Coverage"`
   - Exemplo:
	 ```powershell
	 dotnet test --collect:"XPlat Code Coverage"
	 ```

---

## Instalação

O ReportGenerator é uma ferramenta global do .NET que deve ser instalada uma única vez:

```powershell
dotnet tool install -g dotnet-reportgenerator-globaltool
```

### Atualização (caso já esteja instalado)

Para atualizar para a versão mais recente:

```powershell
dotnet tool update -g dotnet-reportgenerator-globaltool
```

### Verificação da instalação

Após instalar, verifique se o comando está disponível:

```powershell
reportgenerator -version
```

---

## Geração do Relatório de Cobertura

### Passo 1: Executar os testes com coleta de cobertura

Antes de gerar o relatório, execute os testes do projeto com coleta de cobertura habilitada:

```powershell
dotnet test --collect:"XPlat Code Coverage"
```

Esse comando:
- Executa todos os testes da solução
- Gera arquivos de cobertura no formato Cobertura XML
- Armazena os resultados em `TestResults/<guid>/coverage.cobertura.xml`

### Passo 2: Gerar o relatório HTML

Após a execução dos testes, utilize o comando principal do ReportGenerator:

```powershell
reportgenerator -reports:"**/TestResults/**/coverage.cobertura.xml" -targetdir:"coverage-report" -reporttypes:Html
```

#### Explicação dos parâmetros:

| Parâmetro | Descrição |
|-----------|-----------|
| `-reports:"**/TestResults/**/coverage.cobertura.xml"` | Padrão glob que localiza todos os arquivos de cobertura gerados pelos testes. O `**` busca recursivamente em subdiretórios. |
| `-targetdir:"coverage-report"` | Diretório de destino onde o relatório HTML será gerado. Será criado automaticamente caso não exista. |
| `-reporttypes:Html` | Formato do relatório (HTML). Outros formatos disponíveis: `HtmlSummary`, `Badges`, `Cobertura`, `lcov`, etc. |

#### Saída esperada:

Após executar o comando, você verá uma saída similar a:

```
2025-01-XX XX:XX:XX Info: Parsing 3 coverage files
2025-01-XX XX:XX:XX Info: Analyzing 42 classes
2025-01-XX XX:XX:XX Info: Writing report file 'coverage-report\index.html'
2025-01-XX XX:XX:XX Info: Report generated in XXs
```

O diretório `coverage-report/` será criado na raiz do projeto contendo:
- `index.html` - página principal do relatório
- Arquivos CSS e JavaScript de suporte
- Relatórios detalhados por namespace, classe e método

---

## Visualização do Relatório

### Passo 3: Abrir o relatório no navegador

Após gerar o relatório HTML, abra-o no navegador padrão do sistema utilizando o comando:

```powershell
start .\coverage-report\index.html
```

#### Como funciona:

- O comando `start` no PowerShell abre arquivos com o aplicativo padrão associado
- O caminho `.\coverage-report\index.html` é relativo ao diretório atual (raiz do projeto)
- O navegador padrão será aberto automaticamente exibindo o relatório

#### O que você verá no relatório:

O relatório HTML interativo contém:

1. **Visão Geral (Summary)**
   - Percentual total de cobertura de linhas
   - Percentual de cobertura de branches
   - Número total de classes e métodos analisados

2. **Navegação por Namespaces**
   - Lista todos os namespaces do projeto
   - Mostra a cobertura individual de cada namespace

3. **Detalhamento por Classe**
   - Ao clicar em um namespace, exibe as classes contidas nele
   - Mostra métricas de cobertura por classe

4. **Visualização de Código-fonte**
   - Ao clicar em uma classe, exibe o código-fonte com destaque:
     - **Verde**: linhas cobertas pelos testes
     - **Vermelho**: linhas não cobertas
     - **Amarelo**: linhas parcialmente cobertas (branches)

5. **Gráficos e Estatísticas**
   - Gráficos de pizza e barras
   - Histórico de cobertura (se executado múltiplas vezes)

#### Navegação alternativa:

Se preferir, você pode abrir o relatório manualmente:
1. Navegue até a pasta `coverage-report` na raiz do projeto
2. Clique duas vezes no arquivo `index.html`

---

## Fluxo Completo (Resumo)

Para facilitar, aqui está o fluxo completo em sequência:

```powershell
# 1. Executar os testes com coleta de cobertura
dotnet test --collect:"XPlat Code Coverage"

# 2. Gerar o relatório HTML
reportgenerator -reports:"**/TestResults/**/coverage.cobertura.xml" -targetdir:"coverage-report" -reporttypes:Html

# 3. Abrir o relatório no navegador
start .\coverage-report\index.html
```

---

## Observações e Boas Práticas

### ⚠️ Caminhos Relativos

- Todos os comandos devem ser executados a partir da **raiz do projeto** (onde está o arquivo `.slnx`)
- O padrão `**/TestResults/**/coverage.cobertura.xml` busca arquivos recursivamente
- O diretório `coverage-report` será criado na raiz do projeto

### 🔄 Atualização do Relatório

- **Sempre execute os testes antes de gerar o relatório**
- O ReportGenerator sobrescreve o diretório `coverage-report` a cada execução
- Para manter histórico, renomeie o diretório antes de gerar um novo relatório

### 🚨 Erros Comuns

#### Erro: "No coverage reports found"

**Causa:** Os testes não foram executados com coleta de cobertura ou os arquivos não foram encontrados.

**Solução:**
```powershell
# Execute os testes com o parâmetro de coleta
dotnet test --collect:"XPlat Code Coverage"

# Verifique se os arquivos foram gerados
Get-ChildItem -Recurse -Filter "coverage.cobertura.xml"
```

#### Erro: "reportgenerator is not recognized"

**Causa:** O ReportGenerator não está instalado ou não está no PATH.

**Solução:**
```powershell
# Instale a ferramenta globalmente
dotnet tool install -g dotnet-reportgenerator-globaltool

# Ou atualize se já estiver instalado
dotnet tool update -g dotnet-reportgenerator-globaltool
```

#### Erro: "Access denied" ao abrir index.html

**Causa:** Problemas de permissão ou antivírus bloqueando o arquivo.

**Solução:**
- Execute o PowerShell como Administrador
- Ou abra o arquivo manualmente navegando até `coverage-report/index.html`

### 📁 Arquivos Gerados

Os seguintes arquivos/diretórios são criados durante o processo:

```
Ofichinna/
├── TestResults/              # Gerado por dotnet test
│   └── {guid}/
│       └── coverage.cobertura.xml
└── coverage-report/          # Gerado por reportgenerator
    ├── index.html           # ← Relatório principal
    ├── *.css
    ├── *.js
    └── ...
```

**Recomendação:** Adicione ao `.gitignore`:
```gitignore
# Cobertura de testes
TestResults/
coverage-report/
```

### 💡 Dicas

1. **Integração com CI/CD**: O ReportGenerator pode gerar badges e relatórios em formato Cobertura para integração com ferramentas de CI/CD.

2. **Filtros de cobertura**: Use parâmetros como `-classfilters` e `-filefilters` para excluir arquivos gerados automaticamente ou classes de infraestrutura.

3. **Múltiplos formatos**: Você pode gerar vários formatos ao mesmo tempo:
   ```powershell
   reportgenerator -reports:"**/TestResults/**/coverage.cobertura.xml" -targetdir:"coverage-report" -reporttypes:"Html;Badges;TextSummary"
   ```

4. **Análise detalhada**: Para identificar rapidamente áreas não cobertas, filtre por "Low Coverage" no relatório HTML.

---

## Referências

- [Documentação oficial do ReportGenerator](https://github.com/danielpalme/ReportGenerator)
- [Documentação do Coverlet](https://github.com/coverlet-coverage/coverlet)
- [Cobertura de código no .NET](https://learn.microsoft.com/pt-br/dotnet/core/testing/unit-testing-code-coverage)

---
