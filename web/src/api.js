// Único lugar que sabe o endereço da API. O resto do front só chama estas funções.
const BASE = import.meta.env.VITE_API_URL ?? 'http://localhost:5195'

async function chamar(caminho, opcoes = {}) {
  const resposta = await fetch(BASE + caminho, {
    headers: { 'Content-Type': 'application/json' },
    ...opcoes,
  })
  const corpo = await resposta.json().catch(() => null)
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
  agendaDoDia: (profissionalId, data) =>
    chamar(`/profissionais/${profissionalId}/agenda?data=${data}&incluirCancelados=true`),
  cancelar: (id) => chamar(`/agendamentos/${id}/cancelar`, { method: 'POST' }),
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
