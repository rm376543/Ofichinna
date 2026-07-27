# Configuração de Logging - Ofichinna

## 📋 Visão Geral

O sistema Ofichinna utiliza **Serilog** como provedor de logging estruturado, com suporte a múltiplos destinos (sinks):
- **Console**: Para visualização em tempo real durante o desenvolvimento
- **Arquivo (.txt)**: Para persistência local com rotação automática
- **Seq**: Para análise centralizada e busca avançada de logs

---

## 🔑 Correlation ID

Todas as requisições são rastreadas através de um **Correlation ID** único:

- **Header de entrada**: `X-Correlation-Id`
- **Header de resposta**: `X-Correlation-Id`
- **Geração automática**: Se não fornecido, o sistema gera um GUID automaticamente
- **Propagação**: O correlation id é automaticamente incluído em todos os logs da requisição

### Exemplo de uso

```bash
# Requisição COM correlation id
curl -H "X-Correlation-Id: abc-123-def" https://api.ofichinna.com/api/perfil

# Requisição SEM correlation id (será gerado automaticamente)
curl https://api.ofichinna.com/api/perfil
```

A resposta sempre incluirá o header `X-Correlation-Id` com o valor usado.

---

## 📁 Arquivo de Log

### Localização

Os logs são salvos na pasta `logs/` na raiz da aplicação.

```
Ofichinna/
├── src/
│   └── Ofichina.Api/
│       ├── bin/
│       ├── logs/                    ← Logs gerados aqui
│       │   ├── ofichinna-2026-01-15_10.txt
│       │   ├── ofichinna-2026-01-15_11.txt
│       │   └── ofichinna-2026-01-15_12.txt
│       └── Program.cs
```

### Padrão de nomenclatura

O sistema utiliza rotação **por hora**, gerando arquivos no formato:

```
ofichinna-YYYY-MM-DD_HH.txt
```

Exemplos:
- `ofichinna-2026-01-15_10.txt` → Logs das 10h às 11h do dia 15/01/2026
- `ofichinna-2026-01-15_11.txt` → Logs das 11h às 12h do dia 15/01/2026

### Formato do log

Cada linha de log segue o template:

```
{Timestamp} [{Level}] [{CorrelationId}] {Message}
{Exception}
```

Exemplo real:

```
2026-01-15 10:35:42.123 +00:00 [INF] [abc-123-def] Iniciando vinculação de perfil. ClienteId: "550e8400-e29b-41d4-a716-446655440000", PerfilId: "6ba7b810-9dad-11d1-80b4-00c04fd430c8"
2026-01-15 10:35:42.456 +00:00 [WRN] [abc-123-def] Validação falhou ao vincular perfil. ClienteId: "550e8400-e29b-41d4-a716-446655440000", PerfilId: "6ba7b810-9dad-11d1-80b4-00c04fd430c8", Erros: "PerfilId é obrigatório"
2026-01-15 10:35:43.789 +00:00 [ERR] [xyz-789-ghi] Falha ao vincular perfil. ClienteId: "550e8400-e29b-41d4-a716-446655440000", PerfilId: "6ba7b810-9dad-11d1-80b4-00c04fd430c8", Erro: "Perfil já vinculado ao cliente"
```

### Rotação automática

- **Intervalo**: A cada hora
- **Retenção**: Não há limite de retenção configurado (gerenciar manualmente ou via política de limpeza)
- **Tamanho**: Sem limite de tamanho por arquivo

---

## 🔍 Seq - Visualização Centralizada

### Acesso local

Durante o desenvolvimento, o Seq está disponível em:

```
http://localhost:5341
```

### Configuração

A URL do Seq é configurada em `appsettings.json`:

```json
{
  "Serilog": {
	"Seq": {
	  "ServerUrl": "http://localhost:5341",
	  "ApiKey": null
	}
  }
}
```

### Buscas no Seq

Exemplos de consultas úteis:

#### Buscar por Correlation ID específico
```
CorrelationId = 'abc-123-def'
```

#### Buscar erros de um cliente específico
```
Level = 'Error' and ClienteId = '550e8400-e29b-41d4-a716-446655440000'
```

#### Buscar logs de uma requisição específica
```
CorrelationId = 'abc-123-def' and @Timestamp >= DateTime('2026-01-15T10:00:00')
```

#### Buscar por tipo de mensagem
```
@Message like '%vinculação%'
```

---

## ⚙️ Configuração

### appsettings.json (Produção)

```json
{
  "Serilog": {
	"MinimumLevel": {
	  "Default": "Information",
	  "Override": {
		"Microsoft.AspNetCore": "Warning",
		"System": "Warning"
	  }
	},
	"Seq": {
	  "ServerUrl": "http://localhost:5341"
	},
	"File": {
	  "Path": "logs/ofichinna-.txt"
	}
  }
}
```

### appsettings.Development.json

