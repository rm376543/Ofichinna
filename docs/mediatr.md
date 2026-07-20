# MediatR no Ofichina

## Visão geral

O projeto passou a usar **MediatR** para desacoplar a camada de API da camada de aplicação.

O fluxo principal ficou assim:

1. A API recebe a requisição HTTP.
2. O controller cria um `Command` ou `Query`.
3. O `IMediator` encaminha a mensagem para o handler correspondente.
4. O handler executa a regra de negócio e retorna um `Result` ou `Result<T>`.

## Estrutura adotada

- `src/Ofichina.Application/Abstractions`
  - Contratos base para commands e queries.
- `src/Ofichina.Application/UseCases`
  - Mensagens e handlers organizados por caso de uso.
- `src/Ofichina.Application/DependencyInjection/HandlersModule.cs`
  - Registro do MediatR no container de DI.
- `src/Ofichina.Api/Controllers`
  - Controllers chamando o `IMediator` diretamente.

## Padrão de implementação

### Commands e Queries

As mensagens continuam agrupadas por contexto funcional, como `Agendamentos`, `Pessoas`, `Veiculos` e `OrdensServico`.

### Handlers

Os handlers seguem a assinatura do MediatR e preservam os retornos baseados em `Result`.

### API

Os controllers apenas validam a entrada e delegam a execução ao mediator, mantendo a camada HTTP mais simples e focada em transporte.

## Como adicionar um novo caso de uso

1. Criar o `Command` ou `Query` em `UseCases/<Contexto>/Commands` ou `Queries`.
2. Criar o handler correspondente em `UseCases/<Contexto>/Handlers`.
3. Registrar os validadores, se houver.
4. Consumir a mensagem no controller via `IMediator.Send(...)`.

## Observações

- O registro do MediatR é feito por varredura do assembly da aplicação.
- A implementação mantém a estrutura atual do projeto e reduz acoplamento entre controllers e handlers.
