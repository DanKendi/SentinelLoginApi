# 🛰️ Sentinel API

API REST de autenticação desenvolvida em **ASP.NET Core 8** para o sistema de **Previsão de Incêndios com Dados Espaciais**.  
Responsável pelo cadastro e login de usuários, integrada ao **Firebase Authentication** para geração de tokens JWT e ao banco de dados **Oracle** para persistência.


## Integrantes

| Nome | RM |
|---|---|
| Daniel K S Araki | 553043 | 
| Jonas K Isiki | 560560 | 
| Lucas R Barbosa | 560179 |
| Marcos V A Marques | 560475 | 


---

## 📑 Índice

- [Visão Geral](#visão-geral)
- [Arquitetura](#arquitetura)
- [Tecnologias](#tecnologias)
- [Estrutura do Projeto](#estrutura-do-projeto)
- [Fluxo de Autenticação](#fluxo-de-autenticação)
- [Banco de Dados](#banco-de-dados)
- [Configuração e Execução](#configuração-e-execução)
- [Endpoints](#endpoints)
- [Exemplos de Requisição](#exemplos-de-requisição)
- [Testes](#testes)
- [Variáveis para a API Java](#variáveis-para-a-api-java)

---

## Visão Geral

A Sentinel API é responsável exclusivamente pela camada de **autenticação e gerenciamento de perfil** do sistema. As demais funcionalidades (alertas, focos de calor, notificações) são tratadas por uma API Java que consome o mesmo token JWT gerado pelo Firebase.

```
App Mobile (React Native)
        │
        ├──► POST /api/auth/register ──► Sentinel API (.NET)
        │                                       │
        ├──► POST /api/auth/login ───────────────┤
        │         │                             │
        │    idToken (JWT)              Firebase Auth + Oracle
        │         │
        └──► API Java (demais endpoints)
                  │
             Valida idToken via Firebase JWKS
```

---

## Arquitetura

O projeto segue os princípios de **Clean Architecture**, dividido em 4 camadas com dependências unidirecionais:

```
┌─────────────────────────────────────────────┐
│              SentinelApi.WebApi              │  ← Controllers, Middlewares, Program.cs
├─────────────────────────────────────────────┤
│           SentinelApi.Application            │  ← Use Cases, DTOs, Validators, Interfaces
├─────────────────────────────────────────────┤
│           SentinelApi.Infrastructure         │  ← EF Core, Oracle, Firebase, Repositórios
├─────────────────────────────────────────────┤
│             SentinelApi.Domain               │  ← Entidades, Interfaces, Exceções
└─────────────────────────────────────────────┘
```

**Regra de dependência:** cada camada só conhece a camada imediatamente abaixo. O Domain não depende de ninguém.

### Princípios aplicados

| Princípio | Onde |
|---|---|
| **Single Responsibility** | Um Use Case por operação (Register, Login, UpdateProfile) |
| **Open/Closed** | Novos repositórios implementam interfaces sem alterar o Domain |
| **Dependency Inversion** | Use Cases dependem de `IUsuarioRepository`, não de `UsuarioRepository` |
| **Injeção de Dependência** | Tudo registrado no `Program.cs` via `builder.Services` |

---

## Tecnologias

| Tecnologia | Versão | Uso |
|---|---|---|
| ASP.NET Core | 8.0 | Framework principal |
| Entity Framework Core | 8.0.11 | ORM para Oracle |
| Oracle.EntityFrameworkCore | 8.23.26000 | Driver Oracle |
| FirebaseAdmin SDK | 3.5.0 | Criação de usuários server-side |
| Firebase REST API | — | Login por e-mail/senha |
| FluentValidation | 12.1.1 | Validação de DTOs |
| BCrypt.Net-Next | 4.2.0 | Disponível para hash de senha |
| Swashbuckle (Swagger) | 6.6.2 | Documentação OpenAPI |
| AspNetCore.HealthChecks.Oracle | 8.0.1 | Health Check do banco |
| xUnit | — | Testes automatizados |
| Moq | 4.20.72 | Mocks nos testes |
| FluentAssertions | — | Asserções expressivas |

---

## Fluxo de Autenticação

### Cadastro

```
Cliente                  Sentinel API              Firebase           Oracle
   │                          │                       │                 │
   │── POST /auth/register ──►│                       │                 │
   │                          │── Valida campos ──────│                 │
   │                          │── Verifica e-mail ────│────────────────►│
   │                          │◄─ E-mail livre ───────│─────────────────│
   │                          │── CreateUserAsync ────►│                 │
   │                          │◄─ uid ────────────────│                 │
   │                          │── Salva no Oracle ────│────────────────►│
   │                          │── SignInAsync ─────────►│                │
   │                          │◄─ idToken ────────────│                 │
   │◄── 201 { idToken, uid } ─│                       │                 │
```

### Login

```
Cliente                  Sentinel API              Firebase           Oracle
   │                          │                       │                 │
   │── POST /auth/login ──────►│                       │                 │
   │                          │── Valida campos ──────│                 │
   │                          │── SignInAsync ─────────►│                │
   │                          │◄─ idToken + uid ──────│                 │
   │                          │── Busca por uid ──────│────────────────►│
   │                          │◄─ dados do usuário ───│─────────────────│
   │◄── 200 { idToken, uid } ─│                       │                 │
```

---

## Banco de Dados

O banco Oracle contém **8 tabelas**. A Sentinel API opera diretamente sobre 3 delas:

| Tabela | Descrição |
|---|---|
| `T_SEN_USUARIO` | Dados cadastrais, localização e token FCM |
| `T_SEN_REGIAO` | Regiões geográficas monitoradas |
| `T_SEN_USUARIO_REGIAO` | Inscrições de usuários em regiões (N:N) |

As demais tabelas (`T_SEN_ALERTA`, `T_SEN_FOCO_CALOR`, `T_SEN_SATELITE`, `T_SEN_NOTIFICACAO`, `T_SEN_HISTORICO_RISCO`) são gerenciadas pela API Java.

### Diagrama DER

<img width="1590" height="781" alt="image" src="https://github.com/user-attachments/assets/bdf93b8a-6019-44b3-b2fa-da6b493d4d05" />


---

## Configuração e Execução

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- Visual Studio 2022 ou VS Code
- Acesso ao banco Oracle (instância local ou remota)
- Projeto criado no [Firebase Console](https://console.firebase.google.com)

### 1. Clonar o repositório

```bash
git clone https://github.com/seu-usuario/SentinelApi.git
cd SentinelApi
```

### 2. Configurar o appsettings.json

Abra `SentinelApi.WebApi/appsettings.json` e preencha com seus dados:

```json
{
  "ConnectionStrings": {
    "Oracle": "User Id=SEU_USUARIO;Password=SUA_SENHA;Data Source=SEU_HOST:1521/SEU_SERVICE;"
  },
  "Firebase": {
    "ProjectId": "SEU_PROJECT_ID",
    "WebApiKey": "SUA_WEB_API_KEY"
  }
}
```

### 3. Adicionar o serviceAccountKey.json

Faça o download da chave privada do Firebase Console:
**Configurações do projeto → Contas de serviço → Gerar nova chave privada**

Renomeie para `serviceAccountKey.json` e coloque na raiz do projeto `SentinelApi.WebApi`.

> ⚠️ Este arquivo está no `.gitignore` e **nunca deve ser commitado**.

### 4. Aplicar as Migrations

As migrations já estão registradas no Oracle. Se estiver configurando do zero em um banco novo:

```sql
CREATE TABLE "__EFMigrationsHistory" (
    "MigrationId"    VARCHAR2(150) NOT NULL,
    "ProductVersion" VARCHAR2(32)  NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

INSERT INTO "__EFMigrationsHistory" VALUES ('InitialCreate', '8.0.11');
INSERT INTO "__EFMigrationsHistory" VALUES ('AddUidFirebase', '8.0.11');
COMMIT;
```

### 5. Executar

No Visual Studio, defina `SentinelApi.WebApi` como **Startup Project** e pressione **F5**.

Ou via CLI:

```bash
cd SentinelApi.WebApi
dotnet run
```

A aplicação sobe em `http://localhost:{porta}` com o **Swagger UI na raiz**.

---

## Endpoints

| Método | Rota | Autenticação | Descrição |
|---|---|---|---|
| `POST` | `/api/auth/register` | ❌ Pública | Cadastra novo usuário |
| `POST` | `/api/auth/login` | ❌ Pública | Autentica e retorna idToken |
| `PUT` | `/api/usuario/perfil` | ✅ JWT | Atualiza localização e raio |
| `GET` | `/health` | ❌ Pública | Status da API e do banco Oracle |

---

## Exemplos de Requisição

### POST /api/auth/register

**Request:**
```json
{
  "nome": "João Silva",
  "email": "joao@email.com",
  "senha": "senha123",
  "fcmToken": "token_do_dispositivo_firebase",
  "raioKm": 100
}
```

**Response 201:**
```json
{
  "idToken": "eyJhbGciOiJSUzI1NiIsImtpZCI6Ii...",
  "uid": "xK92mPabc123",
  "nome": "João Silva",
  "email": "joao@email.com"
}
```

**Response 400 — E-mail duplicado:**
```json
{
  "title": "E-mail já cadastrado.",
  "status": 400
}
```

**Response 400 — Dados inválidos:**
```json
{
  "title": "Dados inválidos.",
  "status": 400,
  "detail": "Email: E-mail inválido. | Senha: Senha deve ter pelo menos 6 caracteres."
}
```

---

### POST /api/auth/login

**Request:**
```json
{
  "email": "joao@email.com",
  "senha": "senha123",
  "fcmToken": "novo_token_do_dispositivo"
}
```

> O campo `fcmToken` é opcional. Se enviado, atualiza o token do dispositivo no Oracle.

**Response 200:**
```json
{
  "idToken": "eyJhbGciOiJSUzI1NiIsImtpZCI6Ii...",
  "uid": "xK92mPabc123",
  "nome": "João Silva",
  "email": "joao@email.com"
}
```

**Response 401 — Credenciais inválidas:**
```json
{
  "title": "E-mail ou senha inválidos.",
  "status": 400
}
```

---

### PUT /api/usuario/perfil

> Requer header: `Authorization: Bearer SEU_ID_TOKEN`

**Request:**
```json
{
  "latitude": -23.5505,
  "longitude": -46.6333,
  "raioKm": 75
}
```

**Response 204:** sem corpo — perfil atualizado com sucesso.

**Response 401:** token ausente, expirado ou inválido.

---

### GET /health

**Response 200:**
```json
{
  "status": "Healthy",
  "checks": [
    {
      "name": "oracle-db",
      "status": "Healthy",
      "description": null
    }
  ]
}
```

---

## Testes

O projeto `SentinelApi.Tests` contém **11 testes automatizados** seguindo o padrão **AAA (Arrange, Act, Assert)**.

### Executar os testes

No Visual Studio:
**Test → Test Explorer → Run All Tests**

Via CLI:
```bash
dotnet test
```

### Cobertura dos testes

#### RegisterUserUseCase (3 testes)

| Teste | Cenário | Resultado esperado |
|---|---|---|
| `ExecuteAsync_DeveRetornarAuthResponse_QuandoDadosValidos` | Cadastro com dados corretos | Retorna `AuthResponse` com uid e idToken |
| `ExecuteAsync_DeveLancarDomainException_QuandoEmailJaCadastrado` | E-mail já existe no Oracle | Lança `DomainException` e não chama o Firebase |
| `ExecuteAsync_NaoDeveSalvarNoOracle_QuandoFirebaseFalha` | Firebase lança exceção | Lança `DomainException` e não salva no Oracle |

#### LoginUserUseCase (3 testes)

| Teste | Cenário | Resultado esperado |
|---|---|---|
| `ExecuteAsync_DeveRetornarAuthResponse_QuandoCredenciaisValidas` | Login com dados corretos | Retorna `AuthResponse` com idToken |
| `ExecuteAsync_DeveLancarDomainException_QuandoCredenciaisInvalidas` | Senha errada | Lança `DomainException` e não consulta o Oracle |
| `ExecuteAsync_DeveLancarDomainException_QuandoUsuarioNaoEncontradoNoOracle` | Existe no Firebase mas não no Oracle | Lança `DomainException` |

#### RegisterRequestValidator (5 testes)

| Teste | Cenário | Resultado esperado |
|---|---|---|
| `Validate_DevePassar_QuandoDadosValidos` | Todos os campos corretos | `IsValid = true` |
| `Validate_DeveFalhar_QuandoEmailInvalido` | E-mail sem `@` | Erro no campo `Email` |
| `Validate_DeveFalhar_QuandoSenhaMenorQue6Caracteres` | Senha com 3 chars | Erro no campo `Senha` |
| `Validate_DeveFalhar_QuandoRaioNegativo` | `raioKm = -10` | Erro no campo `RaioKm` |
| `Validate_DeveFalhar_QuandoRaioAcimaDoLimite` | `raioKm = 999` | Erro no campo `RaioKm` |

---

## Variáveis para a API Java

A API Java precisa das seguintes configurações para validar o mesmo token JWT gerado pelo Firebase:

```yaml
jwt:
  issuer: https://securetoken.google.com/SEU_PROJECT_ID
  jwks-uri: https://www.googleapis.com/robot/v1/metadata/x509/securetoken@system.gserviceaccount.com
  audience: SEU_PROJECT_ID
```

> ⚠️ **Não existe `jwt.secret`** — a validação usa criptografia assimétrica com as chaves públicas do Firebase (JWKS). O `SEU_PROJECT_ID` é o mesmo valor configurado em `Firebase:ProjectId` no `appsettings.json` desta API.

---

## Tratamento de Erros

Todos os erros são tratados pelo `ExceptionHandlingMiddleware` e retornam no formato `ProblemDetails`:

| Exceção | Status HTTP | Quando ocorre |
|---|---|---|
| `DomainException` | 400 | Regra de negócio violada (e-mail duplicado, dados inválidos) |
| `UnauthorizedAccessException` | 401 | Acesso negado a recurso de outro usuário |
| `Exception` (genérica) | 500 | Erro inesperado interno |

---

*Desenvolvido por Daniel — FIAP ADS 2025*
