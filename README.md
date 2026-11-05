# Agenda Online · Barbearia Luciano

Sistema de agendamento online da Barbearia Luciano (Araguari/MG).

- O **cliente** reserva sozinho, sem criar conta.
- O **barbeiro** vê as reservas na agenda dele.
- O **proprietário** gerencia barbeiros, serviços e horários.

Projeto acadêmico, equipe de 8 pessoas.

## Datas

| Data | Entrega |
| --- | --- |
| 05/11/2026 | Sprint 1 Review (apresentação do software) |
| **19/11/2026** | **Sprint 2 Review: incremento funcional** |
| 26/11/2026 | Folga para ajustes |
| **03/12/2026** | **Entrega final (V2)** |

As datas seguem o plano de ensino. Se o board da equipe divergir, vale o plano de ensino.

## Estado atual

Legenda: ✅ na `main` · 🚧 planejado, ainda não está na `main`

| Parte | Estado |
| --- | --- |
| Domain: `Intervalo`, `CalculadoraDisponibilidade`, `Agendamento` + testes | ✅ |
| CI (build e testes em todo PR) | ✅ |
| Application: casos de uso (agendar, cancelar, listar horários) | 🚧 |
| Infrastructure: PostgreSQL + Entity Framework Core | 🚧 (só no PC do Lead) |
| API: endpoints reais (hoje só tem o exemplo `/weatherforecast`) | 🚧 |
| Front-end React em `web/` | 🚧 (pasta ainda não existe) |
| Testes de integração contra PostgreSQL | 🚧 (só o teste vazio de exemplo) |

## Quem faz o quê

| Papel | Responsável por |
| --- | --- |
| Lead | Organiza o time, revisa PRs, disponibilidade e concorrência |
| Back-end | Application, Infrastructure e endpoints da API |
| QA/Ops | Testes, CI e ambiente |
| Front A | Área pública: o cliente escolhe serviço, barbeiro e horário |
| Front B | Painel do barbeiro: agenda do dia, cancelamentos |
| Front C | Gestão do proprietário: barbeiros, serviços, horários |

As tarefas de cada um estão no board da equipe. Peça o link ao Lead.

## Stack

| Parte | Tecnologia |
| --- | --- |
| Back-end | .NET 10 (ASP.NET Core Web API) |
| Banco | PostgreSQL + Entity Framework Core 🚧 |
| Front-end | React, na pasta `web/` 🚧 |
| Testes | xUnit |
| CI | GitHub Actions |

## Primeiros passos

### 1. Instale

- **Git**
- **.NET SDK 10.0.400** ou mais novo da linha 10.0.4xx. A versão fica travada no `global.json`.
- **Node.js LTS** (só para o front) 🚧
- **PostgreSQL** (só para o back, quando o banco entrar na `main`) 🚧

Confira o SDK:

```bash
dotnet --version   # precisa mostrar 10.0.4xx
```

### 2. Clone, compile e teste

```bash
git clone https://github.com/heitorbolisw4/agenda-online-barbearia.git
cd agenda-online-barbearia
dotnet build
dotnet test
```

Se `dotnet test` passar, seu ambiente está pronto para o back-end.

### 3. Suba a API

```bash
dotnet run --project src/Agenda.Web
```

A porta aparece no terminal. Em desenvolvimento, a documentação da API fica em `/openapi/v1.json`.

### 4. Banco de dados 🚧

Ainda não está na `main`. O plano:

1. Criar um banco local `agenda` no PostgreSQL.
2. Guardar a connection string com `dotnet user-secrets` no projeto `src/Agenda.Web` (nunca no `appsettings.json`).
3. Criar as tabelas com `dotnet ef database update`.

Os comandos exatos entram aqui junto com o PR do banco.

### 5. Front-end 🚧

Ainda não existe. O plano: um app React em `web/`, rodando com `npm install` e `npm run dev`, chamando a API do passo 3.

O time de front define a ferramenta (ex.: Vite) no PR que criar a pasta, e atualiza esta seção.

## Estrutura

```
src/
  Agenda.Domain/          regras de negócio puras, não referencia nenhum projeto
  Agenda.Application/     casos de uso e interfaces (ex.: repositórios)
  Agenda.Infrastructure/  banco de dados e implementações das interfaces
  Agenda.Web/             API HTTP; liga todas as peças no Program.cs
tests/
  Agenda.Domain.Tests/        testes das regras puras, sem banco
  Agenda.Integration.Tests/   testes contra PostgreSQL de verdade 🚧
web/                      front-end React 🚧
```

**Regra:** as dependências apontam para dentro. O `Domain` não conhece banco nem web.

Onde colocar o seu código:

- Regra que vale sem banco e sem HTTP (ex.: "não pode cancelar duas vezes") → `Domain`.
- Fluxo que busca dados e chama o Domain (ex.: "agendar horário") → `Application`.
- SQL, EF Core, acesso a serviço externo → `Infrastructure`.
- Rota HTTP (`MapGet`, `MapPost`) → `Agenda.Web`.

## Como contribuir

1. Atualize a `main`: `git switch main` e `git pull`.
2. Crie uma branch: `git switch -c feat/nome-da-tarefa`.
   - `feat/` funcionalidade nova
   - `fix/` correção de bug
   - `chore/` configuração, CI, documentação
3. Código **e** testes na mesma branch.
4. Rode `dotnet build` e `dotnet test` antes de subir.
5. Suba a branch (`git push -u origin feat/nome-da-tarefa`) e abra um PR para a `main`.
6. O CI precisa ficar verde antes do merge. O merge é por squash.
7. Depois do merge, apague a branch e volte ao passo 1.

A `main` é protegida: não aceita push direto, só PR com o CI verde.

PR pequeno é revisado rápido. Uma tarefa do board por PR.

## Segredos e dados pessoais

O repositório é **público**.

- Nunca coloque senha, connection string ou token em arquivo versionado (incluindo `appsettings.json`). Use `dotnet user-secrets`.
- Nunca coloque nomes de alunos nem links de pastas da turma no repositório.
- A pasta `doc/` fica fora do Git (`.gitignore`) por esse motivo.

## Especificação

A especificação completa (v1.0) não fica no repositório. Peça o link ao Lead.
