# Configuração Docker para SQL Server - Ofichinna

## 📋 Visão Geral

Esta documentação descreve como configurar e executar uma instância de SQL Server usando Docker para o projeto Ofichinna.

---

## 🏗️ Estrutura de Arquivos

```
Ofichinna/
├── docker/
│   ├── Dockerfile                # Imagem Docker do SQL Server
│   ├── docker-compose.yml        # Orquestração de containers
│   ├── DOCKER_SQLSERVER.md       # Documentação detalhada (este arquivo)
│   ├── QUICKSTART.md             # Guia rápido de inicialização
│   ├── LOGGING.md                # Documentação de logging e Seq
│   └── EXEMPLOS_CORRELATION_ID.md # Exemplos de uso do correlation id
└── src/Ofichina.Api/
	├── appsettings.json              # String de conexão para produção
	└── appsettings.Development.json  # String de conexão para desenvolvimento
```

---

## 🚀 Como Usar

### Opção 1: Usando Docker Compose (RECOMENDADO)

#### Pré-requisitos
- Docker instalado e em execução
- Docker Compose instalado

#### Serviços disponíveis
- `sqlserver`: banco de dados SQL Server na porta `1433`
- `seq`: visualização de logs na porta `5341`
- `sonarqube`: análise de qualidade de código na porta `9000`

#### Iniciar o SQL Server
```bash
docker-compose up -d
```

#### Iniciar apenas o Seq
```bash
docker-compose up -d seq
```

#### Verificar status
```bash
docker-compose ps
```

#### Parar o SQL Server
```bash
docker-compose down
```

#### Parar e remover volumes (dados)
```bash
docker-compose down -v
```

---

### Opção 2: Usando Apenas Docker

#### Construir a imagem
```bash
docker build -f docker/Dockerfile -t ofichinna-sqlserver .
```

#### Executar o container
```bash
docker run -d `
  --name ofichinna-sqlserver `
  -e ACCEPT_EULA=Y `
  -e MSSQL_SA_PASSWORD=P@ssw0rd2024!Ofichina `
  -p 1433:1433 `
  ofichinna-sqlserver
```

#### Parar o container
```bash
docker stop ofichinna-sqlserver
docker rm ofichinna-sqlserver
```

---

## 🔑 Credenciais de Acesso

| Campo | Valor |
|-------|-------|
| **Server** | `localhost:1433` ou `sqlserver:1433` (docker compose) |
| **Username** | `sa` |
| **Password** | `P@ssw0rd2024!Ofichina` |
| **Database** | `ofichinna` |

### ⚠️ Importante
- **NUNCA** use as credenciais padrão em produção
- Altere a senha em variáveis de ambiente seguras
- Use `.env` para sobrescrever valores em produção

---

## 📝 Strings de Conexão

### Desenvolvimento Local (localhost)
```
Server=localhost,1433;Database=ofichinna;User Id=sa;Password=P@ssw0rd2024!Ofichina;TrustServerCertificate=True;
```

### Com Docker Compose (interno)
```
Server=sqlserver,1433;Database=ofichinna;User Id=sa;Password=P@ssw0rd2024!Ofichina;TrustServerCertificate=True;
```

---

## 🛠️ Configuração no Projeto

### appsettings.json (Produção)
```json
{
  "ConnectionStrings": {
	"DefaultConnection": "Server=localhost,1433;Database=ofichinna;User Id=sa;Password=P@ssw0rd2024!Ofichina;TrustServerCertificate=True;"
  }
}
```

### appsettings.Development.json (Desenvolvimento)
```json
{
  "ConnectionStrings": {
	"DefaultConnection": "Server=sqlserver,1433;Database=ofichinna;User Id=sa;Password=P@ssw0rd2024!Ofichina;TrustServerCertificate=True;"
  }
}
```

---

## 🔍 Conectando ao Banco via SQL Server Management Studio (SSMS)

1. Abra **SQL Server Management Studio**
2. Na tela de conexão, insira:
   - **Server name**: `localhost,1433`
   - **Authentication**: SQL Server Authentication
   - **Login**: `sa`
   - **Password**: `P@ssw0rd2024!Ofichina`
3. Clique em **Connect**

---

## 🔍 Conectando via SQL Server Azure Data Studio

1. Abra **Azure Data Studio**
2. Clique em **Create a connection**
3. Preencha com:
   - **Server**: `localhost,1433`
   - **Database**: `ofichinna`
   - **Authentication type**: SQL Login
   - **User name**: `sa`
   - **Password**: `P@ssw0rd2024!Ofichina`
4. Clique em **Connect**

---

## 📊 Volumes Docker

O Docker Compose cria 2 volumes para persistência de dados:

| Volume | Função |
|--------|--------|
| `sqlserver_data` | Armazena os arquivos de banco de dados |
| `seq_data` | Armazena os eventos do Seq |
| `postgres_data` | Armazena dados do PostgreSQL do SonarQube |
| `sonarqube_data` | Armazena os dados do SonarQube |
| `sonarqube_extensions` | Armazena as extensões do SonarQube |
| `sonarqube_logs` | Armazena os logs do SonarQube |

---

## ⚙️ Variáveis de Ambiente

| Variável | Valor Padrão | Descrição |
|----------|-------------|-----------|
| `ACCEPT_EULA` | `Y` | Aceita os termos de licença do SQL Server |
| `MSSQL_SA_PASSWORD` | `P@ssw0rd2024!Ofichina` | Senha do administrador (sa) |
| `MSSQL_PID` | `Developer` | Versão: Developer (gratuita) |
| `MSSQL_COLLATION` | `SQL_Latin1_General_CP1_CI_AS` | Agrupamento de caracteres |

---

## 🐛 Troubleshooting

### Acessar o Seq
```bash
docker-compose logs -f seq
```

Abra no navegador:
```text
http://localhost:5341
```

Se o Seq não carregar, verifique se a porta `5341` está livre e se o serviço foi iniciado com `docker-compose up -d seq`.

### Container não inicia
```bash
# Visualizar logs
docker-compose -f docker/docker-compose.yml logs sqlserver

