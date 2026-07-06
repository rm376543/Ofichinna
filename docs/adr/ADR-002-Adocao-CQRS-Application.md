# ADR-002 - Adoção de CQRS na Camada Application

-   **Status:** Aceita
-   **Data:** 2026-07-05
-   **Autores:** Equipe de Arquitetura

------------------------------------------------------------------------

# Contexto

A aplicação adota Clean Architecture e Domain-Driven Design (DDD),
separando o domínio das responsabilidades de infraestrutura e
apresentação.

À medida que o número de casos de uso aumenta, torna-se importante
evitar que serviços de aplicação concentrem responsabilidades de leitura
e escrita, reduzindo a coesão e dificultando a manutenção.

------------------------------------------------------------------------

# Decisão

Foi adotado o padrão **CQRS (Command Query Responsibility Segregation)**
exclusivamente na camada **Application**.

Os casos de uso serão organizados em:

-   **Commands:** representam operações que modificam o estado da
    aplicação.
-   **Queries:** representam operações responsáveis apenas por consulta.
-   **Handlers:** implementam a lógica de execução dos Commands e
    Queries.

Os objetos de entrada e saída dos casos de uso serão definidos na camada
**Contracts**, desacoplando a camada Application dos contratos públicos
utilizados pela API e demais consumidores.

A adoção de CQRS neste projeto é organizacional e **não implica**:

-   Banco de dados segregado para leitura e escrita;
-   Event Sourcing;
-   Mensageria;
-   Arquitetura distribuída.

------------------------------------------------------------------------

# Justificativa

-   Separação clara entre operações de escrita e leitura.
-   Casos de uso menores e com responsabilidade única.
-   Melhor organização da camada Application.
-   Reutilização dos contratos definidos na camada Contracts.
-   Facilidade para testes unitários dos casos de uso.
-   Redução do acoplamento entre funcionalidades.
-   Melhor escalabilidade da base de código.

------------------------------------------------------------------------

# Alternativas Consideradas

## Serviços de Aplicação Tradicionais

### Vantagens

-   Menor quantidade de classes.
-   Estrutura simples para aplicações pequenas.

### Desvantagens

-   Mistura de responsabilidades de leitura e escrita.
-   Tendência ao crescimento excessivo dos serviços.
-   Maior dificuldade de manutenção.

### Motivo da rejeição

Não atende adequadamente à evolução esperada da aplicação.

------------------------------------------------------------------------

# Consequências

## Positivas

-   Organização consistente dos casos de uso.
-   Maior legibilidade da camada Application.
-   Melhor separação de responsabilidades.
-   Facilidade para adicionar novas funcionalidades.
-   Melhor testabilidade.
-   Aderência ao princípio da responsabilidade única (SRP).

## Negativas

-   Maior quantidade de arquivos e classes.
-   Curva de aprendizado para desenvolvedores sem experiência com CQRS.
-   Necessidade de disciplina para manter a organização das
    funcionalidades.

------------------------------------------------------------------------

# Impacto Arquitetural

-   Toda alteração de estado deverá ser implementada por meio de um
    Command.
-   Toda consulta deverá ser implementada por meio de uma Query.
-   Handlers representam a implementação dos casos de uso.
-   Os objetos de entrada e saída pertencem à camada **Contracts** e são
    consumidos pela camada **Application**.
-   O domínio permanece independente do padrão CQRS.
-   A camada Presentation comunica-se com a Application utilizando
    Commands e Queries por meio dos contratos definidos na camada
    Contracts.

------------------------------------------------------------------------

# Revisão

Esta decisão poderá ser revisada caso a complexidade da aplicação
diminua significativamente ou novos requisitos justifiquem outra
estratégia para organização dos casos de uso.
