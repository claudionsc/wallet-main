# Wallet API

Este projeto é uma API desenvolvida em .NET 8.0 que simula um sistema de carteira digital, permitindo o gerenciamento de clientes, saldos e histórico de transações. A API utiliza Entity Framework Core para persistência de dados com PostgreSQL e autenticação via JWT.

## Tecnologias Utilizadas

- **.NET 8.0 SDK**: Framework para desenvolvimento da aplicação.
- **Entity Framework Core**: ORM para interação com o banco de dados.
- **PostgreSQL**: Banco de dados relacional.
- **JWT (JSON Web Tokens)**: Para autenticação e autorização.
- **DotNetEnv**: Para carregamento de variáveis de ambiente.
- **Swagger/OpenAPI**: Para documentação e teste interativo da API.

## Pré-requisitos

Antes de começar, certifique-se de ter os seguintes softwares instalados em sua máquina:

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Um ambiente preparado para executar o Docker  [Sobre o Docker Desktop](https://docs.docker.com/desktop/)

## Configuração do Ambiente

1.  **Clone o repositório** (se aplicável, caso contrário, descompacte o arquivo `Wallet.zip`):

    ```bash
    git clone <URL_DO_REPOSITORIO>
    cd Wallet/Wallet
    ```

2.  **Configurar Variáveis de Ambiente**:

    Crie um arquivo `.env` na raiz do diretório `Wallet/Wallet` (onde está o arquivo `Wallet.csproj`) com as seguintes variáveis:

    ```
    JWT_SECRET_KEY="SuaChaveSecretaMuitoSeguraEComPeloMenos32Caracteres"
    JWT_ISSUER="SeuEmissor"
    JWT_AUDIENCE="SuaAudiencia"
    CONNECTION_STRING="Host=localhost;Port=5432;Database=DigitalWallet;Username=walletuser;Password=walletpass"
    ```

    - `JWT_SECRET_KEY`: Uma chave secreta forte para assinar os tokens JWT. **Mínimo de 32 caracteres é recomendado.**
    - `JWT_ISSUER`: O emissor do token JWT (ex: `yourdomain.com`).
    - `JWT_AUDIENCE`: A audiência do token JWT (ex: `your_app_name`).
    - `CONNECTION_STRING`: A string de conexão para o seu banco de dados PostgreSQL. 

    *Obs: O arquivo docker-compose contém os dados da string para conexão com a sua imagem do Postgres. Caso deseje utilizar uma imagem á existente, lembre-se de alterar no arquivo o Host, Port, Database, Username e Password*


## Executando a Aplicação

1.  **Abra o sistema com o seu editor de texto de preferência**  

2.  **Restaurar dependências**:

    ```bash
    dotnet restore
    ```

3.  **Aplicar Migrações do Banco de Dados**:

    Se esta for a primeira vez que você está configurando o projeto ou se houver novas migrações, execute:

    ```bash
    dotnet ef database update
    ```

    *Nota: Se você precisar criar uma nova migração, use `dotnet ef migrations add NomeDaSuaMigracao`.*

        
4.  **Execução do Docker**:

    Na raíz do projeto, execute: 

    ```bash
    docker-compose up -d
    ```

5.  **Executar a Aplicação**:

    ```bash
    dotnet run
    ```

    A API será iniciada e estará disponível em `http://localhost:5142` (ou `https://localhost:7004` se configurado para HTTPS).

## Testando a API com Swagger

Após iniciar a aplicação, você pode acessar a interface do Swagger para testar os endpoints da API:

Abra seu navegador e navegue para:

`http://localhost:5142/swagger`

No Swagger UI, você poderá:

-   Visualizar todos os endpoints disponíveis.
-   Autenticar-se usando o botão "Authorize" (fornecendo o token JWT no formato `Bearer SEU_TOKEN`).
-   Fazer requisições aos endpoints e ver as respostas.

## Estrutura do Projeto

-   `Controllers/`: Contém os controladores da API que lidam com as requisições HTTP.
-   `Data/`: Contém o `AppDbContext` para configuração do Entity Framework Core e modelos de dados.
-   `Interfaces/`: Define as interfaces para os serviços.
-   `Model/`: Contém os modelos de dados (entidades) do banco de dados.
-   `Services/`: Contém a lógica de negócio da aplicação (ex: `BalanceService`, `ClientsService`, `JwtService`).
-   `DTOs/`: Contém os Data Transfer Objects usados para comunicação entre a API e os clientes.
-   `Middleware/`: Contém middlewares personalizados (ex: `AppSeeder` para dados iniciais).
-   `Utils/`: Contém classes utilitárias (ex: `HashHelper`).
-   `Program.cs`: Ponto de entrada da aplicação, onde a injeção de dependências, configuração de middlewares e rotas são definidas.
-   `Wallet.csproj`: Arquivo de projeto .NET que lista as dependências e configurações do projeto.
-   `Wallet.sln`: Arquivo de solução do Visual Studio.

---

Este README fornece as informações essenciais para configurar, executar e testar a API Wallet. Para mais detalhes sobre a implementação, consulte o código-fonte e a documentação gerada pelo Swagger, ou entre em contato através do email: claudio.nsc@hotmail.com

