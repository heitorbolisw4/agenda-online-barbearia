# Agenda Online · Barbearia Luciano

Sistema de agendamento online da Barbearia Luciano (Araguari/MG). O cliente reserva sozinho, sem conta, e o barbeiro vê a reserva na agenda dele.

Projeto acadêmico. Entrega final em 03/12/2026.

## Stack

| Parte | Tecnologia |
| --- | --- |
| Back-end | .NET 10 (ASP.NET Core Web API) |
| Banco | PostgreSQL (a configurar no Sprint 1) |
| Front-end | React (pasta `web/`, a criar) |
| Testes | xUnit |
| CI | GitHub Actions (build e testes a cada push e PR) |

## Rodar localmente

**Pré-requisito:** .NET SDK **10.0.400** ou mais novo da linha 10.0.4xx. A versão fica travada no `global.json`.

```bash
dotnet --version   # confere o SDK instalado
```

**Clonar, compilar e testar:**

```bash
git clone https://github.com/heitorbolisw4/agenda-online-barbearia.git
cd agenda-online-barbearia
dotnet build
dotnet test
```

**Subir a API:**

```bash
dotnet run --project src/Agenda.Web
```

A porta aparece no terminal. Em desenvolvimento, a documentação da API fica em `/openapi/v1.json`.

## Estrutura

```
src/
  Agenda.Domain/          regras de negócio puras, não referencia nenhum projeto
  Agenda.Application/     casos de uso e interfaces (ex.: repositórios)
  Agenda.Infrastructure/  banco de dados e implementações das interfaces
  Agenda.Web/             API HTTP; liga todas as peças no Program.cs
tests/
  Agenda.Domain.Tests/        testes das regras puras, sem banco
  Agenda.Integration.Tests/   testes contra PostgreSQL de verdade
```

**Regra:** as dependências apontam para dentro. O `Domain` não conhece banco nem web.

## Como contribuir

1. Atualize a `main`: `git switch main` e `git pull`.
2. Crie uma branch: `git switch -c feat/nome-da-tarefa`.
   - `feat/` funcionalidade nova
   - `fix/` correção de bug
   - `chore/` configuração, CI, documentação
3. Código **e** testes na mesma branch.
4. Abra um PR para a `main`. O CI precisa passar antes do merge.
5. Depois do merge, apague a branch e volte ao passo 1.

A `main` é protegida: não aceita push direto, só PR com o CI verde.

## Segredos

O repositório é **público**. Nunca coloque senha, connection string ou token em arquivo versionado (incluindo `appsettings.json`). Use `dotnet user-secrets` no desenvolvimento.

## Especificação

A especificação completa (v1.0) não fica no repositório. Ela está na [pasta do Drive da disciplina](https://drive.google.com/drive/folders/1KlODrLl-931_7F_iNLPK9Ba-78MNFEm8) (acesso da turma).
