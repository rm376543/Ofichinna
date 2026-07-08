# ADR-005 - Organização da Solução por Camadas e Projetos

- **Status:** Aceita
- **Data:** 2026-07-05
- **Autores:** Equipe de Arquitetura

---

# Contexto

A aplicação foi concebida para suportar um domínio de negócio com regras complexas, evolução contínua e necessidade de alta manutenibilidade.

À medida que a solução cresce, torna-se importante organizar o código de forma que cada responsabilidade permaneça isolada, reduzindo o acoplamento entre componentes e facilitando a evolução independente das diferentes partes da aplicação.

Além disso, a organização física da solução deve refletir sua arquitetura lógica, tornando explícitas as dependências permitidas entre os projetos.

---

# Decisão

A solução será organizada em projetos independentes, cada um representando uma camada da arquitetura.

A estrutura da solução será composta pelos seguintes projetos:

- **Ofichina.Api**
- **Ofichina.Application**
- **Ofichina.Domain**
- **Ofichina.Contracts**
- **Ofichina.Infrastructure**
- **Ofichina.Bootstrap**

Cada projeto possuirá responsabilidades claramente definidas e dependerá apenas das camadas permitidas pela arquitetura.

---

# Responsabilidades

## Domain

Representa o núcleo da aplicação.

É responsável por:

- Entidades;
- Value Objects;
- Aggregates;
- Domain Services;
- Interfaces de repositórios;
- Regras de negócio;
- Exceções de domínio.

O Domain não possui dependência de nenhuma outra camada da solução.

---

## Application

Responsável pela implementação dos casos de uso da aplicação.

Inclui:

- Commands;
- Queries;
- Handlers;
- Validators;
- Interfaces de serviços;
- Orquestração das regras de negócio.

A camada Application utiliza os contratos definidos em **Contracts** e as abstrações disponibilizadas pelo **Domain**.

Não possui conhecimento sobre detalhes de infraestrutura.

---

## Contracts

Centraliza todos os contratos públicos utilizados na comunicação da aplicação.

Inclui:

- Requests;
- Responses;
- DTOs;
- Objetos compartilhados entre API e Application.

Seu objetivo é desacoplar os contratos públicos dos modelos internos da aplicação.

---

## Infrastructure

Responsável pelas implementações técnicas da solução.

Inclui:

- Persistência de dados;
- Implementação dos repositórios;
- Entity Framework Core;
- Autenticação;
- Autorização;
- Integrações externas;
- Configurações de infraestrutura.

Nenhuma regra de negócio deverá ser implementada nesta camada.

---

## Api

Representa a camada de apresentação.

É responsável por:

- Controllers;
- Endpoints;
- Middlewares;
- Configuração HTTP;
- Exposição da API REST.

A API atua apenas como ponto de entrada da aplicação.

---

## Bootstrap

Responsável pela composição da aplicação.

Inclui:

- Registro das dependências;
- Configuração dos módulos;
- Inicialização da Infrastructure;
- Inicialização da Application;
- Configuração dos serviços compartilhados.

Seu objetivo é impedir que a camada Api conheça diretamente detalhes de implementação das demais camadas.

---

# Dependências entre Projetos

As dependências deverão obedecer à seguinte estrutura:

```
                          +----------------+
                          |      Api       |
                          +-------+--------+
                                  |
                                  v
                          +----------------+
                          |   Bootstrap    |
                          +-------+--------+
                                  |
                   +--------------+--------------+
                   |                             |
                   v                             v
          +----------------+            +--------------------+
          |  Application   |            |   Infrastructure   |
          +-------+--------+            +---------+----------+
                  |                                |
          +-------+-------+                        |
          |               |                        |
          v               v                        |
+----------------+  +----------------+             |
|    Domain      |  |   Contracts    |             |
+----------------+  +----------------+             |
         ^                                         |
         +-----------------------------------------+
```

Regras de dependência:

- Domain não depende de nenhum projeto.
- Contracts não depende de nenhum projeto da solução.
- Application depende apenas de Domain e Contracts.
- Infrastructure depende de Domain.
- Bootstrap depende de Application e Infrastructure.
- Api depende apenas de Bootstrap.

Essa organização garante que detalhes técnicos permaneçam isolados das regras de negócio.

---

# Justificativa

A organização da solução em projetos independentes proporciona:

## Separação de Responsabilidades

Cada projeto possui uma responsabilidade única e bem definida.

Isso reduz o acoplamento entre componentes e facilita a compreensão da arquitetura.

---

## Independência Tecnológica

As regras de negócio permanecem isoladas das tecnologias utilizadas na infraestrutura.

Mudanças em banco de dados, autenticação ou integrações externas não impactam diretamente o domínio.

---

## Manutenibilidade

A divisão em projetos menores facilita:

- Navegação no código;
- Revisão de código;
- Refatorações;
- Evolução da solução.

---

## Testabilidade

A separação entre camadas permite testar o domínio e os casos de uso independentemente dos detalhes técnicos.

---

## Escalabilidade

A estrutura facilita o crescimento da solução sem comprometer sua organização arquitetural.

Novas funcionalidades podem ser adicionadas preservando os limites entre os componentes.

---

# Alternativas Consideradas

## Projeto Único (Monolítico)

### Vantagens

- Estrutura simples.
- Configuração reduzida.

### Desvantagens

- Alto acoplamento.
- Baixa organização.
- Dificuldade de manutenção.
- Crescimento desordenado do código.

### Motivo da rejeição

Não atende aos requisitos de evolução e manutenção da aplicação.

---

## Organização apenas por Pastas

### Vantagens

- Menor quantidade de projetos.
- Simplicidade inicial.

### Desvantagens

- Não impede dependências indevidas.
- Baixo isolamento entre responsabilidades.
- Maior risco de violação da arquitetura.

### Motivo da rejeição

A organização física da solução deve reforçar as restrições arquiteturais por meio de projetos independentes.

---

# Consequências

## Positivas

- Melhor organização da solução.
- Redução do acoplamento.
- Maior facilidade para testes.
- Evolução independente das camadas.
- Arquitetura mais explícita.
- Melhor escalabilidade da base de código.

---

## Negativas

- Maior quantidade de projetos.
- Configuração inicial mais complexa.
- Necessidade de disciplina para manter as dependências corretas.

---

# Impacto Arquitetural

A adoção desta decisão estabelece que:

- Cada camada deverá permanecer em seu respectivo projeto.
- Dependências deverão seguir exclusivamente a direção definida nesta ADR.
- Não serão permitadas referências diretas que violem a arquitetura.
- O Bootstrap será responsável pela composição da aplicação.
- A Api não deverá conhecer detalhes de implementação da Infrastructure.
- O Domain permanecerá completamente independente das demais camadas.

---

# Revisão

Esta decisão poderá ser revisada caso:

- A arquitetura da solução seja alterada significativamente;
- Novos requisitos justifiquem outra organização estrutural;
- A evolução do projeto indique uma forma mais adequada de modularização.