import { useState } from 'react'
import { api, sessao } from '../api.js'

// Login de barbeiro e proprietário (mesma rota /login). Deu certo: guarda o token e o App troca de tela.
export default function Entrar({ titulo = 'Área do barbeiro' }) {
  const [login, setLogin] = useState('')
  const [senha, setSenha] = useState('')
  const [enviando, setEnviando] = useState(false)
  const [erro, setErro] = useState('')

  async function entrar(e) {
    e.preventDefault()
    setErro('')
    setEnviando(true)
    try {
      const { token } = await api.entrar(login.trim(), senha)
      sessao.entrar(token)
    } catch (e) {
      // 401 = login ou senha errados. A API não diz qual dos dois, de propósito.
      setErro(e.status === 401 ? 'Login ou senha incorretos.' : 'Não foi possível falar com a API. Ela está rodando?')
    } finally {
      setEnviando(false)
    }
  }

  return (
    <section className="cartao entrar">
      <h2>{titulo}</h2>
      <form onSubmit={entrar} className="formulario">
        <label>
          Login
          <input value={login} onChange={(e) => setLogin(e.target.value)} autoComplete="username" autoCapitalize="none" required autoFocus />
        </label>
        <label>
          Senha
          <input type="password" value={senha} onChange={(e) => setSenha(e.target.value)} autoComplete="current-password" required />
        </label>
        {erro && <p className="erro">{erro}</p>}
        <button className="principal" disabled={enviando}>{enviando ? 'Entrando…' : 'Entrar'}</button>
      </form>
    </section>
  )
}
