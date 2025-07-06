# API CRUD Completa com C# e .NET 8 (Minimal APIs)

Este projeto é uma **API RESTful completa** que implementa as operações **CRUD** (Create, Read, Update, Delete) para gerenciar dados de pessoas. Ele foi desenvolvido utilizando as **Minimal APIs do .NET 8**, focando na simplicidade e eficiência.

## Tecnologias Utilizadas

*   **C#**
*   **.NET 8**: A versão mais recente do framework, com foco em performance e recursos como as Minimal APIs.
*   **Minimal APIs**: Uma abordagem simplificada para construir APIs HTTP com o .NET, sem a necessidade de controllers tradicionais.
*   **Entity Framework Core**: Um ORM (Object-Relational Mapper) que facilita a interação com o banco de dados.
*   **SQLite**: Um banco de dados leve e baseado em arquivo, ideal para prototipagem e aplicações menores.
*   **Swagger/OpenAPI**: Ferramenta para documentação e teste interativo da API.

## Configuração do Projeto

Para configurar e rodar este projeto na sua máquina, siga os passos abaixo:

1.  **Verificar a versão do .NET SDK**:
    Certifique-se de ter o .NET 8 SDK instalado. Você pode verificar isso executando no terminal:
    
    ```bash
    dotnet --info
    ```
    
    Se você tiver versões anteriores ou mais recentes (como a 9.0 Release Candidate), não há problema, mas o projeto foi desenvolvido com foco na versão 8.

3.  **Criar o Projeto (se estiver começando do zero)**:
    Navegue até a pasta onde deseja criar o projeto e execute o comando:
    
    ```bash
    dotnet new webapi -o Person --minimal
    ```
    
    Este comando cria um novo projeto de Web API com Minimal APIs em uma pasta chamada `Person`.

5.  **Abrir o Projeto**:
    Abra a pasta `Person` no seu IDE preferido (como Rider ou Visual Studio).

6.  **Instalar Pacotes NuGet do Entity Framework Core**:
    Precisamos instalar três pacotes importantes para o Entity Framework Core e o SQLite. Você pode fazer isso via interface do seu IDE (Gerenciador de Pacotes NuGet) ou via terminal:
    *   **Microsoft.EntityFrameworkCore**: O pacote principal do EF Core.
    *   **Microsoft.EntityFrameworkCore.SQLite**: O provider para SQLite.
    *   **Microsoft.EntityFrameworkCore.Design**: Ferramentas de design para migrações.

    No terminal (dentro da pasta do projeto `Person`):
    
    ```bash
    dotnet add package Microsoft.EntityFrameworkCore --version 8.0.0
    dotnet add package Microsoft.EntityFrameworkCore.Sqlite --version 8.0.0
    dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.0
    ```
    
    **Importante**: Certifique-se de usar a versão `8.0.0` para corresponder ao .NET 8.

8.  **Instalar a Ferramenta Global `dotnet-ef`**:
    Esta ferramenta é necessária para executar comandos de migração.
    
    ```bash
    dotnet tool install --global dotnet-ef --version 8.0.0
    ```
    
    Se já estiver instalada, o comando irá reinstalar ou informar que já existe.

## Estrutura e Conceitos

Este projeto foi estruturado para demonstrar as capacidades das Minimal APIs e algumas boas práticas de design:

*   **Minimal APIs**: Ao invés de usar `Controllers`, todos os endpoints são configurados diretamente no arquivo `Program.cs` [2, 5]. Para uma melhor organização e modularidade, utilizei o conceito de **`MapGroup`** para agrupar as rotas relacionadas a `Person`.

*   **Modelos (`PersonModel` e `PersonRequest`)**:
    *   `PersonModel.cs`: Representa a entidade `Pessoa` no banco de dados. Possui propriedades como `ID` (do tipo `Guid` para garantir unicidade global) e `Name`.
        *   O `ID` é inicializado automaticamente no construtor com um novo `Guid`.
        *   As propriedades têm `Private set` para que a alteração dos dados seja feita através de métodos específicos na própria classe (`ChangeName`, `SetInactive`), seguindo o princípio da **responsabilidade única**.
    *   `PersonRequest.cs`: Utilize um `record` para a requisição de criação/atualização de pessoa. `Record`s são tipos de referência ideais para DTOs (Data Transfer Objects) por serem mais concisos e imutáveis.

*   **Contexto de Banco de Dados (`PersonContext.cs`)**:
    *   Herda de `DbContext` do Entity Framework Core e representa a sessão do banco de dados.
    *   Contém um `DbSet<PersonModel>` chamado `People`, que simboliza a tabela de pessoas no banco de dados.
    *   O método `OnConfiguring` é sobrescrito para especificar o uso do SQLite e a string de conexão para o arquivo `Person.sqlite`.

*   **Injeção de Dependência**:
    O `PersonContext` é injetado como um serviço `Scoped` no contêiner de dependência da aplicação (`builder.Services.AddScoped<PersonContext>()`), permitindo que ele seja acessado pelos handlers das rotas.

