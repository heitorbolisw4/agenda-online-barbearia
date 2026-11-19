import { useEffect, useState } from 'react'
import { api, hojeEmSaoPaulo, somarDias, hora, dataPorExtenso } from '../api.js'

// Reserva em etapas: uma pergunta por tela. Escolheu, avança.
const ETAPAS = ['servico', 'barbeiro', 'dia', 'horario', 'dados']
const TITULOS = {
  servico: 'Qual serviço?',
  barbeiro: 'Com qual barbeiro?',
  dia: 'Que dia?',
  horario: 'Que horário?',
  dados: 'Seus dados',
}
const DIAS_VISIVEIS = 14

export default function Reservar() {
  const [etapa, setEtapa] = useState('servico')
  const [servicos, setServicos] = useState([])
  const [profissionais, setProfissionais] = useState([])
  const [servico, setServico] = useState(null)
  const [profissional, setProfissional] = useState(null)
  const [data, setData] = useState(null)
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

  // Ao chegar na etapa de horário, busca os horários livres daquele dia.
  useEffect(() => {
    if (etapa !== 'horario') return
    let ignorar = false
    setHorarios(null)
    api.horariosLivres(profissional.id, servico.id, data)
      .then((h) => { if (!ignorar) setHorarios(h) })
      .catch((e) => setErro(e.message))
    return () => { ignorar = true }
  }, [etapa, servico, profissional, data])

  const indice = ETAPAS.indexOf(etapa)
  const avancar = () => { setErro(''); setEtapa(ETAPAS[indice + 1]) }
  const voltar = () => { setErro(''); setEtapa(ETAPAS[indice - 1]) }

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
        // Alguém pegou o horário antes: volta para a escolha de horário.
        setHorario(null)
        setEtapa('horario')
        setErro('Alguém acabou de pegar esse horário. Escolha outro.')
      } else {
        setErro(e.message)
      }
    } finally {
      setEnviando(false)
    }
  }

  function recomecar() {
    setComprovante(null); setServico(null); setProfissional(null); setData(null); setHorario(null)
    setNome(''); setTelefone(''); setErro(''); setEtapa('servico')
  }

  if (comprovante) {
    return (
      <section className="cartao comprovante">
        <h2>Reserva confirmada ✔</h2>
        <p><b>{comprovante.servicoNome}</b> com <b>{profissional.nome}</b></p>
        <p>{dataPorExtenso(data)}, das {hora(comprovante.inicio)} às {hora(comprovante.fim)}</p>
        <p>Cliente: {comprovante.clienteNome}</p>
        <p className="mudo">Nº da reserva: {comprovante.id}</p>
        <button className="principal" onClick={recomecar}>Fazer outra reserva</button>
      </section>
    )
  }

  const hoje = hojeEmSaoPaulo()
  // A barbearia não abre no domingo: nem mostra.
  const dias = Array.from({ length: DIAS_VISIVEIS }, (_, i) => somarDias(hoje, i))
    .filter((d) => new Date(d + 'T12:00:00Z').getUTCDay() !== 0)

  return (
    <div className="fluxo">
      <div className="progresso" aria-label={`Etapa ${indice + 1} de ${ETAPAS.length}`}>
        <span className="mudo">Etapa {indice + 1} de {ETAPAS.length}</span>
        <div className="barra"><div style={{ width: `${((indice + 1) / ETAPAS.length) * 100}%` }} /></div>
      </div>

      {/* O que já foi escolhido, para o cliente não se perder. */}
      {indice > 0 && (
        <p className="escolhas">
          {servico && <span>{servico.nome}</span>}
          {indice > 1 && profissional && <span>{profissional.nome}</span>}
          {indice > 2 && data && <span>{dataPorExtenso(data)}</span>}
          {indice > 3 && horario && <span>{hora(horario.inicio)}</span>}
        </p>
      )}

      {erro && <p className="erro">{erro}</p>}

      <section className="cartao etapa" key={etapa}>
        <h2>{TITULOS[etapa]}</h2>

        {etapa === 'servico' && (
          <div className="lista">
            {servicos.map((s) => (
              <button key={s.id} onClick={() => { setServico(s); avancar() }}>
                {s.nome} <span className="mudo">{s.duracaoMin} min</span>
              </button>
            ))}
          </div>
        )}

        {etapa === 'barbeiro' && (
          <div className="lista">
            {profissionais.map((p) => (
              <button key={p.id} onClick={() => { setProfissional(p); avancar() }}>{p.nome}</button>
            ))}
          </div>
        )}

        {etapa === 'dia' && (
          <div className="grade">
            {dias.map((d) => (
              <button key={d} className="botao-dia" onClick={() => { setData(d); avancar() }}>
                {dataPorExtenso(d)}
              </button>
            ))}
          </div>
        )}

        {etapa === 'horario' && (
          <>
            {horarios === null && <p className="mudo">Carregando…</p>}
            {horarios?.length === 0 && <p className="mudo">Sem horários livres neste dia. Volte e escolha outro dia.</p>}
            <div className="grade">
              {horarios?.map((h) => (
                <button key={h.inicio} onClick={() => { setHorario(h); avancar() }}>{hora(h.inicio)}</button>
              ))}
            </div>
          </>
        )}

        {etapa === 'dados' && (
          <form onSubmit={confirmar} className="formulario">
            <label>Nome<input value={nome} onChange={(e) => setNome(e.target.value)} required autoFocus /></label>
            <label>Celular (WhatsApp)<input value={telefone} onChange={(e) => setTelefone(e.target.value)} inputMode="tel" required /></label>
            <p className="resumo">
              {servico.nome} com {profissional.nome}, {dataPorExtenso(data)} às {hora(horario.inicio)}
            </p>
            <button type="submit" className="principal" disabled={enviando}>
              {enviando ? 'Confirmando…' : 'Confirmar reserva'}
            </button>
          </form>
        )}
      </section>

      {indice > 0 && <button className="link" onClick={voltar}>← Voltar</button>}
    </div>
  )
}
