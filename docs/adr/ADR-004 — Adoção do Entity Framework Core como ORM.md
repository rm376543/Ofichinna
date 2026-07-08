# ADR-004 - Adoção do Entity Framework Core como ORM

- **Status:** Aceita
- **Data:** 2026-07-05
- **Autores:** Equipe de Arquitetura

---

# Contexto

A aplicação adota os princípios da Clean Architecture e do Domain-Driven Design (DDD), nos quais a persistência de dados é considerada um detalhe de infraestrutura.

O domínio define apenas os contratos necessários para acesso aos dados, enquanto a implementação da persistência é responsabilidade exclusiva da camada **Infrastructure**.

Diante da necessidade de persistir entidades de domínio, controlar a evolução do banco de dados e manter integração com o ecossistema .NET, tornou-se necessária a adoção de um Object-Relational Mapper (ORM).

---

# Decisão

Foi adotado o **Entity Framework Core (EF Core)** como ORM oficial da aplicação.

Sua utilização será restrita à camada **Infrastructure**, sendo responsável por:

- Implementação do `DbContext`;
- Configuração dos mapeamentos das entidades;
- Implementação dos repositórios;
- Execução de migrations;
- Persistência e recuperação de dados.

As demais camadas da aplicação (**Domain**, **Application**, **Contracts** e **Presentation**) não deverão possuir dependência direta do Entity Framework Core.

A comunicação entre a camada Application e a Infrastructure ocorrerá exclusivamente por meio das interfaces de repositório definidas no domínio.

---

# Justificativa

A adoção do Entity Framework Core foi motivada pelos seguintes fatores.

## Integração com o ecossistema .NET

O Entity Framework Core é o ORM oficial da plataforma .NET, oferecendo integração nativa com:

- Injeção de dependência;
- Configuração da aplicação;
- Logging;
- LINQ;
- Migrations.

Essa integração reduz o esforço de configuração e manutenção.

---

## Isolamento da persistência

Ao restringir sua utilização à camada Infrastructure, a tecnologia de persistência permanece isolada das regras de negócio.

Essa abordagem mantém o domínio independente de frameworks e facilita futuras substituições da tecnologia de acesso aos dados.

---

## Produtividade

O EF Core reduz significativamente a quantidade de código necessário para operações de persistência.

Além disso, fornece recursos como:

- Change Tracking;
- Materialização automática de objetos;
- Configuração fluente de entidades;
- Gerenciamento de relacionamentos.

---

## Evolução do banco de dados

O suporte a **Migrations** permite versionar o esquema do banco de dados juntamente com o código-fonte da aplicação.

Isso favorece ambientes de desenvolvimento, homologação e produção.

---

## Manutenibilidade

A padronização da persistência utilizando um único ORM facilita a manutenção da solução e reduz a complexidade técnica do projeto.

---

# Alternativas Consideradas

## ADO.NET

### Vantagens

- Controle total sobre comandos SQL.
- Nenhuma camada adicional de abstração.

### Desvantagens

- Alto volume de código repetitivo.
- Mapeamento manual entre banco e objetos.
- Maior custo de manutenção.
- Maior possibilidade de inconsistências.

### Motivo da rejeição

Embora ofereça maior controle sobre as consultas SQL, não proporciona o nível de produtividade e padronização desejado para o projeto.

---

## Dapper

### Vantagens

- Excelente desempenho.
- Controle explícito das consultas SQL.
- Baixa sobrecarga.

### Desvantagens

- Mapeamentos realizados manualmente.
- Ausência de Change Tracking.
- Não possui suporte nativo para migrations.
- Maior responsabilidade da aplicação na persistência.

### Motivo da rejeição

O projeto prioriza produtividade, padronização e facilidade de manutenção em detrimento de ganhos pontuais de desempenho.

O uso do Dapper poderá ser avaliado futuramente para cenários específicos de leitura com requisitos elevados de performance.

---

# Consequências

## Positivas

- Padronização do acesso aos dados.
- Redução de código repetitivo.
- Evolução controlada do banco de dados por meio de migrations.
- Melhor integração com o ecossistema .NET.
- Isolamento da tecnologia de persistência na camada Infrastructure.
- Maior produtividade no desenvolvimento.

---

## Negativas

- Curva de aprendizado para recursos avançados do EF Core.
- Necessidade de atenção ao desempenho de consultas complexas.
- Dependência do Entity Framework Core restrita à camada Infrastructure.

---

# Impacto Arquitetural

A adoção desta decisão estabelece que:

- Apenas a camada **Infrastructure** poderá referenciar o Entity Framework Core.
- O **Domain** permanece independente de tecnologias de persistência.
- A **Application** acessará os dados exclusivamente por meio das interfaces definidas no domínio.
- A camada **Contracts** não possuirá qualquer dependência do ORM.
- O acesso direto ao `DbContext` fora da Infrastructure não é permitido.
- Alterações no esquema do banco deverão ser controladas por meio de migrations.

---

# Revisão

Esta decisão poderá ser revisada caso:

- Novos requisitos de desempenho justifiquem outra estratégia de persistência;
- Seja necessária a adoção de múltiplos mecanismos de acesso a dados;
- A evolução tecnológica do ecossistema .NET indique uma alternativa mais adequada ao contexto da aplicação.