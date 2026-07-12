# Exemplos de Uso - Correlation ID e Logging

Este documento apresenta exemplos práticos de como utilizar o sistema de Correlation ID e visualizar logs no Ofichinna.

---

## 🎯 Pré-requisitos

1. Aplicação rodando localmente ou em ambiente de desenvolvimento
2. Seq rodando via Docker (opcional, mas recomendado)

```bash
# Iniciar todos os serviços incluindo Seq
cd docker
docker-compose up -d

# Ou apenas o Seq
docker-compose up -d seq
```

3. Token JWT válido (para endpoints autenticados)

---

## 📋 Exemplo 1: Requisição COM Correlation ID

### Requisição

```bash
curl -X GET \
  "https://localhost:7000/api/cliente/550e8400-e29b-41d4-a716-446655440000/perfil" \
  -H "X-Correlation-Id: minha-requisicao-123" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

### Resposta

```json
{
  "success": true,
  "data": ["Administrador", "Usuario"],
  "message": null,
  "errors": []
}
```

**Headers de resposta:**
```
X-Correlation-Id: minha-requisicao-123
Content-Type: application/json; charset=utf-8
```

### Logs gerados

#### Console / Arquivo

```
2026-01-15 14:35:42.123 +00:00 [DBG] [minha-requisicao-123] Requisição iniciada com CorrelationId: minha-requisicao-123
2026-01-15 14:35:42.456 +00:00 [INF] [minha-requisicao-123] Consultando perfis do cliente. ClienteId: "550e8400-e29b-41d4-a716-446655440000"
2026-01-15 14:35:42.789 +00:00 [INF] [minha-requisicao-123] Perfis obtidos com sucesso. ClienteId: "550e8400-e29b-41d4-a716-446655440000", Quantidade: 2
2026-01-15 14:35:42.987 +00:00 [DBG] [minha-requisicao-123] Requisição finalizada com CorrelationId: minha-requisicao-123
```

#### Seq

1. Acesse: http://localhost:5341
2. Na barra de busca, digite:
   ```
   CorrelationId = 'minha-requisicao-123'
   ```
3. Visualize todos os logs relacionados agrupados

---

## 📋 Exemplo 2: Requisição SEM Correlation ID (geração automática)

### Requisição

```bash
curl -X POST \
  "https://localhost:7000/api/cliente/550e8400-e29b-41d4-a716-446655440000/perfil/6ba7b810-9dad-11d1-80b4-00c04fd430c8" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..." \
  -H "Content-Type: application/json"
```

### Resposta

```json
{
  "success": true,
  "message": "Perfil vinculado com sucesso.",
  "errors": []
}
```

**Headers de resposta:**
```
X-Correlation-Id: a3c5e7f9-1234-5678-90ab-cdef12345678
Content-Type: application/json; charset=utf-8
```

> ⚠️ **Nota**: O sistema gerou automaticamente um GUID como Correlation ID

### Logs gerados

```
2026-01-15 14:40:12.123 +00:00 [DBG] [a3c5e7f9-1234-5678-90ab-cdef12345678] Requisição iniciada com CorrelationId: a3c5e7f9-1234-5678-90ab-cdef12345678
2026-01-15 14:40:12.456 +00:00 [INF] [a3c5e7f9-1234-5678-90ab-cdef12345678] Iniciando vinculação de perfil. ClienteId: "550e8400-e29b-41d4-a716-446655440000", PerfilId: "6ba7b810-9dad-11d1-80b4-00c04fd430c8"
2026-01-15 14:40:12.789 +00:00 [INF] [a3c5e7f9-1234-5678-90ab-cdef12345678] Perfil vinculado com sucesso. ClienteId: "550e8400-e29b-41d4-a716-446655440000", PerfilId: "6ba7b810-9dad-11d1-80b4-00c04fd430c8"
2026-01-15 14:40:12.987 +00:00 [DBG] [a3c5e7f9-1234-5678-90ab-cdef12345678] Requisição finalizada com CorrelationId: a3c5e7f9-1234-5678-90ab-cdef12345678
```

---

## 📋 Exemplo 3: Rastreando erro com Correlation ID

### Requisição com erro de validação

```bash
curl -X POST \
  "https://localhost:7000/api/cliente/00000000-0000-0000-0000-000000000000/perfil/00000000-0000-0000-0000-000000000000" \
  -H "X-Correlation-Id: erro-validacao-001" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..." \
  -H "Content-Type: application/json"
