# 🐳 Docker SQL Server - Guia Rápido

## ⚡ Iniciar em 30 segundos

### Windows (PowerShell)
```bash
# Iniciar
docker-compose up -d

# Verificar status
docker-compose ps

# Ver logs
docker-compose logs -f sqlserver
```

---

## 🔑 Credenciais

- **Server**: `localhost:1433`
- **Usuario**: `sa`
- **Senha**: `@Ofichinna2024`
- **Database**: `ofichina`

---

## 📚 Documentação Completa

Veja **[DOCKER_SQLSERVER.md](./DOCKER_SQLSERVER.md)** para:
- Instruções detalhadas
- Troubleshooting
- Configuração avançada
- Conexão com ferramentas GUI

---

## 🎯 Próximos Passos

1. ✅ Iniciar SQL Server: `docker-compose up -d`
2. ✅ Verificar banco de dados conecta
3. ✅ Executar migrations: `dotnet ef database update`
4. ✅ Iniciar aplicação: `dotnet run`

---

## 📖 Documentação Relacionada

- [DOCKER_SQLSERVER.md](./DOCKER_SQLSERVER.md) - Guia completo
- [.env.example](./.env.example) - Variáveis de ambiente
- [docker-compose.yml](./docker-compose.yml) - Configuração de containers

---

## ⚠️ Nota Importante

As credenciais padrão são apenas para **desenvolvimento**. Para produção:
1. Altere `SA_PASSWORD` em `docker-compose.yml`
2. Use variáveis de ambiente seguras
3. Configure certificados SSL
4. Use diferentes credenciais por ambiente
