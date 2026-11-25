// Único lugar que sabe o endereço da API. O resto do front só chama estas funções.
const BASE = import.meta.env.VITE_API_URL ?? 'http://localhost:5195'

// Sessão do barbeiro. O token (JWT) que a API devolve no login fica no navegador
// e vai no header Authorization das rotas protegidas.
const CHAVE_TOKEN = 'agenda.token'

function lerPayload(token) {
  // O meio do JWT é JSON em base64url. Só lemos; quem confere a assinatura é a API.
  const base64 = token.split('.')[1].replace(/-/g, '+').replace(/_/g, '/')
  const json = decodeURIComponent(Array.from(atob(base64), (c) => '%' + c.charCodeAt(0).toString(16).padStart(2, '0')).join(''))
  return JSON.parse(json)
}

export const sessao = {
  token() {
    try {
      const token = localStorage.getItem(CHAVE_TOKEN)
      if (!token) return null
      // Venceu? Joga fora para não mandar um crachá que a API vai recusar.
      if (lerPayload(token).exp * 1000 <= Date.now()) { localStorage.removeItem(CHAVE_TOKEN); return null }
      return token
    } catch { return null }
  },
  nome() {
    const token = this.token()
    return token ? lerPayload(token).unique_name : null
  },
  // "Barbeiro" ou "Proprietario". Só decide o que a tela mostra; quem barra de verdade é a API (403).
  papel() {
    const token = this.token()
    return token ? lerPayload(token).role ?? null : null
  },
  entrar(token) { localStorage.setItem(CHAVE_TOKEN, token); window.dispatchEvent(new Event('sessao')) },
  sair() { localStorage.removeItem(CHAVE_TOKEN); window.dispatchEvent(new Event('sessao')) },
}

async function chamar(caminho, opcoes = {}) {
  const token = sessao.token()
  const resposta = await fetch(BASE + caminho, {
    ...opcoes,
    headers: {
      'Content-Type': 'application/json',
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
    },
  })
  const corpo = await resposta.json().catch(() => null)
  // Mandou crachá e a API recusou: venceu ou é inválido. Volta para o login.
  if (resposta.status === 401 && token) sessao.sair()
  if (!resposta.ok) {
    // A API devolve ProblemDetails ({ title, detail, status }) ou um texto simples.
    const mensagem = typeof corpo === 'string' ? corpo : corpo?.detail ?? 'Erro inesperado'
    const erro = new Error(mensagem)
    erro.status = resposta.status
    throw erro
  }
  return corpo
}

export const api = {
  servicos: () => chamar('/servicos'),
  profissionais: () => chamar('/profissionais'),
  horariosLivres: (profissionalId, servicoId, data) =>
    chamar(`/disponibilidade?profissionalId=${profissionalId}&servicoId=${servicoId}&data=${data}`),
  reservar: (dados) => chamar('/agendamentos', { method: 'POST', body: JSON.stringify(dados) }),
  entrar: (login, senha) => chamar('/login', { method: 'POST', body: JSON.stringify({ login, senha }) }),
  minhaAgenda: (data) => chamar(`/minha-agenda?data=${data}&incluirCancelados=true`),
  cancelar: (id) => chamar(`/agendamentos/${id}/cancelar`, { method: 'POST' }),
}

// Gestão do proprietário (contrato combinado com o back: rotas em /gestao).
export const apiGestao = {
  servicos: () => chamar('/gestao/servicos'),
  criarServico: (dados) => chamar('/gestao/servicos', { method: 'POST', body: JSON.stringify(dados) }),
  editarServico: (id, dados) => chamar(`/gestao/servicos/${id}`, { method: 'PUT', body: JSON.stringify(dados) }),
  ativarServico: (id, ativo) => chamar(`/gestao/servicos/${id}/${ativo ? 'ativar' : 'desativar'}`, { method: 'POST' }),
  profissionais: () => chamar('/gestao/profissionais'),
  criarProfissional: (dados) => chamar('/gestao/profissionais', { method: 'POST', body: JSON.stringify(dados) }),
  ativarProfissional: (id, ativo) => chamar(`/gestao/profissionais/${id}/${ativo ? 'ativar' : 'desativar'}`, { method: 'POST' }),
}

// Datas sempre no fuso da barbearia, não no fuso do navegador.
const FUSO = 'America/Sao_Paulo'

export function hojeEmSaoPaulo() {
  return new Intl.DateTimeFormat('en-CA', { timeZone: FUSO }).format(new Date()) // yyyy-mm-dd
}

export function somarDias(data, dias) {
  const d = new Date(data + 'T12:00:00Z')
  d.setUTCDate(d.getUTCDate() + dias)
  return d.toISOString().slice(0, 10)
}

export function hora(iso) {
  return new Intl.DateTimeFormat('pt-BR', { timeZone: FUSO, hour: '2-digit', minute: '2-digit' }).format(new Date(iso))
}

export function dataPorExtenso(data) {
  const texto = new Intl.DateTimeFormat('pt-BR', { timeZone: 'UTC', weekday: 'long', day: '2-digit', month: '2-digit' })
    .format(new Date(data + 'T12:00:00Z'))
  return texto.charAt(0).toUpperCase() + texto.slice(1) // "Sexta-feira, 20/11"
}
