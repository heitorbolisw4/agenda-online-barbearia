import { apiGestao } from '../api.js'
import { gestaoFalsa } from './dadosFalsos.js'

// De onde a gestão busca os dados: API de verdade ou dados de exemplo na memória.
export const DADOS_FALSOS = import.meta.env.VITE_GESTAO_FALSA === 'true'
export const gestao = DADOS_FALSOS ? gestaoFalsa : apiGestao
