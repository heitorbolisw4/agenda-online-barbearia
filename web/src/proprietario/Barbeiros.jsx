import { useEffect, useState } from 'react'
import { gestao } from './fonte.js'

const VAZIO = { nome: '', login: '', senha: '' }

// Lista de barbeiros com cadastro. Barbeiro desativado não entra mais no painel
// e some da escolha do cliente; as reservas antigas dele continuam.
export default function Barbeiros() {
  const [barbeiros, setBarbeiros] = useState(null)
  const [form, setForm] = useState(VAZIO)
  const [salvando, setSalvando] = useState(false)
  const [erro, setErro] = useState('')

  const carregar = () => gestao.profissionais().then(setBarbeiros).catch((e) => setErro(e.message))
  useEffect(() => { carregar() }, [])

  async function cadastrar(e) {
    e.preventDefault()
    setErro('')
    setSalvando(true)
    try {
      await gestao.criarProfissional({ nome: form.nome.trim(), login: form.login.trim(), senha: form.senha })
      setForm(VAZIO)
      await carregar()
    } catch (e) {
      setErro(e.message)
    } finally {
      setSalvando(false)
    }
  }

  async function alternar(b) {
    if (b.ativo && !window.confirm(`Desativar ${b.nome}? Ele não vai mais entrar no painel nem receber reservas.`)) return
    setErro('')
    try {
      await gestao.ativarProfissional(b.id, !b.ativo)
      await carregar()
    } catch (e) {
      setErro(e.message)
    }
  }

  const campo = (nome) => ({ value: form[nome], onChange: (e) => setForm({ ...form, [nome]: e.target.value }) })

  return (
    <>
      <section className="cartao">
        <h2>Novo barbeiro</h2>
        <form onSubmit={cadastrar} className="formulario">
          <label>
            Nome
            <input {...campo('nome')} maxLength={60} required />
          </label>
          <label>
            Login
            <input {...campo('login')} maxLength={40} autoCapitalize="none" autoComplete="off" required />
          </label>
          <label>
            Senha inicial
            <input {...campo('senha')} type="password" minLength={6} autoComplete="new-password" required />
            <span className="mudo">Mínimo de 6 caracteres. Passe para o barbeiro pessoalmente.</span>
          </label>
          {erro && <p className="erro">{erro}</p>}
          <div className="opcoes">
            <button className="principal" disabled={salvando}>{salvando ? 'Salvando…' : 'Cadastrar'}</button>
          </div>
        </form>
      </section>

      <section className="cartao">
        <h2>Barbeiros</h2>
        {barbeiros === null && <p className="mudo">Carregando…</p>}
        <ul className="itens">
          {barbeiros?.map((b) => (
            <li key={b.id} className={b.ativo ? '' : 'inativo'}>
              <span className="quem">
                <b>{b.nome}</b>
                <span className="mudo">
                  @{b.login}{b.papel === 'Proprietario' ? ' · proprietário' : ''}{b.ativo ? '' : ' · desativado'}
                </span>
              </span>
              {/* O proprietário não se desativa: ficaria trancado fora do sistema. */}
              {b.papel !== 'Proprietario' && (
                <span className="acoes">
                  <button onClick={() => alternar(b)}>{b.ativo ? 'Desativar' : 'Ativar'}</button>
                </span>
              )}
            </li>
          ))}
        </ul>
      </section>
    </>
  )
}
