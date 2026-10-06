# 🎓 Gerador de Certificados

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)
![C#](https://img.shields.io/badge/C%23-239120)
![SQL Server](https://img.shields.io/badge/SQL%20Server-EF%20Core-CC2927)
![RabbitMQ](https://img.shields.io/badge/RabbitMQ-MassTransit-FF6600)
[![Academia do Programador](https://img.shields.io/badge/Academia%20do%20Programador-Fullstack%202026-6f42c1)](https://www.academiadoprogramador.net/inicio)

O **Gerador de Certificados** é uma API REST desenvolvida em ASP.NET Core para automatizar a emissão de certificados de conclusão de cursos.

O sistema permite cadastrar usuários e cursos, solicitar a geração de certificados para vários alunos e acompanhar o processamento de forma assíncrona. Os certificados são gerados individualmente em PDF e disponibilizados em um arquivo ZIP para download.

A aplicação utiliza mensageria com RabbitMQ e MassTransit, persistência com SQL Server e autenticação baseada em JWT.

## Projeto

Desenvolvido durante o curso **Fullstack 2026** da [Academia do Programador](https://www.academiadoprogramador.net/), com foco na aplicação prática de conceitos de desenvolvimento de APIs, arquitetura de software, processamento assíncrono e persistência de dados.

## Funcionalidades

- Cadastro e autenticação de usuários com JWT.
- Cadastro e consulta de cursos.
- Solicitação de certificados para um ou mais alunos.
- Processamento assíncrono por meio de RabbitMQ e MassTransit.
- Geração de certificados individuais em PDF.
- Acompanhamento do status da geração.
- Compactação dos certificados em um arquivo ZIP.
- Download dos certificados gerados.
- Validação das regras de negócio.

### Fluxo de geração

O processamento dos certificados ocorre de forma assíncrona, permitindo que a API responda à solicitação sem precisar aguardar a geração de todos os arquivos.

1. O usuário realiza a autenticação e cadastra um curso.
2. Envia uma solicitação contendo os nomes dos alunos.
3. A aplicação valida os dados e registra um lote de geração.
4. Uma mensagem é enviada ao RabbitMQ.
5. O Consumer recebe a mensagem e recupera o lote no banco de dados.
6. Os certificados são gerados individualmente em PDF.
7. Os arquivos são compactados em um ZIP.
8. O processamento é concluído e o arquivo fica disponível para download.

A solicitação de geração retorna **HTTP 202 Accepted**, indicando que o processamento foi aceito e será realizado de forma assíncrona.

## Getting Started

### Prerequisites

Antes de executar o projeto, certifique-se de possuir as seguintes ferramentas instaladas:

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [Git](https://git-scm.com/downloads)
- SQL Server
- RabbitMQ

O RabbitMQ pode ser executado em um contêiner Docker.

### Clone o repositório

Clone o projeto e acesse o diretório da solução:

```bash
git clone https://github.com/Os-Desinstanciados/GeradorDeCertificados
cd GeradorDeCertificados
```

### Configure o ambiente

Configure a conexão com o SQL Server, o acesso ao RabbitMQ e as informações necessárias para autenticação JWT nas configurações da aplicação.

Não armazene senhas, chaves JWT ou outras credenciais diretamente no repositório. Para o ambiente de desenvolvimento, utilize variáveis de ambiente ou o .NET User Secrets.

Certifique-se de que o SQL Server e o RabbitMQ estejam disponíveis antes de iniciar a API.

### Configure o banco de dados

Aplique as migrations existentes do Entity Framework Core para criar e atualizar o banco de dados.

Execute o comando de atualização das migrations, informando o projeto de infraestrutura e a API como projeto de inicialização, conforme a configuração da solução.

### Testes

O projeto possui testes automatizados desenvolvidos com MSTest para validar as entidades, regras de negócio, casos de uso da camada de aplicação e geração dos documentos PDF.

| Teste | Tecnologia | Objetivo | Situação |
|---|---|---|---|
| Unitários de domínio | MSTest | Validar entidades e regras de negócio | Concluído |
| Unitários de aplicação | MSTest + Moq | Validar os fluxos da camada de aplicação de forma isolada | Concluído |
| Geração de PDF | MSTest + QuestPDF + PDF Pig | Validar a geração e o conteúdo dos certificados | Concluído |

**Testes implementados:**

| Classe | Quantidade |
|---|---:|
| Curso | 12 |
| Certificado | 12 |
| Gerador | 20 |
| CertificadoPdfGenerator | 2 |
| SolicitarGeracaoCertificadosCommandHandler | 3 |
| **Total** | **49** |

Todos os **49 testes** foram executados com sucesso.

**Executar os testes:**

```bash
dotnet test ./tests/GeradorDeCertificados.Teste.Unidade/GeradorDeCertificados.Teste.Unidade.csproj
```

**Executar todos os projetos de teste da solução:**

```bash
dotnet test
```

### Run the app

Execute a API:

```bash
dotnet run --project ./src/Api
```

Após a inicialização, acesse o endereço exibido no terminal para utilizar a API.

A documentação interativa dos endpoints está disponível por meio do Swagger.

## Tecnologias

- **.NET 10** — plataforma de desenvolvimento.
- **C#** — linguagem de programação.
- **ASP.NET Core Web API** — desenvolvimento da API REST.
- **Entity Framework Core** — mapeamento objeto-relacional e persistência de dados.
- **SQL Server** — banco de dados relacional.
- **MassTransit** — abstração e integração com o sistema de mensageria.
- **RabbitMQ** — gerenciamento das filas e mensagens.
- **MediatR** — organização e execução dos casos de uso.
- **JWT** — autenticação baseada em tokens.
- **Swagger** — documentação e testes interativos dos endpoints.
- **QuestPDF** — geração dos certificados em PDF.
- **PDF Pig** — leitura e validação dos documentos PDF nos testes.
- **Moq** — criação de mocks para os testes unitários da camada de aplicação.
- **Docker** — execução de serviços de infraestrutura.
- **MSTest** — testes automatizados.

## Arquitetura

O projeto utiliza uma arquitetura em camadas, separando as responsabilidades entre **Domínio, Aplicação, Infraestrutura e API**.

### 🧠 Domínio

Contém as entidades, os contratos e as regras de negócio da aplicação.

Seus principais módulos são:

- **Cursos:** informações e validações dos cursos.
- **Certificados:** certificados individuais e controle dos lotes de geração.

A classe `Gerador` é responsável por representar o lote de processamento e controlar seus estados.

### ⚙️ Aplicação

Responsável pela coordenação dos casos de uso, processamento dos comandos e comunicação com os serviços necessários.

Utiliza MediatR para organizar as operações e MassTransit para encaminhar as solicitações de geração ao processamento assíncrono.

### 🗄️ Infraestrutura

Responsável pela persistência e integração com os serviços externos.

Inclui:

- Entity Framework Core e SQL Server.
- Implementação dos repositórios.
- Migrations do banco de dados.
- Geração dos documentos PDF.
- Armazenamento dos arquivos.
- Integração com RabbitMQ.

### 🌐 API

Responsável por receber as requisições HTTP, disponibilizar os endpoints e retornar as respostas aos clientes.

Disponibiliza as operações de autenticação, gerenciamento de cursos, solicitação de certificados, consulta de status e download dos arquivos.

## Processamento assíncrono

A geração dos certificados utiliza MassTransit e RabbitMQ para separar o recebimento da requisição HTTP do processamento dos arquivos.

Após a validação da solicitação, a aplicação registra o lote no banco de dados e envia uma mensagem para a fila.

O Consumer recupera as informações do lote, gera os PDFs, registra os resultados individuais e compacta os arquivos gerados.

O estado do lote pode ser consultado durante o processamento, permitindo acompanhar a operação sem manter a requisição HTTP aberta.

## Situação do projeto

- [x] Cadastro e autenticação de usuários.
- [x] Cadastro de cursos.
- [x] Solicitação de geração de certificados.
- [x] Integração com RabbitMQ e MassTransit.
- [x] Geração de PDFs.
- [x] Compactação e download de arquivos ZIP.
- [x] Validação do fluxo completo localmente.
- [x] Implementação de 49 testes automatizados.
- [x] Testes unitários da camada de aplicação.
- [x] Testes de geração e validação de PDFs.
- [x] Deploy em ambiente de produção.