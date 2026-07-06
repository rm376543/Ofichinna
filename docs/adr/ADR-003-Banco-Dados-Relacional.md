# ADR-0001 - Adoção do Microsoft SQL Server como Banco de Dados Relacional

- **Status:** Aceito
- **Data:** 2026-07-03
- **Autor:** Marcio Henrique Lima de Oliveira
- **Projeto:** Ofichina

---

# Contexto

O sistema Ofichina será responsável pelo gerenciamento de oficinas mecânicas, incluindo cadastro de clientes, veículos, ordens de serviço, peças, estoque, faturamento e histórico de manutenções.

Grande parte das informações possui relacionamentos entre si e exige consistência dos dados durante operações críticas, como abertura e encerramento de ordens de serviço, movimentação de estoque e emissão de cobranças.

Além disso, espera-se que o sistema evolua continuamente, mantendo integridade dos dados e facilidade para geração de relatórios.

---

# Problema

Qual tecnologia de persistência deve ser utilizada para armazenar os dados da aplicação, garantindo consistência, integridade, desempenho adequado e facilidade de manutenção?

---

# Objetivos

- Garantir integridade referencial dos dados.
- Suportar transações ACID.
- Facilitar consultas complexas.
- Permitir evolução do modelo de dados.
- Possuir ampla integração com o ecossistema .NET.
- Reduzir riscos tecnológicos durante o desenvolvimento.

---

# Drivers Arquiteturais

| Driver | Prioridade |
|---------|------------|
| Integridade dos dados | Alta |
| Consistência transacional | Alta |
| Facilidade de manutenção | Alta |
| Desempenho | Alta |
| Escalabilidade | Média |
| Produtividade da equipe | Alta |
| Custo operacional | Média |

---

# Alternativas Consideradas

## Opção 1 – Microsoft SQL Server

Banco de dados relacional com suporte completo a transações ACID, integridade referencial, índices, procedures, views e recursos avançados de administração.

### Vantagens

- Excelente integração com .NET e Entity Framework Core.
- Forte suporte a transações.
- Integridade referencial nativa.
- Ferramentas maduras de administração.
- Excelente documentação.
- Grande adoção no mercado corporativo.
- Alto desempenho para sistemas transacionais.

### Desvantagens

- Licenciamento pode gerar custos em ambientes de produção (dependendo da edição).
- Maior consumo de recursos em comparação com bancos mais leves.

---

## Opção 2 – PostgreSQL

Banco de dados relacional open source com excelente desempenho e conformidade com padrões SQL.

### Vantagens

- Gratuito.
- Alto desempenho.
- Excelente suporte à linguagem SQL.
- Grande comunidade.

### Desvantagens

- Equipe possui menor experiência com administração da plataforma.
- Menor integração com ferramentas utilizadas internamente.

---

## Opção 3 – Banco NoSQL (MongoDB)

Banco orientado a documentos.

### Vantagens

- Flexibilidade no esquema.
- Escalabilidade horizontal.

### Desvantagens

- Não atende naturalmente ao modelo altamente relacional da aplicação.
- Maior complexidade para garantir consistência entre documentos.
- Consultas relacionais tornam-se mais complexas.

---

# Decisão

Será adotado o Microsoft SQL Server como banco de dados principal da aplicação.

O acesso aos dados será realizado através do Entity Framework Core, utilizando o SQL Server como provedor de persistência.

---

# Justificativa

A decisão foi baseada principalmente nas características do domínio da aplicação.

O sistema possui entidades altamente relacionadas, como clientes, veículos, ordens de serviço, funcionários, peças e movimentações de estoque. Esses relacionamentos exigem integridade referencial, consistência transacional e validações realizadas diretamente pelo banco de dados.

O SQL Server oferece suporte completo às propriedades ACID, permitindo que operações críticas sejam executadas de forma segura e consistente, reduzindo o risco de inconsistências nos dados.

Além disso, a equipe possui experiência prévia com SQL Server e com o ecossistema .NET, reduzindo a curva de aprendizado, aumentando a produtividade e simplificando a manutenção da solução.

A integração nativa com Entity Framework Core também reduz o esforço de desenvolvimento e facilita a utilização de migrations, consultas LINQ e gerenciamento do modelo de dados.

Embora existam alternativas open source igualmente maduras, como PostgreSQL, o ganho operacional obtido pela familiaridade da equipe e pela integração com as ferramentas já utilizadas justificou a escolha do SQL Server.

---

# Consequências

## Positivas

- Forte consistência dos dados.
- Integridade referencial garantida pelo banco.
- Suporte completo a transações.
- Facilidade para criação de consultas complexas.
- Excelente integração com o ecossistema .NET.
- Facilidade de manutenção do modelo de dados.

## Negativas

- Dependência da tecnologia SQL Server.
- Possível custo de licenciamento em ambientes produtivos.
- Necessidade de administração especializada do banco em ambientes de maior porte.

---

# Trade-offs

A escolha prioriza consistência, confiabilidade e produtividade da equipe em detrimento da flexibilidade de esquema oferecida por bancos NoSQL.

Também implica aceitar eventual custo de licenciamento em troca de uma plataforma consolidada e amplamente utilizada em aplicações corporativas.

---

# Impacto Arquitetural

A decisão impacta diretamente:

- Camada de Infrastructure.
- Configuração do Entity Framework Core.
- Estratégia de migrations.
- Processo de backup e recuperação.
- Pipeline de implantação.
- Ambiente Docker utilizado durante o desenvolvimento.

---

# Critérios de Aceitação

A decisão será considerada atendida quando:

- [ ] O banco suportar transações ACID.
- [ ] O Entity Framework Core estiver configurado para SQL Server.
- [ ] As migrations forem utilizadas para gerenciamento do esquema.
- [ ] A integridade referencial for implementada através de chaves estrangeiras.
- [ ] O ambiente Docker possuir uma instância SQL Server para desenvolvimento.

---

# Riscos

| Risco | Mitigação |
|--------|-----------|
| Crescimento do banco | Estratégias de indexação, particionamento e monitoramento. |
| Custos de licenciamento | Avaliar edições adequadas e alternativas futuras, caso necessário. |
| Dependência tecnológica | Utilizar abstrações de acesso aos dados para reduzir acoplamento à tecnologia específica. |

---

# Referências

- Microsoft SQL Server Documentation
- Entity Framework Core Documentation
- Domain-Driven Design: Tackling Complexity in the Heart of Software — Eric Evans
- Clean Architecture — Robert C. Martin

---

# Histórico

| Data | Alteração | Responsável |
|------|-----------|-------------|
| 2026-07-03 | Documento criado | Equipe de Arquitetura |