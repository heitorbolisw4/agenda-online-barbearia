# Agenda Online · Barbearia Luciano

Sistema de agendamento online da Barbearia Luciano (Araguari/MG).

- O **cliente** reserva sozinho, sem criar conta.
- O **barbeiro** vê as reservas na agenda dele.
- O **proprietário** gerencia barbeiros, serviços e horários.

Projeto acadêmico.

## Índice

1. [Estado atual](#estado-atual)
2. [Stack](#stack)
3. [Estrutura de pastas](#estrutura-de-pastas)
4. [Se você é Back-end, trabalhe aqui](#se-você-é-back-end-trabalhe-aqui)
5. [Se você é Front-end, trabalhe aqui](#se-você-é-front-end-trabalhe-aqui)
6. [Como rodar o Back-end](#como-rodar-o-back-end)
7. [Como rodar o Front-end](#como-rodar-o-front-end)
8. [Como contribuir](#como-contribuir)
9. [Segredos e dados pessoais](#segredos-e-dados-pessoais)
10. [Usando agentes de IA](#usando-agentes-de-ia)

## Estado atual

Legenda: ✅ na `main` · 🚧 planejado, ainda não está na `main`

| Parte | Estado |
| --- | --- |
| Domain: `Intervalo`, `CalculadoraDisponibilidade`, `Agendamento` + testes | ✅ |
| CI (build e testes em todo PR) | ✅ |
| Application: casos de uso (agendar, cancelar, listar horários) | 🚧 |
| Infrastructure: PostgreSQL + Entity Framework Core | 🚧 |
| API: endpoints reais (hoje só tem o exemplo `/weatherforecast`) | 🚧 |
| Front-end React em `web/` | 🚧 (pasta ainda não existe) |
| Testes de integração contra PostgreSQL | 🚧 (só o teste vazio de exemplo) |

## Stack

| Parte | Tecnologia |
| --- | --- |
| Back-end | .NET 10 (ASP.NET Core Web API) |
| Banco | PostgreSQL + Entity Framework Core 🚧 |
| Front-end | React, na pasta `web/` 🚧 |
| Testes | xUnit |
| CI | GitHub Actions |

## Estrutura de pastas

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

## Se você é Back-end, trabalhe aqui

Você mexe em `src/` e `tests/`. Não mexe em `web/`.

| O que você vai escrever | Onde fica |
| --- | --- |
| Regra que vale sem banco e sem HTTP (ex.: "não pode cancelar duas vezes") | `src/Agenda.Domain/` |
| Fluxo que busca dados e chama o Domain (ex.: "agendar horário") | `src/Agenda.Application/` |
| Interface de repositório (ex.: `IAgendamentoRepository`) | `src/Agenda.Application/` |
| SQL, EF Core, implementação do repositório | `src/Agenda.Infrastructure/` |
| Rota HTTP (`MapGet`, `MapPost`) e injeção de dependência | `src/Agenda.Web/` |
| Teste de regra pura | `tests/Agenda.Domain.Tests/` |
| Teste que precisa do banco | `tests/Agenda.Integration.Tests/` |

Regras:

- Toda regra nova no `Domain` vem com teste no mesmo PR.
- Regra inválida lança `DomainException`.
- O `Domain` não usa `DateTime.Now`. Quem precisa da hora atual recebe ela de fora.

## Se você é Front-end, trabalhe aqui

Você mexe só em `web/`. Não mexe em `src/` nem em `tests/`.

O front fala com o back **só pela API HTTP**. Se faltar um endpoint, peça ao time de back em vez de escrever código em `src/`.

Divisão planejada dentro de `web/` 🚧:

| Área | O que tem |
| --- | --- |
| Pública | Cliente escolhe serviço, barbeiro e horário, e confirma a reserva |
| Painel do barbeiro | Agenda do dia, chegada do cliente, cancelamentos |
| Gestão do proprietário | Cadastro de barbeiros, serviços e horários |

Cada área fica em sua própria pasta dentro de `web/src/`. Assim, duas pessoas não editam o mesmo arquivo ao mesmo tempo.

## Como rodar o Back-end

### 1. Instale as ferramentas

| Ferramenta | Para quê | Como conferir |
| --- | --- | --- |
| Git | baixar e enviar código | `git --version` |
| .NET SDK **10.0.400** ou mais novo da linha 10.0.4xx | compilar e rodar | `dotnet --version` |
| Editor: VS Code (com a extensão C# Dev Kit), Visual Studio 2026 ou Rider | escrever código | — |
| PostgreSQL 🚧 | banco de dados | `psql --version` |

No Windows, dá para instalar o SDK pelo terminal:

```powershell
winget install Microsoft.DotNet.SDK.10
```

A versão do SDK fica travada no `global.json`. Se `dotnet --version` mostrar outra linha (ex.: 9.x), o build falha.

### 2. Clone o repositório

```bash
git clone https://github.com/heitorbolisw4/agenda-online-barbearia.git
cd agenda-online-barbearia
```

### 3. Compile e rode os testes

```bash
dotnet build
dotnet test
```

Se os dois passarem, seu ambiente está pronto. O CI roda exatamente esses comandos.

Para rodar só um projeto de teste:

```bash
dotnet test tests/Agenda.Domain.Tests
```

### 4. Suba a API

```bash
dotnet run --project src/Agenda.Web
```

A API sobe em `http://localhost:5195`.

Para testar se está viva, abra no navegador:

- `http://localhost:5195/weatherforecast` (endpoint de exemplo)
- `http://localhost:5195/openapi/v1.json` (documentação da API, só em desenvolvimento)

Para usar HTTPS (`https://localhost:7092`), confie no certificado de desenvolvimento uma vez e rode com o perfil `https`:

```bash
dotnet dev-certs https --trust
dotnet run --project src/Agenda.Web --launch-profile https
```

### 5. Configure o banco 🚧

Ainda não está na `main`. O plano:

1. Instalar o PostgreSQL e criar um banco local chamado `agenda`.
2. Guardar a connection string fora do Git, com `dotnet user-secrets`:

   ```bash
   dotnet user-secrets init --project src/Agenda.Web
   dotnet user-secrets set "ConnectionStrings:Agenda" "Host=localhost;Database=agenda;Username=postgres;Password=SUA_SENHA" --project src/Agenda.Web
   ```

3. Instalar a ferramenta do EF Core e criar as tabelas:

   ```bash
   dotnet tool install --global dotnet-ef
   dotnet ef database update --project src/Agenda.Infrastructure --startup-project src/Agenda.Web
   ```

Os nomes exatos (chave da connection string, projeto das migrations) são confirmados no PR que trouxer o banco.

### Problemas comuns

| Erro | Causa provável |
| --- | --- |
| `A compatible .NET SDK was not found` | SDK diferente do `global.json`. Instale o 10.0.4xx. |
| `address already in use` na porta 5195 | Outra instância da API está rodando. Feche o outro terminal. |
| Navegador reclama do certificado em `https` | Rode `dotnet dev-certs https --trust`. |

## Como rodar o Front-end

🚧 A pasta `web/` ainda não existe. Este é o passo a passo planejado. Quem criar a pasta confirma os comandos neste README.

### 1. Instale as ferramentas

| Ferramenta | Para quê | Como conferir |
| --- | --- | --- |
| Git | baixar e enviar código | `git --version` |
| Node.js **LTS** (vem com o npm) | rodar o React | `node --version` e `npm --version` |
| Editor: VS Code | escrever código | — |

No Windows:

```powershell
winget install OpenJS.NodeJS.LTS
```

### 2. Clone o repositório

Mesmo passo do back-end:

```bash
git clone https://github.com/heitorbolisw4/agenda-online-barbearia.git
cd agenda-online-barbearia
```

### 3. Instale as dependências do front

```bash
cd web
npm install
```

Rode `npm install` de novo sempre que o `package.json` mudar depois de um `git pull`.

### 4. Suba o back-end

O front precisa da API rodando. Em **outro terminal**, na raiz do repositório:

```bash
dotnet run --project src/Agenda.Web
```

Se você não quer instalar o .NET, peça a alguém do back para combinar um mock da API.

### 5. Suba o front

```bash
npm run dev
```

O endereço do front aparece no terminal (com Vite, normalmente `http://localhost:5173`).

O endereço da API fica numa variável de ambiente em `web/.env.local` (fora do Git). Exemplo com Vite:

```
VITE_API_URL=http://localhost:5195
```

### 6. Antes de abrir PR

```bash
npm run build   # precisa compilar sem erro
npm run lint    # se o projeto tiver lint configurado
```

## Como contribuir

### Por que trabalhamos com branches

Uma **branch** é uma cópia paralela do código onde você trabalha sem afetar ninguém.

- **A `main` sempre funciona.** Ela só recebe código que passou no CI. Quem clona hoje consegue rodar.
- **Ninguém atropela ninguém.** Oito pessoas mexendo direto na `main` geram conflito o tempo todo. Cada um na sua branch, o conflito só aparece no merge, e em um lugar só.
- **Todo código é revisado.** A branch vira um PR, e o PR é o lugar onde alguém lê o seu código antes de entrar.
- **Dá para desfazer.** Se uma ideia deu errado, você apaga a branch. A `main` nem ficou sabendo.

### Como criar uma branch

```bash
git switch main                       # vai para a main
git pull                              # baixa o que os outros já juntaram
git switch -c feat/nome-da-tarefa     # cria a branch nova e já entra nela
```

Confira em qual branch você está:

```bash
git branch        # a branch atual aparece com *
git status        # mostra a branch e os arquivos alterados
```

Para voltar a uma branch que já existe (sem `-c`):

```bash
git switch feat/nome-da-tarefa
```

**Armadilha:** sempre crie a branch a partir da `main` atualizada. Se você criar a partir de outra branch, o seu PR leva junto o código dela.

Prefixos do nome:

- `feat/` funcionalidade nova (ex.: `feat/cancelar-agendamento`)
- `fix/` correção de bug (ex.: `fix/horario-duplicado`)
- `chore/` configuração, CI, documentação (ex.: `chore/readme`)

Use letras minúsculas, sem acento e com hífen no lugar de espaço.

### Do commit ao merge

1. Crie a branch (passo acima).
2. Faça a tarefa. Salve o progresso com commits:

   ```bash
   git add .
   git commit -m "feat: cancela agendamento confirmado"
   ```

3. Código **e** testes na mesma branch.
4. Rode os testes antes de subir (`dotnet test` no back, `npm run build` no front).
5. Suba a branch (`git push -u origin feat/nome-da-tarefa`) e abra um PR para a `main`.
6. O CI precisa ficar verde antes do merge.
7. Depois do merge, apague a branch e volte ao passo 1.

A `main` é protegida: não aceita push direto, só PR com o CI verde.

PR pequeno é revisado rápido. Uma tarefa do board por PR.

## Segredos e dados pessoais

O repositório é **público**.

- Nunca coloque senha, connection string ou token em arquivo versionado (incluindo `appsettings.json`). Use `dotnet user-secrets` no back e `.env.local` no front.
- Nunca coloque nomes de alunos nem links de pastas da turma no repositório.
- A pasta `doc/` fica fora do Git (`.gitignore`) por esse motivo.

## Usando agentes de IA

Se você usa um agente de IA (Claude Code, Copilot, Cursor etc.), ele deve ler o [`AGENTS.md`](AGENTS.md) antes de mexer no código. Lá estão as regras de como o time trabalha.

## Especificação

A especificação completa (v1.0) não fica no repositório. Peça o link ao Lead.
