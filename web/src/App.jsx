import { useEffect, useState } from 'react'
import Reservar from './publica/Reservar.jsx'
import AgendaDoDia from './barbeiro/AgendaDoDia.jsx'
import Entrar from './barbeiro/Entrar.jsx'
import { sessao } from './api.js'

// Dois endereços, sem link entre eles:
//   /          → cliente reserva
//   /#/painel  → barbeiro (passado só para a equipe)
function rotaAtual() {
  return window.location.hash === '#/painel' ? 'painel' : 'publica'
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

  return (
    <div className="pagina">
      <header className="topo">
        <h1>Barbearia Luciano</h1>
      </header>
      <main>{rota === 'painel' ? painel : <Reservar />}</main>
    </div>
  )
}
