# 📚 OFICHINNA - Clean Architecture .NET 10

## 🎯 Visão Geral

Projeto Ofichinna implementado com **Clean Architecture** e **CQRS Pattern**, utilizando .NET 10 com SQL Server e Entity Framework Core.

### ✅ Status
- **Implementação**: Completa
- **Build**: ✅ Sucesso (sem erros)
- **Documentação**: ✅ Base consolidada e em evolução
- **Pronto para**: Evolução de features e documentação

---

## 📖 Documentação

### 🔴 COMECE AQUI
1. **[🎯 START_HERE.md](./START_HERE.md)** - Ponto de partida rápido
   - Visão resumida da solução
   - Próximos passos
   - Ordem de leitura
2. **[📋 SUMARIO_EXECUTIVO.md](./SUMARIO_EXECUTIVO.md)** - Visão geral de 5 minutos
   - Status da implementação
   - Números e métricas
   - Quick start
   - Próximos passos

### 🔵 ENTENDA A ARQUITETURA
3. **[🏗️ MAPA_VISUAL.md](./MAPA_VISUAL.md)** - Diagramas e visualizações
   - Estrutura hierárquica completa
   - Fluxo de requisição
   - Ciclo de inicialização
   - Matriz de dependências
   - Exemplo de extensão
   - Dicas de desenvolvimento

4. **[📐 ARQUITETURA.md](./ARQUITETURA.md)** - Referência técnica detalhada
   - Descrição de cada camada
   - Padrões implementados
   - Estrutura de pastas
   - Dependências de NuGet
   - Como usar cada pattern
   - Notas importantes

### 🟢 IMPLEMENTE FEATURES
5. **[📖 GUIA_IMPLEMENTACAO.md](./GUIA_IMPLEMENTACAO.md)** - Guia prático passo-a-passo
   - Exemplo prático completo
   - Checklist para nova feature
   - Padrões de design
   - Código exemplo comentado

6. **[📎 API_REFERENCE.md](./API_REFERENCE.md)** - Referência da API
   - Endpoints de todos os controllers
   - Exemplos de request/response
   - Códigos de status

7. **[🧩 DOMINIO_FEATURES.md](./DOMINIO_FEATURES.md)** - Entidades e features
   - Domínios, relacionamentos e operações disponíveis

8. **[🤝 CONTRIBUTING.md](./CONTRIBUTING.md)** - Guia de contribuição
   - Regras de documentação
   - Padrões de entrega
   - Checklist antes de PR

9. **[📐 DESIGN_APPROVAL_SHEET.md](./DESIGN_APPROVAL_SHEET.md)** - Template de aprovação de design
   - Obrigatório antes de features de negócio significativas
   - Complementar aos ADRs
   - Exemplo preenchido em `das/`

### 🟡 VALIDAÇÃO E RASTREAMENTO
9. **[✅ RELATORIO_IMPLEMENTACAO.md](./RELATORIO_IMPLEMENTACAO.md)** - Relatório completo
   - Base consolidada da implementação
   - Estrutura de pastas
   - Padrões de design
   - Validação
   - Estatísticas
   - Checklist de qualidade

10. **[🧰 TROUBLESHOOTING.md](./TROUBLESHOOTING.md)** - Suporte operacional
   - Banco de dados
   - Migrations
   - SonarQube

---

## 🏗️ Estrutura de Projetos

