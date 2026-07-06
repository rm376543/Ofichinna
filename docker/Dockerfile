# Dockerfile para SQL Server
# Cria uma instância de SQL Server com configurações básicas para desenvolvimento
# Imagem base: SQL Server 2022 (Enterprise Edition - Developer License)

FROM mcr.microsoft.com/mssql/server:2022-latest

# Definir o ambiente como não-interativo
ENV ACCEPT_EULA=Y

# Senha do SA (System Administrator)
# IMPORTANTE: Altere esta senha em produção e em variáveis de ambiente!
ENV MSSQL_SA_PASSWORD=P@ssw0rd2024!Ofichina

# Edição do SQL Server (Developer é gratuita e inclui todos os recursos)
ENV MSSQL_PID=Developer

# Collation (agrupamento de caracteres)
ENV MSSQL_COLLATION=SQL_Latin1_General_CP1_CI_AS

# Configurações de memória (opcional)
ENV MSSQL_MEMORY_LIMIT_MB=2048

# Expor a porta padrão do SQL Server
EXPOSE 1433

# Healthcheck para verificar se o servidor está saudável
HEALTHCHECK --interval=15s --timeout=3s --start-period=40s --retries=3 \
    CMD /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "P@ssw0rd2024!Ofichina" -Q "SELECT 1" || exit 1

# O container iniciará automaticamente o SQL Server
CMD ["/opt/mssql/bin/sqlservr"]
