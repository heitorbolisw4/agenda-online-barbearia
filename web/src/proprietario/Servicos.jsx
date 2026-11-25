import { useEffect, useState } from 'react'
import { gestao } from './fonte.js'

const VAZIO = { id: null, nome: '', duracaoMin: 30 }

// Lista de serviços com cadastro e edição no mesmo formulário.
// Desativar não apaga: o serviço some da reserva do cliente, mas as reservas antigas continuam.
export default function Servicos() {
  const [servicos, setServicos] = useState(null)
  const [form, setForm] = useState(VAZIO)
  const [salvando, setSalvando] = useState(false)
  const [erro, setErro] = useState('')

  const carregar = () => gestao.servicos().then(setServicos).catch((e) => setErro(e.message))
  useEffect(() => { carregar() }, [])

  async function salvar(e) {
    e.preventDefault()
    setErro('')
    setSalvando(true)
    const dados = { nome: form.nome.trim(), duracaoMin: Number(form.duracaoMin) }
    try {
      if (form.id) await gestao.editarServico(form.id, dados)
      else await gestao.criarServico(dados)
      setForm(VAZIO)
      await carregar()
    } catch (e) {
      setErro(e.message)
    } finally {
      setSalvando(false)
    }
  }

  async function alternar(s) {
    setErro('')
    try {
      await gestao.ativarServico(s.id, !s.ativo)
      await carregar()
    } catch (e) {
      setErro(e.message)
    }
  }

  return (
    <>
      <section className="cartao">
        <h2>{form.id ? 'Editar serviço' : 'Novo serviço'}</h2>
        <form onSubmit={salvar} className="formulario">
          <label>
            Nome
            <input value={form.nome} onChange={(e) => setForm({ ...form, nome: e.target.value })} maxLength={60} required />
          </label>
          <label>
            Duração (minutos)
            <input type="number" min={5} max={480} step={5} value={form.duracaoMin}
              onChange={(e) => setForm({ ...form, duracaoMin: e.target.value })} required />
          </label>
          {erro && <p className="erro">{erro}</p>}
          <div className="opcoes">
            <button className="principal" disabled={salvando}>{salvando ? 'Salvando…' : form.id ? 'Salvar alterações' : 'Cadastrar'}</button>
            {form.id && <button type="button" onClick={() => { setForm(VAZIO); setErro('') }}>Cancelar edição</button>}
          </div>
        </form>
      </section>

      <section className="cartao">
        <h2>Serviços</h2>
        {servicos === null && <p className="mudo">Carregando…</p>}
        {servicos?.length === 0 && <p className="mudo">Nenhum serviço cadastrado.</p>}
        <ul className="itens">
          {servicos?.map((s) => (
            <li key={s.id} className={s.ativo ? '' : 'inativo'}>
              <span className="quem">
                <b>{s.nome}</b>
                <span className="mudo">{s.duracaoMin} min{s.ativo ? '' : ' · desativado'}</span>
              </span>
              <span className="acoes">
                <button onClick={() => { setForm({ id: s.id, nome: s.nome, duracaoMin: s.duracaoMin }); setErro('') }}>Editar</button>
                <button onClick={() => alternar(s)}>{s.ativo ? 'Desativar' : 'Ativar'}</button>
              </span>
            </li>
          ))}
        </ul>
      </section>
    </>
  )
}