```
Ofichinna/
│
├─ Ofichina.Bootstrap/      [Layer: Composição]
│
├─ src/
│  ├─ Ofichina.Api/              [Layer: Apresentação]
│  ├─ Ofichina.Contracts/        [Layer: Contratos]
│  ├─ Ofichina.Application/      [Layer: Aplicação]
│  ├─ Ofichina.Domain/           [Layer: Domínio]
│  └─ Ofichina.Infrastructure/   [Layer: Infraestrutura]
│
├─ tests/
│  ├─ Ofichina.UnitTests/
│  └─ Ofichina.IntegrationTests/
│
└─ docs/
   ├─ README.md
   ├─ START_HERE.md
   ├─ QUICK_REFERENCE.md
   ├─ SUMARIO_EXECUTIVO.md
   ├─ API_REFERENCE.md
   ├─ DESIGN_APPROVAL_SHEET.md
   ├─ DOMINIO_FEATURES.md
   ├─ LOGGING.md
   ├─ EXEMPLOS_CORRELATION_ID.md
   ├─ DOCUMENTACAO_COMPLETA.md
   ├─ CONCLUSAO.md
   ├─ RESUMO_FINAL.md
   ├─ mediatr.md
   ├─ MAPA_VISUAL.md
   ├─ ARQUITETURA.md
   ├─ GUIA_IMPLEMENTACAO.md
   ├─ AUTORIZACAO-RBAC-POLICIES.md
   ├─ TROUBLESHOOTING.md
   ├─ CONTRIBUTING.md
   ├─ INDICE.md
   ├─ RELATORIO_IMPLEMENTACAO.md
   ├─ das/
   ├─ DAS-001-exemplo-ordem-servico.md
   └─ DAS-002-fluxo-orcamento.md
   ├─ adr/
   │  ├─ ADR-001 - Clean-Architecture-DDD.md
   │  ├─ ADR-002 - Adocao-CQRS-Application.md
   │  ├─ ADR-003 - Banco-Dados-Relacional.md
   │  ├─ ADR-004 - Adoção do Entity Framework Core como ORM.md
   │  ├─ ADR-005 - Organização da Solução por Camadas e Projetos.md
   │  ├─ ADR-006 - Estratégia para Validações com FluentValidation.md
   │  ├─ ADR-007 - Autorizacao-RBAC-e-Policies.md
   │  └─ ADR-008 - Estrategia-Validacoes-FluentValidation.md
   └─ Documentacao Inicial Projeto/
```

---

## 🚀 Quick Start

### 1. Configurar Banco de Dados
```json
// appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=Ofichinna;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

### 2. Criar Migrations
```bash
dotnet ef migrations add InitialAuth -p src/Ofichina.Infrastructure
dotnet ef database update -p src/Ofichina.Infrastructure
```

> O projeto `Ofichina.Infrastructure` já possui `ApplicationDbContextFactory` para suportar o `dotnet ef` em design-time.

### 3. Executar
```bash
dotnet run --project src/Ofichina.Api
```

Acesse: `https://localhost:7000/swagger`

---

## 🎯 Padrões Implementados

| Padrão | Local | Descrição |
|--------|-------|-----------|
| **CQRS** | Application/Abstractions | Commands (escrita) e Queries (leitura) |
| **Repository** | Domain/Interfaces + Infrastructure/Repositories | Abstração de persistência |
| **Unit of Work** | Domain/Interfaces + Infrastructure/Repositories | Gerenciamento de transações |
| **Specification** | Domain/Specifications | Critérios de query encapsulados |
| **Result** | Domain/Shared | Resultado com sucesso/erro |
| **Value Object** | Domain/Shared | Objetos imutáveis por valor |
| **DI** | Application/DependencyInjection | Modularidade e inversão de controle |
| **Validation** | Application/Validators | FluentValidation integrado |

---

## 📦 Dependências

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="10.0.9" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="10.0.9" />
<PackageReference Include="FluentValidation" Version="11.9.2" />
<PackageReference Include="FluentValidation.DependencyInjectionExtensions" Version="11.9.2" />
```

---

## 🔄 Fluxo de Inicialização

```
Program.cs
    ↓
AddBootstrapMiddleware(configuration)
    ├─ AddAuthorizationModule()
    ├─ AddAuthenticationModules(configuration)
    ├─ AddAuthorizationResultHandlerModule()
    ├─ AddAuthenticationServices()
    ├─ AddApplication()
    └─ AddInfrastructure(configuration)
