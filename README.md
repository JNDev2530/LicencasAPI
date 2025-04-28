# API de Controle de Licenças

Esta API permite o controle completo de licenças de software, oferecendo funcionalidades para:

- **Cadastrar Clientes**
- **Cadastrar Produtos**
- **Gerar e Gerenciar Chaves de Licença**

---

## 🚀 Tecnologias Utilizadas

- **ASP.NET Web API** (versão 6.0)
- **.NET SDK 6.0**
- **SQL Server 2019**
- **Swashbuckle.AspNetCore** (Swagger para documentação automática)

---

## 🔧 Pré-requisitos

Antes de iniciar, verifique se você tem instalado em sua máquina:

- [.NET SDK 6.0](https://dotnet.microsoft.com/download)
- [Git](https://git-scm.com/)
- **SQL Server 2019** (instância local ou remota)

---

## 🗄️ Banco de Dados (SQL Server 2019)

Antes de executar a API, crie o banco de dados `ControleLicenca` e todas as tabelas executando o script abaixo diretamente no SQL Server Management Studio (ou similar):

```sql
-- Cria o banco de dados
CREATE DATABASE ControleLicenca;
GO
USE ControleLicenca;
GO

-- Tabela de Produtos
CREATE TABLE produto (
   produtoID INT IDENTITY(1,1) PRIMARY KEY,
   produtoNome VARCHAR(100) NOT NULL,
   produtoStatus BIT NOT NULL
);

-- Tabela de Usuários
CREATE TABLE usuario (
  usuarioID INT IDENTITY(1,1) PRIMARY KEY,
  usuarioNome VARCHAR(100) NOT NULL,
  usuarioSenha VARCHAR(250) NOT NULL
);

-- Tabela de Chaves
CREATE TABLE chave (
  chaveID INT IDENTITY(1,1) PRIMARY KEY,
  chaveMac VARCHAR(20) NOT NULL,
  chaveDataCriacao DATETIME NOT NULL,
  chaveDataExpiracao DATETIME NOT NULL,
  chaveStatus BIT NOT NULL,
  chaveLicenca TEXT NOT NULL
);

-- Relação Chave ⇄ Produto
CREATE TABLE chaveProduto (
   chaveProdutoID INT IDENTITY(1,1) PRIMARY KEY,
   idChave INT NOT NULL FOREIGN KEY REFERENCES chave(chaveID),
   idProduto INT NOT NULL FOREIGN KEY REFERENCES produto(produtoID)
);

-- Tabela de Clientes
CREATE TABLE cliente (
   clienteID INT IDENTITY(1,1) PRIMARY KEY,
   clienteCNPJ VARCHAR(30) NOT NULL,
   clienteNomeRazaoSocial VARCHAR(250) NOT NULL,
   clienteNomeFantasia VARCHAR(250) NOT NULL,
   clienteStatus BIT NOT NULL
);

-- Relação Cliente ⇄ Chave
CREATE TABLE clienteChave (
   clienteChaveID INT IDENTITY(1,1) PRIMARY KEY,
   idCliente INT NOT NULL FOREIGN KEY REFERENCES cliente(clienteID),
   idChave INT NOT NULL FOREIGN KEY REFERENCES chave(chaveID)
);

-- Tabela de Aplicações
CREATE TABLE aplicacao (
   aplicacaoID INT IDENTITY(1,1) PRIMARY KEY,
   aplicacaoNome VARCHAR(250) NOT NULL,
   aplicacaoSenha VARCHAR(250) NOT NULL,
   aplicacaoToken VARCHAR(250) NOT NULL,
   tokenExpiracao DATETIME NOT NULL,
   dataCriacao DATETIME NOT NULL,
   dataAtualizacao DATETIME NOT NULL
);
```

---

## 📥 Instalação e Execução

1. Clone este repositório:

    ```bash
    git clone https://github.com/<seu-usuario>/minha-webapi-licencas.git
    cd minha-webapi-licencas
    ```

2. Abra o arquivo `appsettings.json` e atualize sua **connection string** para apontar ao SQL Server 2019:

    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Server=SEU_SERVIDOR;Database=LicencasDB;User Id=usuario;Password=senha;TrustServerCertificate=True;"
    }
    ```

3. Restaure as dependências e execute o projeto:

    ```bash
    dotnet restore
    dotnet run
    ```

4. A API estará disponível em:
    - `https://localhost:5001`
    - `http://localhost:5000`

