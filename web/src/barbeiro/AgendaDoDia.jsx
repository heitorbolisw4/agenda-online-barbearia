import { useEffect, useState } from 'react'
import { api, sessao, hojeEmSaoPaulo, somarDias, hora, dataPorExtenso } from '../api.js'

// Painel do barbeiro: agenda de um dia, com opção de cancelar.
// Só aparece depois do login. A API descobre o barbeiro pelo token, não pela tela.
export default function AgendaDoDia() {
  const [data, setData] = useState(hojeEmSaoPaulo())
  const [agenda, setAgenda] = useState(null)
  const [erro, setErro] = useState('')

  const [versao, setVersao] = useState(0)
  const carregar = () => setVersao((v) => v + 1)

  useEffect(() => {
    // Se o usuário trocar de dia rápido, ignora a resposta da busca antiga.
    let ignorar = false
    setAgenda(null)
    api.minhaAgenda(data)
      .then((a) => { if (!ignorar) setAgenda(a) })
      .catch((e) => setErro(e.message))
    return () => { ignorar = true }
  }, [data, versao])

  async function cancelar(item) {
    if (!window.confirm(`Cancelar ${item.servicoNome} de ${item.clienteNome} às ${hora(item.inicio)}?`)) return
    try {
      await api.cancelar(item.id)
      carregar()
    } catch (e) {
      setErro(e.message)
    }
  }

  const ativos = agenda?.filter((a) => a.status !== 'Cancelado').length ?? 0

  return (
    <div className="fluxo">
      {erro && <p className="erro">{erro}</p>}

      <section className="cartao">
        <div className="quem-logou">
          <span>Olá, <b>{sessao.nome()}</b></span>
          <button className="link" onClick={() => sessao.sair()}>Sair</button>
        </div>
        <div className="dia">
          <button onClick={() => setData(somarDias(data, -1))}>◀</button>
          <span>{dataPorExtenso(data)}</span>
          <button onClick={() => setData(somarDias(data, 1))}>▶</button>
        </div>
        <button className="link" onClick={() => setData(hojeEmSaoPaulo())}>Hoje</button>
      </section>

      <section className="cartao">
        <h2>Agenda {agenda && <span className="mudo">· {ativos} atendimento(s)</span>}</h2>
        {agenda === null && <p className="mudo">Carregando…</p>}
        {agenda?.length === 0 && <p className="mudo">Nenhuma reserva neste dia.</p>}
        <ul className="agenda">
          {agenda?.map((a) => (
            <li key={a.id} className={a.status === 'Cancelado' ? 'cancelado' : ''}>
              <span className="horario">{hora(a.inicio)}–{hora(a.fim)}</span>
              <span className="quem">
                <b>{a.clienteNome}</b> · {a.servicoNome}
                <a className="mudo" href={`https://wa.me/55${a.clienteTelefone.replace(/\D/g, '')}`} target="_blank" rel="noreferrer">
                  {a.clienteTelefone}
                </a>
              </span>
              <span className="status">{a.status}</span>
              {a.status === 'Confirmado' && <button onClick={() => cancelar(a)}>Cancelar</button>}
            </li>
          ))}
        </ul>
      </section>
    </div>
  )
}
