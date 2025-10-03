# APIFCG

**APIFCG** é uma aplicação web Orquetradora de Microsserviços desenvolvida em .NET 8.0, como parte do projeto de pós-graduação em **Arquitetura de Sistemas .NET com Azure**, da FIAP.  
Este projeto representa o andamento da plataforma **FIAP Cloud Games (FCG)**, que visa oferecer uma solução completa para a venda de jogos digitais.
---

## Desenho da Arquitetura.

![Imagem Arquitetura](/APIFCG/Assets/images/Arquitetura.png)

---

## 🧱 Arquitetura

- **Microsserviços**: Sendo esta API centralizadora para a chamada dos Microserviços
- **Oquestradora**: Esta APi fica responsavel por chamar os microserviços de Jogos, Pagamento e usuarios.
- **.NET 8.0 MVC**: uso do padrão Controller-Based MVC
- **Camadas**:
  - Apresentação (Controllers)
  - Aplicação (Serviços)
  - Domínio (Entidades e Regras de Negócio)
  - Infraestrutura (Acesso a dados com EF Core)

---

## 🎯 Objetivo

Criar uma base robusta e escalável para a FCG, garantindo:
- Persistência de dados
- Segurança
- Qualidade do software
- Práticas modernas de desenvolvimento

---

## ✅ Funcionalidades

### 🧑 Cadastro de Usuários
- Nome, e-mail e senha
- Validação de:
  - Formato de e-mail
  - Senha segura (mínimo 8 caracteres, com letras, números e caracteres especiais)

### 🔐 Autenticação e Autorização
- Autenticação via **JWT**
- Perfis de acesso:
  - **Usuário**: acesso à plataforma e à biblioteca de jogos
  - **Administrador**: cadastro de jogos, administração de usuários e promoções

---

## 🛠️ Tecnologias Utilizadas

- **.NET 8.0**
- **Entity Framework Core**
- **MySql**
- **Swagger/OpenAPI** (documentação)
- **JWT** (autenticação)
- **xUnit / NUnit** (testes unitários)
- **FluentValidation** (validações)
- **New Relic** (monitoração de aplicação)
- **Microsoft Azure** (hospedagem e infraestrutura)

---

## 🧪 Qualidade de Software

- Cobertura de testes unitários para regras de negócio
- Adoção de **TDD** ou **BDD** em pelo menos um módulo
- Middleware de tratamento de erros com logs estruturados
- Validações robustas com mensagens claras

---

## 📦 Como executar o projeto

### Pré-requisitos

- [.NET SDK 8.0](https://dotnet.microsoft.com/en-us/download)
- [MySql](https://learn.microsoft.com/pt-br/sql/database-engine/configure-windows/sql-server-express-localdb)
- IDE recomendada: Visual Studio 2022 ou superior

### Clonar o repositório

```bash
git clone https://github.com/seu-usuario/APIFCG.git
cd APIFCG
```

---

## ☁️ Implantação e Monitoração

- O projeto está hospedado na **Microsoft Azure**, garantindo alta disponibilidade e escalabilidade.
- A monitoração da aplicação é realizada via **New Relic**, permitindo acompanhamento em tempo real de métricas, logs e performance.