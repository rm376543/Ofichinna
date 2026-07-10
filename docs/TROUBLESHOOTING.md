## Banco de Dados
- Dificuldade em conectar ao banco de dados
  - Verifique se o serviço do banco de dados está em execução.
  - Confirme se as credenciais (usuário e senha) estão corretas.
  - Certifique-se de que o host e a porta do banco de dados estão corretos.

- Erro ao gerar migrations
	- Se quiser limpar e gera a migration, do zero, va no SQL
	- Abra a base de dados
	- Procure pela tabela __EFMigrationsHistory - Apague-a
	- Volte ao projeto
	- Abra a pasta "Migrations" dentro do projeto "Infrastructure"
	- Apague todas as migrations
	- Abra agora a pasta Persistence dentro do projeto "Infrastructure"
	- Procure a pasta Migrations e apague todas as migrations e principalmente se houver, apague o snapshot

	Gere uma nova migration apontando para o projeto "Infrastructure"
	- dotnet ef migrations add <nome_migration> -p src\Ofichina.Infrastructure

	Atualize a base de dados com a migration gerada
	- dotnet ef database update --project src/Ofichina.Infrastructure

	Observe se voce esta na raiz do projeto - Ofichinna, caso contrario, navegue até a raiz do projeto e execute os comandos acima.

## SonarQube
### Etapas para configuração do projeto c# com sonarqube via docker
 - 1 Acessar o HUB docker - https://hub.docker.com/_/sonarqube?tag=community
 - 2 Pesquise do lado direito no menu dropdown, se estiver escrito "latest" mude para "comunity"
 - 3 Logo embaixo deve aparecer o comando pull do docker > docker pull sonarqube:community
 - 4 Abra o powershell e execute o comando > docker pull sonarqube:community
 - 5 Agora abra o docker, procure no menu as images e inicie o docker
 - 6 Na tela que aparecera com opções adicionais so reforce o uso da porta, aqui estou utilizando 9000
 - 7 Abra o navegador e navegue para seu localhost - http://localhost:9000
 - 8 No login / senha digite > admin/admin
 - 9 Vai pedir para trocar senha
 - 10 Apos fazer login, voce precisa criar a configuração local do projeto
 - 11 Clique na opção "Create a Local Project"
 - 12 Informe aqui o nome do projeto (Project Display Name) - Tente deixar o mesmo nome do projeto c#
 - 13 Defina o nome da sua branch principal (no GitHub), clique em salvar 
 - 14 Na tela agora, procure pelo botão "Locally"
 - 15 Em "Provide a token" selecione Generate a project token
 - 16 Token name (eu coloquei o nome do projeto)
 - 17 "Expires in" deixei a opção que não expira ( eh local ), clique em generate
 - 18 Selecione o tipo de projeto (C#)
 - 19 Abra o powershell novamente, navegue na raiz do seu projeto onde tem o arquivo .sln
 - 20 rode os comandos abaixo na sequencia, um por um.:
	- dotnet tool install --global dotnet-sonarscanner (vai instalar a ferramenta no seu projeto)
	- dotnet tool install --global dotnet-coverage (vai criar o arquivo de cobertura de testes, utilizado na tela de overview do sonarqube)
	## Esse comando abaixo sera mostrado na tela do sonarqube ja preenchido, apenas copie-o e cole no powershell
	- dotnet sonarscanner begin /k:"Ofichinna" /d:sonar.host.url="http://localhost:9000" /d:sonar.token="" <-- Comando ja esta pronto so substituir o token gerado no passo 17, entre as aspas do final do comando.

 - 21 acesse o site oficial da documentacao do sonarqube
	- https://docs.sonarsource.com/sonarqube-server/analyzing-source-code/test-coverage/dotnet-test-coverage

	Ou atraves do site:
	- no menu superior, procure por sonarqube server
	- no menu lateral, procure por analyzing source core
	- depois procure por test coverage
	- depois .net test coverage

	Role a pagina para visualizar os comandos
	Execute na ordem
	- dotnet build --no-incremental
	- dotnet-coverage collect "dotnet test" -f xml -o "coverage.xml"
	- dotnet sonarscanner end /d:sonar.token=""

	### Caso sonarqube nao funcione
	- Verifique a aba extensoes
	- SonarQube
	- Connected Mode: Manage Binding
		- Clique em manage binding
		- Se houver alguma conexao, remova-a clicando no icone da lixeira
		- Clique em New Connection
		- Selecione SonarQube Server, informe o endereco do servidor (http://localhost:9000) e de Ok
		- Ao voltar na tela inicial, na caixa de selecao em Connection to Bind, selecione o servidor
		- Na caixa Project to Bind, clique em "Select a project"
		- Voce precisa voltar no painel do SonarQube e agora gera um token de usuario, que fara a conexao entre o Visual Studio e o SonarQube, 
		- Para isso, 
			- clique no seu usuario (canto superior direito) 
				> My Account
				> Security 
				> Generate Tokens 
				> Informe um nome para o token e clique em Generate > Copie o token gerado e cole no campo "Token" do Visual Studio, clique em OK
		- Volte na tela para inserir o token e vincule o projeto.