# Projeto Back-End .NET com Entity Framework, Swagger e Docker Compose

Este projeto é uma API RESTful desenvolvida em **.NET**, utilizando **Entity Framework Core** como ORM, documentação via **Swagger** e conteinerização com **Docker Compose**.

---

## Tecnologias Utilizadas

- [.NET 9 ou superior](https://dotnet.microsoft.com/)
- [Entity Framework Core](https://learn.microsoft.com/ef/)
- [Swagger (Swashbuckle)](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)
- [PostgreSQL / SQL Server (via Docker)]
- [Docker & Docker Compose](https://www.docker.com/)

---

## Pré-requisitos

Antes de começar, você precisa ter instalado:

- [.NET SDK](https://dotnet.microsoft.com/en-us/download)
- [Docker](https://www.docker.com/)
- [Docker Compose](https://docs.docker.com/compose/install/)

---

## ⚙️ Como Rodar o Projeto

### 1. Clone o repositório

```bash
git https://github.com/danlimax/api-econsulta
cd api-econsulta
```

### 2. Instalar dependências

```bash
dotnet restore
```

### 3. Rodar comando Docker Compose

```bash
docker-compose up -d
```

### 4. Instale as ferramentas do Entity Framework Core - CLI do .NET Core criar uma migration e atualizar o banco de dados.

-[ferramentas do Entity Framework Core - CLI do .NET Core](https://learn.microsoft.com/pt-br/ef/core/cli/dotnet)

```bash
dotnet ef migrations add NovaMigration

dotnet ef database update
```

### 5. Para rodar o projeto, pode ser iniciado de algumas formas

```bash
dotnet run

dotnet watch run

```

### 6. O swagger está na rota raiz para verificação dos endpoints.

- [Swagger http](http://localhost:5211/) Caso esteja utilizando o profile http.

### 7 Credenciais do banco de dados.

POSTGRES_USER: meuusuario
POSTGRES_PASSWORD: minhasenha
POSTGRES_DB: econsulta