## Migrações e Banco de Dados

Para criar e atualizar o banco de dados baseado no modelo C#:

1.  **Gerar a Primeira Migração**:
    No terminal, dentro da pasta do projeto, execute:
    
    ```bash
    dotnet ef migrations add Initial
    ```
    
    Isso criará uma pasta `Migrations` com o código C# que descreve as alterações no banco de dados (tabela `People` com `ID` e `Name`, e a tabela de histórico de migrações `__EFMigrationsHistory`). O método `Up` define as alterações a serem aplicadas, e o `Down` as reverte.

3.  **Atualizar o Banco de Dados**:
    Para aplicar as migrações e criar o arquivo `Person.sqlite`, execute:
    
    ```bash
    dotnet ef database update
    ```
    
    Este comando cria o arquivo do banco de dados e aplica todas as migrações pendentes [16]. Após a execução, você verá um arquivo `Person.sqlite` na raiz do projeto. Você pode usar uma ferramenta como o Database Explorer do Rider (ou extensões em outros IDEs) para visualizar o conteúdo do banco de dados.

## Operações CRUD

Aqui está como as operações CRUD foram implementadas:

*   **Criar (POST)**
    *   **Rota**: `POST /Person`
    *   **Função**: Adiciona uma nova pessoa ao banco de dados.
    *   **Detalhes**: Recebe um `PersonRequest` no corpo da requisição, cria uma nova instância de `PersonModel` (com ID gerado automaticamente), adiciona ao `DbContext` e salva as alterações de forma assíncrona (`SaveChangesAsync`).
    *   **Exemplo de uso no Swagger**: O Swagger solicitará o `name` para criar a pessoa.

*   **Ler (GET)**
    *   **Rota**: `GET /Person`
    *   **Função**: Retorna uma lista de todas as pessoas cadastradas.
    *   **Detalhes**: Busca todos os registros da tabela `People` de forma assíncrona (`ToListAsync`) e retorna-os com um status `200 OK`.
    *   **Exemplo de uso no Swagger**: Não requer parâmetros e exibe todos os registros.

*   **Atualizar (PUT)**
    *   **Rota**: `PUT /Person/{id}`
    *   **Função**: Atualiza o nome de uma pessoa específica pelo seu ID.
    *   **Detalhes**:
        *   Recebe o `ID` da pessoa na rota (do tipo `Guid`) e o `PersonRequest` com o novo nome no corpo da requisição.
        *   Primeiro, busca a pessoa pelo `ID` usando `FirstOrDefaultAsync` (para evitar exceções se o ID não for encontrado).
        *   Se a pessoa não for encontrada, retorna `404 Not Found`.
        *   Caso contrário, o nome é atualizado usando o método `ChangeName` do `PersonModel` e as alterações são salvas assincronamente.
    *   **Exemplo de uso no Swagger**: Requer o ID da pessoa e o novo nome.

*   **Deletar (DELETE - Soft Delete Exemplo)**
    *   **Rota**: `DELETE /Person/{id}`
    *   **Função**: "Desativa" uma pessoa pelo seu ID, em vez de excluí-la fisicamente do banco.
    *   **Detalhes**:
        *   Este projeto implementa um **soft delete** para fins demonstrativos. Isso significa que, em vez de remover a linha do banco de dados (hard delete), o nome da pessoa é alterado para "desativado" usando o método `SetInactive` no `PersonModel`. Isso permite manter o histórico dos dados.
        *   Recebe o `ID` da pessoa na rota (do tipo `Guid`).
        *   Busca a pessoa pelo `ID`. Se não encontrar, retorna `404 Not Found`.
        *   Se encontrada, o nome é alterado e as mudanças são salvas.
    *   **Exemplo de uso no Swagger**: Requer o ID da pessoa a ser desativada.

## Boas Práticas e Observações

*   **`Guid` como ID**: A escolha de `Guid` para IDs é uma boa prática para evitar conflitos em sistemas distribuídos e ao fazer migrações de dados.
*   **Métodos para Alteração de Propriedades**: Utilizar métodos como `ChangeName` e `SetInactive` no modelo, em vez de permitir alteração direta de propriedades, promove o **Domain-Driven Design (DDD)** e evita erros comuns de desenvolvedores.
*   **Operações Assíncronas**: Todas as interações com o banco de dados são assíncronas (`async/await`) para melhorar a performance da aplicação, liberando threads para outras requisições.
*   **Swagger**: A aplicação já vem com o Swagger configurado, permitindo testar facilmente todos os endpoints no navegador.
*   **Tokens de Cancelamento**: Embora não implementados exaustivamente neste exemplo, em aplicações de produção, é uma boa prática utilizar `CancellationToken` em operações assíncronas para permitir que requisições longas sejam canceladas, evitando que o banco de dados continue processando requisições que não são mais necessárias.




