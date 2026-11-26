import { useState } from 'react'
import { sessao } from '../api.js'
import { DADOS_FALSOS } from './fonte.js'
import Servicos from './Servicos.jsx'
import Barbeiros from './Barbeiros.jsx'

// Área do proprietário: cadastro de serviços e de barbeiros, uma aba de cada vez.
export default function Gestao() {
  const [aba, setAba] = useState('servicos')

  return (
    <div className="fluxo">
      {DADOS_FALSOS && (
        <p className="aviso">Dados de exemplo. Nada aqui vai para o banco; recarregar a página desfaz tudo.</p>
      )}

      <section className="cartao">
        {sessao.nome() && (
          <div className="quem-logou">
            <span>Olá, <b>{sessao.nome()}</b></span>
            <span className="opcoes">
              <a href="#/painel">Minha agenda</a>
              <button className="link" onClick={() => sessao.sair()}>Sair</button>
            </span>
          </div>
        )}
        <div className="opcoes" role="tablist">
          <button role="tab" aria-selected={aba === 'servicos'} className={aba === 'servicos' ? 'escolhido' : ''} onClick={() => setAba('servicos')}>
            Serviços
          </button>
          <button role="tab" aria-selected={aba === 'barbeiros'} className={aba === 'barbeiros' ? 'escolhido' : ''} onClick={() => setAba('barbeiros')}>
            Barbeiros
          </button>
        </div>
      </section>

      {aba === 'servicos' ? <Servicos /> : <Barbeiros />}
    </div>
  )
}
