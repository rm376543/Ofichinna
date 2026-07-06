# ADR-001 - Adoção da Clean Architecture com Domain-Driven Design (DDD)

-   **Status:** Aceita
-   **Data:** 2026-07-05
-   **Autores:** Marcio Henrique Lima de Oliveira

------------------------------------------------------------------------

# Contexto

O projeto consiste no desenvolvimento de uma aplicação corporativa em
.NET voltada ao gerenciamento de processos de negócio com regras
complexas, múltiplos perfis de usuários e evolução contínua dos
requisitos.

Espera-se que a solução apresente:

-   Crescimento contínuo de funcionalidades;
-   Evolução frequente das regras de negócio;
-   Integração com serviços externos;
-   Necessidade de testes automatizados;
-   Baixo acoplamento entre regras de negócio e detalhes técnicos;
-   Facilidade de manutenção e evolução por diferentes equipes.

Diante desse cenário, foi necessária a adoção de uma arquitetura que
privilegie a organização do domínio e reduza o impacto de mudanças
tecnológicas sobre as regras de negócio.

------------------------------------------------------------------------

# Decisão

Foi adotada a combinação de **Clean Architecture** com os princípios do
**Domain-Driven Design (DDD)** como padrão arquitetural da solução.

A aplicação será organizada nas seguintes camadas:

## Domain

-   Entidades;
-   Value Objects;
-   Aggregates;
-   Domain Services;
-   Interfaces de repositórios;
-   Regras de negócio.

## Application

-   Commands;
-   Queries;
-   Handlers;
-   DTOs;
-   Interfaces de serviços;
-   Orquestração dos casos de uso utilizando CQRS.

## Contracts

-   Requests;
-   Responses;
-   DTOs compartilhados;
-   Contratos públicos da API.

## Infrastructure

-   Persistência;
-   Implementação de repositórios;
-   Autenticação e autorização;
-   Integrações externas;
-   Configurações técnicas.

## Presentation

-   API REST;
-   Controllers;
-   Middlewares;
-   Configuração da aplicação.

As dependências da solução deverão sempre apontar para as camadas
internas, respeitando o princípio da inversão de dependência.

------------------------------------------------------------------------

# Justificativa

-   Centralização das regras de negócio no domínio.
-   Independência da infraestrutura.
-   Evolução incremental da aplicação.
-   Facilidade para testes automatizados.
-   Baixo acoplamento entre camadas.
-   Aderência aos princípios do Domain-Driven Design.
-   Organização clara da solução.

------------------------------------------------------------------------

# Princípios Arquiteturais

-   Separação de responsabilidades.
-   Dependências direcionadas para as camadas internas.
-   Isolamento das regras de negócio.
-   Comunicação entre camadas por contratos bem definidos.
-   Baixo acoplamento e alta coesão.
-   Evolução incremental da solução.

------------------------------------------------------------------------

# Alternativas Consideradas

## Arquitetura em Camadas Tradicional (N-Tier)

**Motivo da rejeição:** maior acoplamento entre regras de negócio e
infraestrutura, dificultando a evolução do sistema.

## Arquitetura CRUD Tradicional

**Motivo da rejeição:** adequada para sistemas simples, mas insuficiente
para um domínio complexo e em constante evolução.

## Vertical Slice Architecture

**Motivo da rejeição:** poderá ser adotada futuramente como forma de
organização da camada Application, mantendo a Clean Architecture como
estrutura principal.

------------------------------------------------------------------------

# Consequências

## Positivas

-   Organização da solução;
-   Facilidade de manutenção;
-   Melhor testabilidade;
-   Independência da infraestrutura;
-   Melhor representação do domínio;
-   Facilidade para implementação de novos casos de uso.

## Negativas

-   Maior esforço inicial;
-   Curva de aprendizado mais elevada;
-   Necessidade de disciplina arquitetural.

------------------------------------------------------------------------

# Impacto Arquitetural

-   O domínio é o núcleo da aplicação.
-   Os casos de uso são implementados na camada Application.
-   Os contratos públicos ficam centralizados na camada Contracts.
-   A Infrastructure contém apenas detalhes técnicos.
-   A Presentation atua como ponto de entrada da aplicação.

------------------------------------------------------------------------

# Revisão

Esta decisão poderá ser revisada caso novos requisitos ou mudanças
tecnológicas justifiquem a adoção de outro modelo arquitetural.
