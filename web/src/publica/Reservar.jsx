import { useEffect, useState } from 'react'
import { api, hojeEmSaoPaulo, somarDias, hora, dataPorExtenso } from '../api.js'

// Fluxo do cliente: serviço -> barbeiro e data -> horário -> dados -> comprovante.
export default function Reservar() {
  const [servicos, setServicos] = useState([])
  const [profissionais, setProfissionais] = useState([])
  const [servico, setServico] = useState(null)
  const [profissional, setProfissional] = useState(null)
  const [data, setData] = useState(hojeEmSaoPaulo())
  const [horarios, setHorarios] = useState(null)
  const [horario, setHorario] = useState(null)
  const [nome, setNome] = useState('')
  const [telefone, setTelefone] = useState('')
  const [enviando, setEnviando] = useState(false)
  const [erro, setErro] = useState('')
  const [comprovante, setComprovante] = useState(null)

  useEffect(() => {
    Promise.all([api.servicos(), api.profissionais()])
      .then(([s, p]) => { setServicos(s); setProfissionais(p) })
      .catch(() => setErro('Não foi possível falar com a API. Ela está rodando?'))
  }, [])

  // Sempre que muda serviço, barbeiro ou data, busca os horários livres de novo.
  useEffect(() => {
    if (!servico || !profissional) return
    let ignorar = false
    setHorarios(null)
    setHorario(null)
    api.horariosLivres(profissional.id, servico.id, data)
      .then((h) => { if (!ignorar) setHorarios(h) })
      .catch((e) => setErro(e.message))
    return () => { ignorar = true }
  }, [servico, profissional, data])

  async function confirmar(e) {
    e.preventDefault()
    setErro('')
    setEnviando(true)
    try {
      const reserva = await api.reservar({
        profissionalId: profissional.id,
        servicoId: servico.id,
        inicio: horario.inicio,
        clienteNome: nome,
        clienteTelefone: telefone,
      })
      setComprovante(reserva)
    } catch (e) {
      if (e.status === 409) {
        setErro('Alguém acabou de pegar esse horário. Escolha outro.')
        setHorario(null)
        setHorarios(await api.horariosLivres(profissional.id, servico.id, data))
      } else {
        setErro(e.message)
      }
    } finally {
      setEnviando(false)
    }
  }

  function recomecar() {
    setComprovante(null); setServico(null); setProfissional(null); setHorario(null)
    setNome(''); setTelefone(''); setErro('')
  }

  if (comprovante) {
    return (
      <section className="cartao comprovante">
        <h2>Reserva confirmada ✔</h2>
        <p><b>{comprovante.servicoNome}</b> com <b>{profissional.nome}</b></p>
        <p>{dataPorExtenso(data)}, das {hora(comprovante.inicio)} às {hora(comprovante.fim)}</p>
        <p>Cliente: {comprovante.clienteNome}</p>
        <p className="mudo">Nº da reserva: {comprovante.id}</p>
        <button onClick={recomecar}>Fazer outra reserva</button>
      </section>
    )
  }

  return (
    <div className="fluxo">
      {erro && <p className="erro">{erro}</p>}

      <section className="cartao">
        <h2>1. Serviço</h2>
        <div className="opcoes">
          {servicos.map((s) => (
            <button key={s.id} className={servico?.id === s.id ? 'escolhido' : ''} onClick={() => setServico(s)}>
              {s.nome} <span className="mudo">{s.duracaoMin} min</span>
            </button>
          ))}
        </div>
      </section>

      {servico && (
        <section className="cartao">
          <h2>2. Barbeiro e dia</h2>
          <div className="opcoes">
            {profissionais.map((p) => (
              <button key={p.id} className={profissional?.id === p.id ? 'escolhido' : ''} onClick={() => setProfissional(p)}>
                {p.nome}
              </button>
            ))}
          </div>
          <div className="dia">
            <button disabled={data <= hojeEmSaoPaulo()} onClick={() => setData(somarDias(data, -1))}>◀</button>
            <span>{dataPorExtenso(data)}</span>
            <button onClick={() => setData(somarDias(data, 1))}>▶</button>
          </div>
        </section>
      )}

      {servico && profissional && (
        <section className="cartao">
          <h2>3. Horário</h2>
          {horarios === null && <p className="mudo">Carregando…</p>}
          {horarios?.length === 0 && <p className="mudo">Sem horários livres neste dia. Tente outro.</p>}
          <div className="grade">
            {horarios?.map((h) => (
              <button key={h.inicio} className={horario?.inicio === h.inicio ? 'escolhido' : ''} onClick={() => setHorario(h)}>
                {hora(h.inicio)}
              </button>
            ))}
          </div>
        </section>
      )}

      {horario && (
        <form className="cartao" onSubmit={confirmar}>
          <h2>4. Seus dados</h2>
          <label>Nome<input value={nome} onChange={(e) => setNome(e.target.value)} required /></label>
          <label>Celular (WhatsApp)<input value={telefone} onChange={(e) => setTelefone(e.target.value)} inputMode="tel" required /></label>
          <p className="resumo">
            {servico.nome} com {profissional.nome}, {dataPorExtenso(data)} às {hora(horario.inicio)}
          </p>
          <button type="submit" className="principal" disabled={enviando}>
            {enviando ? 'Confirmando…' : 'Confirmar reserva'}
          </button>
        </form>
      )}
    </div>
  )
}