```

### Resposta

```json
{
  "success": false,
  "message": null,
  "errors": [
	"UsuarioId deve ser um GUID válido",
	"PerfilId deve ser um GUID válido"
  ]
}
```

**Headers de resposta:**
```
X-Correlation-Id: erro-validacao-001
```

### Logs gerados

```
2026-01-15 15:10:12.123 +00:00 [DBG] [erro-validacao-001] Requisição iniciada com CorrelationId: erro-validacao-001
2026-01-15 15:10:12.456 +00:00 [INF] [erro-validacao-001] Iniciando vinculação de perfil. ClienteId: "00000000-0000-0000-0000-000000000000", PerfilId: "00000000-0000-0000-0000-000000000000"
2026-01-15 15:10:12.789 +00:00 [WRN] [erro-validacao-001] Validação falhou ao vincular perfil. ClienteId: "00000000-0000-0000-0000-000000000000", PerfilId: "00000000-0000-0000-0000-000000000000", Erros: "UsuarioId deve ser um GUID válido, PerfilId deve ser um GUID válido"
2026-01-15 15:10:12.987 +00:00 [DBG] [erro-validacao-001] Requisição finalizada com CorrelationId: erro-validacao-001
```

---

## 🔍 Buscas no Seq

### 1. Buscar por Correlation ID específico

**Query:**
```
CorrelationId = 'minha-requisicao-123'
```

**Resultado:**
- Todos os logs daquela requisição específica
- Timeline completa do processamento
- Estrutura de dados completa

---

### 2. Buscar erros de um cliente específico

**Query:**
```
Level = 'Error' and ClienteId = '550e8400-e29b-41d4-a716-446655440000'
```

**Resultado:**
- Todos os erros relacionados ao cliente
- Correlation IDs das requisições com problema
- Permite rastrear o fluxo completo

---

### 3. Buscar por intervalo de tempo

**Query:**
```
@Timestamp >= DateTime('2026-01-15T14:00:00') and @Timestamp <= DateTime('2026-01-15T15:00:00')
```

**Resultado:**
- Logs de todas as requisições entre 14h e 15h

---

### 4. Buscar por tipo de operação

**Query:**
```
@Message like '%vinculação%'
```

**Resultado:**
- Todas as operações de vinculação
- Sucessos e falhas

---

### 5. Buscar warnings ou erros

**Query:**
```
Level in ['Warning', 'Error']
```

**Resultado:**
- Todos os logs de Warning ou Error
- Útil para análise de problemas

---

### 6. Combinar múltiplos critérios

**Query:**
```
CorrelationId = 'minha-requisicao-123' and Level in ['Warning', 'Error']
```

**Resultado:**
- Warnings e erros de uma requisição específica

---

## 📁 Consultando logs em arquivo

### Localização dos arquivos

```bash
cd src/Ofichina.Api/logs
ls -la
```

**Saída esperada:**
```
ofichinna-2026-01-15_10.txt
ofichinna-2026-01-15_11.txt
ofichinna-2026-01-15_12.txt
ofichinna-2026-01-15_13.txt
ofichinna-2026-01-15_14.txt
```

### Buscar por Correlation ID em arquivo

#### Windows (PowerShell)

```powershell
# Buscar em um arquivo específico
Select-String -Path "logs\ofichinna-2026-01-15_14.txt" -Pattern "minha-requisicao-123"

# Buscar em todos os arquivos do dia
Select-String -Path "logs\ofichinna-2026-01-15*.txt" -Pattern "minha-requisicao-123"

# Ver contexto (linhas antes/depois)
Select-String -Path "logs\ofichinna-2026-01-15_14.txt" -Pattern "minha-requisicao-123" -Context 2,2
```

#### Linux/Mac (bash)

```bash
# Buscar em um arquivo específico
grep "minha-requisicao-123" logs/ofichinna-2026-01-15_14.txt

# Buscar em todos os arquivos do dia
grep "minha-requisicao-123" logs/ofichinna-2026-01-15*.txt

