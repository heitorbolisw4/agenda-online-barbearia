using Agenda.Application.Exceptions;
using Agenda.Application.Interface;
using Agenda.Domain;

namespace Agenda.Application;

public class CancelarAgendamento(IAgendamentoEscrita escrita)
{
    public async Task<Agendamento> Executar(Guid agendamentoId, Guid profissionalId)
    {
        var agendamento = await escrita.ObterPorId(agendamentoId);
        if(agendamento is null || agendamento.ProfissionalId != profissionalId)
        {
            throw new NotFoundException("Agendamento nao encontrado");
            
        }


        // A regra (só cancela se estiver Confirmado) mora no Domain.
        agendamento.Cancelar();

        await escrita.Salvar();
        return agendamento;
    }
}