```

---

## 📋 Exemplo: Criar Nova Entidade

Seguindo este passo-a-passo (consulte GUIA_IMPLEMENTACAO.md para detalhes):

```
1. Criar Entidade em Domain/Entities/
2. Criar Interface em Domain/Interfaces/
3. Implementar Repositório em Infrastructure/Repositories/
4. Registrar em Infrastructure/Modules/RepositoryModule.cs
5. Adicionar DbSet em ApplicationDbContext
6. Criar Validador em Application/Validators/
7. Criar Command/Query em Application/UseCases/
8. Criar Handler em Application/UseCases/Handlers/
9. Criar Controller em Api/Controllers/
```

Veja exemplo completo em **GUIA_IMPLEMENTACAO.md**

---

## 📊 Estatísticas

| Métrica | Valor |
|---------|-------|
| Projetos | 9 |
| Documentos Markdown | 30 (21 na raiz + 1 DAS + 8 ADRs) |
| Controllers documentados | 13 |
| Endpoints documentados | 63 |
| Entidades/features | 9 domínios principais |
| Padrões de Design | CQRS, Repository, Unit of Work, Specification, Result, Value Object |
| Módulos da API | Swagger, Correlation ID, exceções |
| Camadas/projetos | 10 projetos na solução |
| Build Status | ✅ Sucesso |
| Erros | 0 |

---

## 🎓 Referências e Leitura

### Documentação Oficial
- [Clean Architecture - Uncle Bob](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [CQRS Pattern - Martin Fowler](https://martinfowler.com/bliki/CQRS.html)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [FluentValidation](https://docs.fluentvalidation.net/)
- [Microsoft.Extensions.DependencyInjection](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection)

### Documentação Local
- 📖 [START_HERE.md](./START_HERE.md) - Ponto de partida
- 📖 [QUICK_REFERENCE.md](./QUICK_REFERENCE.md) - Comandos e consulta rápida
- 📖 [SUMARIO_EXECUTIVO.md](./SUMARIO_EXECUTIVO.md) - Visão geral
- 📖 [MAPA_VISUAL.md](./MAPA_VISUAL.md) - Diagramas
- 📖 [ARQUITETURA.md](./ARQUITETURA.md) - Referência técnica
- 📖 [GUIA_IMPLEMENTACAO.md](./GUIA_IMPLEMENTACAO.md) - Guia prático
- 📖 [DOMINIO_FEATURES.md](./DOMINIO_FEATURES.md) - Entidades e features
- 📖 [RELATORIO_IMPLEMENTACAO.md](./RELATORIO_IMPLEMENTACAO.md) - Relatório
- 📖 [API_REFERENCE.md](./API_REFERENCE.md) - Contratos e exemplos da API
- 📐 [DESIGN_APPROVAL_SHEET.md](./DESIGN_APPROVAL_SHEET.md) - Template de aprovação de design
- 📐 [das/DAS-001-exemplo-ordem-servico.md](./das/DAS-001-exemplo-ordem-servico.md) - Exemplo preenchido
- 📐 [das/DAS-002-fluxo-orcamento.md](./das/DAS-002-fluxo-orcamento.md) - Aprovação do fluxo de orçamento
- 📖 [LOGGING.md](./LOGGING.md) - Serilog, Seq e correlação
- 📖 [EXEMPLOS_CORRELATION_ID.md](./EXEMPLOS_CORRELATION_ID.md) - Exemplos de correlação
- 📖 [DOCUMENTACAO_COMPLETA.md](./DOCUMENTACAO_COMPLETA.md) - Mapa da documentação
- 📖 [CONCLUSAO.md](./CONCLUSAO.md) - Conclusão
- 📖 [RESUMO_FINAL.md](./RESUMO_FINAL.md) - Resumo final
- 📖 [mediatr.md](./mediatr.md) - Uso de MediatR
- 📖 [AUTORIZACAO-RBAC-POLICIES.md](./AUTORIZACAO-RBAC-POLICIES.md) - Autorização
- 📖 [INDICE.md](./INDICE.md) - Índice completo
- 📖 [adr/](./adr/) - Architecture Decision Records
- 📖 [CONTRIBUTING.md](./CONTRIBUTING.md) - Padrões de contribuição
- 📖 [TROUBLESHOOTING.md](./TROUBLESHOOTING.md) - Solução de problemas

### 🧭 Quando usar DAS ou ADR?

- Use um **DAS** antes de implementar uma feature significativa: ele descreve o problema, escopo, contratos, fluxo, testes, riscos e aprovação daquela feature.
- Use um **ADR** quando a mudança registrar uma decisão arquitetural duradoura e transversal, como Clean Architecture, CQRS, EF Core ou RBAC.
- Um DAS deve referenciar os ADRs aplicáveis; um DAS não substitui um ADR quando a feature altera a arquitetura da solução.

---

## ✅ Checklist de Implementação

### Antes de Começar
- [ ] Ler SUMARIO_EXECUTIVO.md (5 min)
- [ ] Entender arquitetura com MAPA_VISUAL.md
- [ ] Revisar ARQUITETURA.md para detalhes técnicos

### Configuração Inicial
- [ ] Configurar appsettings.json
- [ ] Executar migrations
- [ ] Testar projeto com `dotnet run`
- [ ] Acessar Swagger

### Primeira Feature
- [ ] Consultar GUIA_IMPLEMENTACAO.md
- [ ] Criar entidade (Domain)
- [ ] Criar repositório (Infrastructure)
- [ ] Criar handlers (Application)
- [ ] Criar controller (Api)
- [ ] Testar endpoints

### Continuação
- [ ] Implementar mais entidades
- [ ] Adicionar autenticação
- [ ] Criar testes
- [ ] Documentar API
- [ ] Preparar deploy

---

## 🎯 Objetivo do Projeto

Clean Architecture implementada com .NET 10, facilitando:
- ✅ Manutenibilidade
- ✅ Testabilidade
- ✅ Escalabilidade
- ✅ Separação de responsabilidades
- ✅ Reutilização de código
- ✅ Onboarding de novos desenvolvedores

---

## 💬 Dúvidas Frequentes

**P: Por onde começo?**  
R: Leia SUMARIO_EXECUTIVO.md (5 min), depois MAPA_VISUAL.md (10 min).

**P: Como adiciono uma nova entidade?**  
R: Siga o passo-a-passo em GUIA_IMPLEMENTACAO.md ou ARQUITETURA.md.

**P: O que cada camada faz?**  
R: Veja MAPA_VISUAL.md na seção "Mapeamento de Responsabilidades".

**P: Que padrões foram usados?**  
R: 8 padrões listados em ARQUITETURA.md e RELATORIO_IMPLEMENTACAO.md.

**P: Posso usar MediatR?**  
R: A solução usa abstrações próprias para commands e queries; adapte MediatR apenas se isso for necessário.

**P: Como testar?**  
R: Crie testes em projects `Ofichina.UnitTests` e `Ofichina.IntegrationTests`.

---

## 📞 Suporte

Para dúvidas sobre a implementação:
1. Consulte a documentação local
2. Revise os exemplos em GUIA_IMPLEMENTACAO.md
3. Verifique ARQUITETURA.md para padrões específicos

---

## 📝 Changelog

### v2.0 - 2026
- ✅ Clean Architecture implementada
- ✅ CQRS Pattern configurado
- ✅ 8 padrões de design implementados
- ✅ Entity Framework Core integrado
- ✅ FluentValidation configurado
- ✅ Documentação completa
- ✅ Exemplos práticos
- ✅ Build validado (0 erros)
- ✅ Fluxo de orçamento documentado na API e em DAS

---

## 🎉 Status Final

```
┌─────────────────────────────────────────┐
│ IMPLEMENTAÇÃO CONCLUÍDA COM SUCESSO ✅ │
└─────────────────────────────────────────┘

Build:           ✅ Sucesso (sem erros)
Arquitetura:     ✅ Clean Architecture
Padrões:         ✅ 8 implementados
Documentação:    ✅ 5 arquivos
Exemplos:        ✅ Inclusos
Pronto para:     ✅ Desenvolvimento
```

---

## 🚀 Começar Agora!

1. **[📋 SUMARIO_EXECUTIVO.md](./SUMARIO_EXECUTIVO.md)** - Leia agora (5 min)
2. **[🏗️ MAPA_VISUAL.md](./MAPA_VISUAL.md)** - Entenda a arquitetura (10 min)
3. **[📖 GUIA_IMPLEMENTACAO.md](./GUIA_IMPLEMENTACAO.md)** - Implemente sua feature
4. **[📎 API_REFERENCE.md](./API_REFERENCE.md)** - Consulte contratos e exemplos
5. **[🤝 CONTRIBUTING.md](./CONTRIBUTING.md)** - Siga os padrões do projeto

---

**Última atualização:** 2026  
**Versão:** 2.0  
**Status:** ✅ PRONTO PARA EVOLUIR

---

*Para questões técnicas, consulte a documentação fornecida ou revise os exemplos de implementação.*