# Ver contexto (2 linhas antes e depois)
grep -C 2 "minha-requisicao-123" logs/ofichinna-2026-01-15_14.txt
```

---

## 🧪 Testando o sistema completo

### Script de teste (PowerShell)

```powershell
# 1. Definir variáveis
$baseUrl = "https://localhost:7000"
$token = "seu_token_jwt_aqui"
$correlationId = "teste-completo-$(Get-Date -Format 'yyyyMMdd-HHmmss')"
$clienteId = "550e8400-e29b-41d4-a716-446655440000"

# 2. Fazer requisição
$response = Invoke-WebRequest `
	-Uri "$baseUrl/api/cliente/$clienteId/perfil" `
	-Method GET `
	-Headers @{
		"X-Correlation-Id" = $correlationId
		"Authorization" = "Bearer $token"
	}

# 3. Exibir resultado
Write-Host "Status: $($response.StatusCode)"
Write-Host "Correlation ID retornado: $($response.Headers['X-Correlation-Id'])"
Write-Host "Body: $($response.Content)"

# 4. Buscar logs no arquivo
Write-Host "`nBuscando logs no arquivo..."
$currentHour = Get-Date -Format "yyyy-MM-dd_HH"
Select-String -Path "src\Ofichina.Api\logs\ofichinna-$currentHour.txt" -Pattern $correlationId

# 5. Exibir link do Seq
Write-Host "`nVisualize os logs no Seq:"
Write-Host "http://localhost:5341/#/events?filter=CorrelationId%20%3D%20'$correlationId'"
```

### Script de teste (Bash)

```bash
#!/bin/bash

# 1. Definir variáveis
BASE_URL="https://localhost:7000"
TOKEN="seu_token_jwt_aqui"
CORRELATION_ID="teste-completo-$(date +%Y%m%d-%H%M%S)"
CLIENTE_ID="550e8400-e29b-41d4-a716-446655440000"

# 2. Fazer requisição
echo "Fazendo requisição com Correlation ID: $CORRELATION_ID"
curl -X GET \
  "$BASE_URL/api/cliente/$CLIENTE_ID/perfil" \
  -H "X-Correlation-Id: $CORRELATION_ID" \
  -H "Authorization: Bearer $TOKEN" \
  -i

# 3. Buscar logs no arquivo
echo -e "\nBuscando logs no arquivo..."
CURRENT_HOUR=$(date +%Y-%m-%d_%H)
grep "$CORRELATION_ID" "src/Ofichina.Api/logs/ofichinna-$CURRENT_HOUR.txt"

# 4. Exibir link do Seq
echo -e "\nVisualize os logs no Seq:"
echo "http://localhost:5341/#/events?filter=CorrelationId%20%3D%20'$CORRELATION_ID'"
```

---

## 💡 Dicas e Boas Práticas

### 1. Padrão de Correlation ID

Recomenda-se usar um padrão consistente para facilitar buscas:

```
# Formato sugerido
{ambiente}-{origem}-{timestamp}-{sequencial}

# Exemplos
prod-web-20260115-001
dev-mobile-20260115-002
test-postman-20260115-003
```

### 2. Preservar Correlation ID em chamadas externas

Se sua API chamar outros serviços, propague o Correlation ID:

```csharp
public async Task<HttpResponseMessage> ChamarServicoExternoAsync(HttpContext context)
{
	var correlationId = context.Items["X-Correlation-Id"]?.ToString();

	var client = new HttpClient();
	if (!string.IsNullOrEmpty(correlationId))
	{
		client.DefaultRequestHeaders.Add("X-Correlation-Id", correlationId);
	}

	return await client.GetAsync("https://api-externa.com/endpoint");
}
```

### 3. Logs estruturados

Sempre use logs estruturados para facilitar buscas:

```csharp
// ✅ Correto
_logger.LogInformation("Processando pedido. ClienteId: {ClienteId}, PedidoId: {PedidoId}", 
	clienteId, pedidoId);

// ❌ Incorreto
_logger.LogInformation($"Processando pedido do cliente {clienteId}");
```

### 4. Monitoramento em produção

Configure alertas no Seq para erros críticos:

1. Acesse Seq → Settings → Alerts
2. Crie um alerta para: `Level = 'Error'`
3. Configure notificações (email, Slack, etc.)

---

## 📚 Referências

- [Documentação completa de Logging](./LOGGING.md)
- [Configuração do Docker Compose](../docker/docker-compose.yml)
- [Código do Middleware](../src/Ofichina.Api/Middleware/CorrelationIdMiddleware.cs)
