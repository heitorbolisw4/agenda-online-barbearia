import { useEffect, useState } from 'react'
import Reservar from './publica/Reservar.jsx'
import AgendaDoDia from './barbeiro/AgendaDoDia.jsx'
import Entrar from './barbeiro/Entrar.jsx'
import Gestao from './proprietario/Gestao.jsx'
import { DADOS_FALSOS } from './proprietario/fonte.js'
import { sessao } from './api.js'

// Três endereços, sem link entre eles:
//   /          → cliente reserva
//   /#/painel  → barbeiro (passado só para a equipe)
//   /#/gestao  → proprietário
function rotaAtual() {
  if (window.location.hash === '#/painel') return 'painel'
  if (window.location.hash === '#/gestao') return 'gestao'
  return 'publica'
}

// Barbeiro logado abriu /#/gestao: explica e oferece o caminho de volta.
function SoProprietario() {
  return (
    <section className="cartao entrar">
      <h2>Área do proprietário</h2>
      <p className="mudo">Você entrou como barbeiro. Esta área é só do proprietário.</p>
      <div className="opcoes">
        <a href="#/painel">Ir para minha agenda</a>
        <button className="link" onClick={() => sessao.sair()}>Entrar com outra conta</button>
      </div>
    </section>
  )
}

export default function App() {
  const [rota, setRota] = useState(rotaAtual())
  const [logado, setLogado] = useState(Boolean(sessao.token()))

  useEffect(() => {
    const aoMudarRota = () => setRota(rotaAtual())
    const aoMudarSessao = () => setLogado(Boolean(sessao.token()))
    window.addEventListener('hashchange', aoMudarRota)
    window.addEventListener('sessao', aoMudarSessao)
    return () => {
      window.removeEventListener('hashchange', aoMudarRota)
      window.removeEventListener('sessao', aoMudarSessao)
    }
  }, [])

  // Área privada: sem login, o painel mostra só a tela de entrar.
  // Esconder aqui é conforto; quem protege de verdade é a API (401 sem token).
  const painel = logado ? <AgendaDoDia /> : <Entrar />

  // Gestão: precisa de login com papel Proprietario. Com dados de exemplo, abre direto.
  let gestao = <Gestao />
  if (!DADOS_FALSOS) {
    if (!logado) gestao = <Entrar titulo="Área do proprietário" />
    else if (sessao.papel() !== 'Proprietario') gestao = <SoProprietario />
  }

  const telas = { painel, gestao, publica: <Reservar /> }

  return (
    <div className="pagina">
      <header className="topo">
        <h1>Barbearia Luciano</h1>
      </header>
      <main>{telas[rota]}</main>
    </div>
  )
}
