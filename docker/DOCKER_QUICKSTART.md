# 🐳 Docker - Guia Rápido

## ⚡ Iniciar em 30 segundos

### Windows (PowerShell)
```bash
# Iniciar todos os serviços
docker-compose up -d

# Verificar status
docker-compose ps

# Ver logs de um serviço específico
docker-compose logs -f sqlserver
docker-compose logs -f seq
```

---

## 🔑 Credenciais e Acessos

### SQL Server
- **Server**: `localhost:1433`
- **Usuario**: `sa`
- **Senha**: `P@ssw0rd2024!Ofichina`
- **Database**: `ofichinna`

### Seq (Visualização de Logs)
- **URL**: http://localhost:5341
- **Autenticação**: Não requerida (ambiente desenvolvimento)

### SonarQube (Análise de Código)
- **URL**: http://localhost:9000
- **Usuário**: `admin`
- **Senha**: `admin` (alterar no primeiro acesso)

---

## 📚 Documentação Completa

Veja **[DOCKER_SQLSERVER.md](./DOCKER_SQLSERVER.md)** para:
- Instruções detalhadas
- Troubleshooting
- Configuração avançada
- Conexão com ferramentas GUI

---

## 🎯 Próximos Passos

1. ✅ Iniciar todos os serviços: `docker-compose up -d`
2. ✅ Verificar banco de dados conecta
3. ✅ Executar migrations: `dotnet ef database update`
4. ✅ Acessar Seq: http://localhost:5341
5. ✅ Iniciar aplicação: `dotnet run`

---

## 📖 Documentação Relacionada

- [DOCKER_SQLSERVER.md](./DOCKER_SQLSERVER.md) - Guia completo SQL Server
- [LOGGING.md](../docs/LOGGING.md) - Configuração de logging e Seq
- [EXEMPLOS_CORRELATION_ID.md](../docs/EXEMPLOS_CORRELATION_ID.md) - Exemplos práticos
- [docker-compose.yml](./docker-compose.yml) - Configuração de containers

---

## ⚠️ Nota Importante

As credenciais padrão são apenas para **desenvolvimento**. Para produção:
1. Altere `SA_PASSWORD` em `docker-compose.yml`
2. Use variáveis de ambiente seguras
3. Configure certificados SSL
4. Use diferentes credenciais por ambiente
5. Configure API Key no Seq para autenticação