```json
{
  "Serilog": {
	"MinimumLevel": {
	  "Default": "Debug",
	  "Override": {
		"Microsoft.AspNetCore": "Information",
		"Microsoft.AspNetCore.Authentication": "Debug",
		"Microsoft.IdentityModel": "Debug",
		"System": "Information"
	  }
	},
	"Seq": {
	  "ServerUrl": "http://localhost:5341"
	},
	"File": {
	  "Path": "logs/ofichinna-.txt"
	}
  }
}
```

---

## 🧪 Testando o Logging

### 1. Iniciar o Seq via Docker

```bash
cd docker
docker-compose up -d seq
```

### 2. Verificar o Seq

Acesse: http://localhost:5341

### 3. Executar a aplicação

```bash
cd src/Ofichina.Api
dotnet run
```

### 4. Fazer uma requisição com Correlation ID

```bash
curl -X GET \
  -H "X-Correlation-Id: teste-123-abc" \
  -H "Authorization: Bearer SEU_TOKEN" \
  https://localhost:7000/api/cliente/550e8400-e29b-41d4-a716-446655440000/perfil
```

### 5. Verificar os logs

#### Console
Verifique a saída do terminal onde a aplicação está rodando.

#### Arquivo
```bash
cat src/Ofichina.Api/logs/ofichinna-2026-01-15_10.txt
```

#### Seq
1. Acesse http://localhost:5341
2. Na barra de busca, digite: `CorrelationId = 'teste-123-abc'`
3. Visualize todos os logs relacionados à requisição

---

## 📊 Níveis de Log

| Nível | Uso | Exemplo |
|-------|-----|---------|
| **Trace** | Informações muito detalhadas (não usado em produção) | Detalhes internos de frameworks |
| **Debug** | Informações de depuração | Valores de variáveis, fluxo de execução |
| **Information** | Fluxo normal da aplicação | "Iniciando vinculação de perfil" |
| **Warning** | Situações anormais que não são erros | "Validação falhou", "Cache expirado" |
| **Error** | Erros recuperáveis | "Falha ao vincular perfil" |
| **Fatal** | Erros críticos que impedem a aplicação | "Falha ao conectar no banco de dados" |

---

## 🛠️ Boas Práticas

### ✅ Fazer

```csharp
// Usar logging estruturado
_logger.LogInformation("Iniciando vinculação. ClienteId: {ClienteId}, PerfilId: {PerfilId}", 
	clienteId, perfilId);

// Incluir contexto relevante
_logger.LogError("Falha ao vincular perfil. ClienteId: {ClienteId}, Erro: {Error}", 
	clienteId, erro);
```

### ❌ Evitar

```csharp
// NÃO usar interpolação de string
_logger.LogInformation($"Iniciando vinculação. ClienteId: {clienteId}");

// NÃO concatenar strings
_logger.LogInformation("Iniciando vinculação. ClienteId: " + clienteId);

// NÃO omitir contexto importante
_logger.LogError("Erro ao vincular perfil");
```

### Correlation ID automático

O correlation id é **automaticamente** incluído em todos os logs. Não é necessário fazer:

```csharp
// ❌ NÃO fazer isso - é automático!
_logger.LogInformation("CorrelationId: {CorrelationId} - Mensagem", correlationId);

// ✅ Apenas logue normalmente
_logger.LogInformation("Mensagem com contexto. Campo: {Campo}", valor);
```

---

## 🔒 Segurança

### Dados sensíveis

**NUNCA** logue informações sensíveis:
- Senhas
- Tokens de autenticação
- Dados de cartão de crédito
- CPF/CNPJ completo (mascare se necessário)

### Exemplo seguro

```csharp
// ❌ Incorreto
_logger.LogInformation("Login: {Email}, Senha: {Senha}", email, senha);

// ✅ Correto
_logger.LogInformation("Tentativa de login. Email: {Email}", email);
```

---

## 📚 Arquivos Relacionados

- `src/Ofichina.Api/Program.cs` - Configuração do Serilog
- `src/Ofichina.Api/Middleware/CorrelationIdMiddleware.cs` - Middleware de Correlation ID
- `src/Ofichina.Api/Modules/CorrelationIdModule.cs` - Módulo de registro
- `src/Ofichina.Api/appsettings.json` - Configuração de produção
- `src/Ofichina.Api/appsettings.Development.json` - Configuração de desenvolvimento
- `docker/docker-compose.yml` - Serviço Seq

## ✅ Configuração efetiva da API

`Program.cs` registra Serilog no console, no Seq e em `logs/ofichinna-.txt`, com rotação horária e `[CorrelationId]` no template. `CorrelationIdModule` registra o `CorrelationIdMiddleware`; `ApiExceptionMiddleware` trata exceções não tratadas e padroniza a resposta HTTP.

**Última atualização:** 2026  
**Versão:** 2.0  
**Status:** ✅ Logging e correlação sincronizados com a API
