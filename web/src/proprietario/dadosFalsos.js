// API de mentira para montar a tela antes do back ficar pronto.
// Mesmas funções e mesmos formatos de apiGestao (api.js). Os dados vivem só na memória:
// recarregar a página volta tudo ao começo.
// Liga com VITE_GESTAO_FALSA=true no web/.env.local. Apague este arquivo quando a API existir.

let servicos = [
  { id: 's1', nome: 'Corte', duracaoMin: 30, ativo: true },
  { id: 's2', nome: 'Barba', duracaoMin: 20, ativo: true },
  { id: 's3', nome: 'Corte + Barba', duracaoMin: 50, ativo: true },
  { id: 's4', nome: 'Pigmentação', duracaoMin: 40, ativo: false },
]

let profissionais = [
  { id: 'p1', nome: 'Luciano', login: 'luciano', papel: 'Proprietario', ativo: true },
  { id: 'p2', nome: 'Barbeiro Exemplo', login: 'exemplo', papel: 'Barbeiro', ativo: true },
]

// Simula a demora da rede e o erro no formato que chamar() lança.
const esperar = (valor) => new Promise((ok) => setTimeout(() => ok(structuredClone(valor)), 300))
function falhar(status, mensagem) {
  const erro = new Error(mensagem)
  erro.status = status
  return new Promise((_, nao) => setTimeout(() => nao(erro), 300))
}

const porNome = (a, b) => a.nome.localeCompare(b.nome, 'pt-BR')
const novoId = () => crypto.randomUUID()

function validarServico({ nome, duracaoMin }) {
  if (!nome?.trim()) return 'O nome do serviço não pode estar vazio.'
  if (!(duracaoMin > 0)) return 'Informe uma duração maior que zero.'
  return null
}

export const gestaoFalsa = {
  servicos: () => esperar([...servicos].sort(porNome)),

  criarServico(dados) {
    const problema = validarServico(dados)
    if (problema) return falhar(400, problema)
    const novo = { id: novoId(), nome: dados.nome.trim(), duracaoMin: dados.duracaoMin, ativo: true }
    servicos.push(novo)
    return esperar(novo)
  },

  editarServico(id, dados) {
    const problema = validarServico(dados)
    if (problema) return falhar(400, problema)
    const s = servicos.find((x) => x.id === id)
    if (!s) return falhar(404, 'Serviço não encontrado.')
    Object.assign(s, { nome: dados.nome.trim(), duracaoMin: dados.duracaoMin })
    return esperar(s)
  },

  ativarServico(id, ativo) {
    const s = servicos.find((x) => x.id === id)
    if (!s) return falhar(404, 'Serviço não encontrado.')
    s.ativo = ativo
    return esperar(s)
  },

  profissionais: () => esperar([...profissionais].sort(porNome)),

  criarProfissional({ nome, login, senha }) {
    if (!nome?.trim() || !login?.trim()) return falhar(400, 'Preencha nome e login.')
    if (!senha || senha.length < 6) return falhar(400, 'A senha precisa de pelo menos 6 caracteres.')
    if (profissionais.some((p) => p.login === login.trim())) return falhar(409, 'Esse login já está em uso.')
    const novo = { id: novoId(), nome: nome.trim(), login: login.trim(), papel: 'Barbeiro', ativo: true }
    profissionais.push(novo)
    return esperar(novo)
  },

  ativarProfissional(id, ativo) {
    const p = profissionais.find((x) => x.id === id)
    if (!p) return falhar(404, 'Barbeiro não encontrado.')
    if (p.papel === 'Proprietario' && !ativo) return falhar(422, 'O proprietário não pode desativar a si mesmo.')
    p.ativo = ativo
    return esperar(p)
  },
}
