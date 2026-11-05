# Instruções para agentes de IA

Este arquivo vale para qualquer agente de IA (Claude Code, Copilot, Cursor, Codex etc.) que trabalhe neste repositório. Leia o [`README.md`](README.md) também.

## Contexto

- Projeto acadêmico: agenda online de uma barbearia.
- A equipe está **aprendendo**. O objetivo é entregar e também entender o código.
- O repositório é **público**.

## Como o time trabalha

- Uma tarefa por branch, uma branch por PR. Prefixos: `feat/`, `fix/`, `chore/`.
- Nunca faça push direto na `main`. Ela é protegida e só aceita PR com CI verde.
- O CI roda `dotnet restore`, `dotnet build` e `dotnet test`. Rode os mesmos comandos antes de propor um commit.
- Código e testes vão no mesmo PR.
- PRs pequenos. Não misture refatoração com funcionalidade nova.

## Arquitetura do back-end

Camadas em `src/`, com dependências apontando para dentro:

- `Agenda.Domain`: regras puras. Não referencia nenhum outro projeto, não acessa banco, HTTP nem relógio do sistema (`DateTime.Now`). Regra inválida lança `DomainException`.
- `Agenda.Application`: casos de uso e interfaces (repositórios). Chama o Domain.
- `Agenda.Infrastructure`: PostgreSQL, EF Core, implementações das interfaces.
- `Agenda.Web`: endpoints HTTP e injeção de dependência no `Program.cs`.

Testes em `tests/` com xUnit. Regra do Domain se testa em `Agenda.Domain.Tests`, sem banco.

Não mude essa estrutura (novos projetos, novas referências entre projetos) sem a pessoa pedir.

## Front-end

- Fica só em `web/`, em React.
- Fala com o back só pela API HTTP. Não escreva código em `src/` para resolver um problema do front.

## Regras que você não pode quebrar

- Nunca escreva senha, connection string, token ou chave em arquivo versionado. Use `dotnet user-secrets` ou `.env.local`.
- Nunca coloque nomes de alunos, e-mails da turma ou links de pastas compartilhadas no repositório.
- Não edite nem crie nada em `doc/`. Essa pasta fica fora do Git.
- Não desative, pule ou apague testes para o CI passar.
- Não altere `global.json` nem `.github/workflows/` sem a pessoa pedir.

## Como ajudar quem está aprendendo

- Explique o motivo da mudança, não só o código.
- Prefira mudanças pequenas que a pessoa consiga revisar e entender.
- Se a pessoa pedir para aprender, guie passo a passo em vez de entregar tudo pronto.
- Se um pedido conflitar com as regras acima, avise antes de fazer.

## Idioma

Código (nomes de classes, métodos, variáveis) e mensagens de commit em português, seguindo o que já existe no repositório.