# Ou com Docker direto
docker logs ofichinna-sqlserver
```

### Erro de conexão recusada
- Verifique se o container está em execução: `docker-compose ps`
- Aguarde 40-60 segundos para o SQL Server iniciar completamente
- Valide as credenciais e porta

### Porta 1433 já está em uso
```bash
# Alternar a porta no docker/docker-compose.yml
ports:
  - "1434:1433"  # Usar 1434 no local, 1433 no container
```

### Redefinir o banco de dados
```bash
# Remover container e volumes
docker-compose -f docker/docker-compose.yml down -v

# Recriar
docker-compose -f docker/docker-compose.yml up -d
```

---

## 📚 Arquivos Criados/Modificados

- ✅ **docker/Dockerfile** - Imagem base SQL Server
- ✅ **docker/docker-compose.yml** - Orquestração completa
- ✅ **docker/DOCKER_SQLSERVER.md** - Documentação completa (este arquivo)
- ✅ **docker/QUICKSTART.md** - Guia rápido
- ✅ **docs/LOGGING.md** - Configuração de logging, correlation id e Seq
- ✅ **docs/EXEMPLOS_CORRELATION_ID.md** - Exemplos práticos de uso
- ✅ **src/Ofichina.Api/appsettings.json** - String de conexão
- ✅ **src/Ofichina.Api/appsettings.Development.json** - String de conexão

---

## 🎯 Próximos Passos

1. **Iniciar o SQL Server**: `docker-compose up -d`
2. **Executar migrations**: `dotnet ef database update`
3. **Verificar conexão**: Usar SSMS ou Azure Data Studio
4. **Iniciar a aplicação**: `dotnet run` ou via Visual Studio

---

## 📖 Referências

- [SQL Server Docker Image](https://hub.docker.com/_/microsoft-mssql-server)
- [Docker Compose Documentation](https://docs.docker.com/compose/)
- [Entity Framework Core SQL Server](https://learn.microsoft.com/en-us/ef/core/providers/sql-server/)

---

## 📝 Notas Importantes

- A instância foi configurada com `TrustServerCertificate=True` para facilitar testes locais
- Em produção, use certificados SSL apropriados
- As credenciais padrão devem ser alteradas antes de usar em ambientes de produção
- O healthcheck verifica a disponibilidade do SQL Server a cada 15 segundos
