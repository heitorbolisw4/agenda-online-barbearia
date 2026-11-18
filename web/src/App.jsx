import { useEffect, useState } from 'react'
import Reservar from './publica/Reservar.jsx'
import AgendaDoDia from './barbeiro/AgendaDoDia.jsx'

// Navegação simples pelo "#" da URL: #/ = área pública, #/painel = barbeiro.
function rotaAtual() {
  return window.location.hash === '#/painel' ? 'painel' : 'publica'
}

export default function App() {
  const [rota, setRota] = useState(rotaAtual())

  useEffect(() => {
    const aoMudar = () => setRota(rotaAtual())
    window.addEventListener('hashchange', aoMudar)
    return () => window.removeEventListener('hashchange', aoMudar)
  }, [])

  return (
    <div className="pagina">
      <header className="topo">
        <h1>Barbearia Luciano</h1>
        <nav>
          <a href="#/" className={rota === 'publica' ? 'ativo' : ''}>Reservar</a>
          <a href="#/painel" className={rota === 'painel' ? 'ativo' : ''}>Painel do barbeiro</a>
        </nav>
      </header>
      <main>{rota === 'painel' ? <AgendaDoDia /> : <Reservar />}</main>
    </div>
  )
}
